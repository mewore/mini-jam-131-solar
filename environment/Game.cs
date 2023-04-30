using System;
using System.Collections.Generic;
using Godot;

public class Game : Node2D
{
    private const int SAMPLES_FOR_NOISE_ADJUSTMENT = 10;
    private const float TOP_MARGIN = 32;

    [Signal]
    public delegate void GameWon();

    [Export]
    private PackedScene smallObstacleScene = null;

    [Export]
    private PackedScene obstacleScene = null;

    [Export]
    private PackedScene bigObstacleScene = null;

    private OpenSimplexNoise obstacleCreationNoise = null;

    [Export]
    private float obstacleFrequency = .3f;

    [Export]
    private float obstacleMovementSpeed = 100f;

    private Node obstacleContainer;

    private NoiseTexture obstacleNoiseTexture;

    [Export]
    private PackedScene suncakeScene = null;
    private Node pickupContainer;

    private float now = 0f;

    private float obstacleX;
    private float maxObstacleY;

    public override void _Ready()
    {
        obstacleContainer = GetNode("Obstacles");
        obstacleNoiseTexture = obstacleContainer.GetNode<Sprite>("ObstacleNoise").Texture as NoiseTexture;
        obstacleCreationNoise = obstacleNoiseTexture.Noise;
        int seed = (Global.CurrentLocation.Hash() + Global.TargetLocation.Hash()).GetHashCode();
        obstacleCreationNoise.Seed = seed;
        pickupContainer = GetNode("Pickups");
    }

    private (float, float) getNoiseRange(float padding)
    {
        float min = 1f;
        float max = -1f;
        float sum = 0f;
        for (int sample = 0; sample < SAMPLES_FOR_NOISE_ADJUSTMENT; sample++)
        {
            Vector2 position = getRandomScrollingObjectPos(padding);
            float value = obstacleCreationNoise.GetNoise2d(position.x + now * obstacleMovementSpeed, position.y);
            min = Mathf.Min(min, value);
            max = Mathf.Max(max, value);
            sum += value;
        }
        return (min, max);
    }

    public void _on_SmallObstacleCreationTimer_timeout() => createObject(smallObstacleScene, 1, obstacleContainer, Obstacle.OBSTACLE_PADDING);
    public void _on_ObstacleCreationTimer_timeout() => createObject(obstacleScene, 2, obstacleContainer, Obstacle.OBSTACLE_PADDING);
    public void _on_BigObstacleCreationTimer_timeout() => createObject(bigObstacleScene, 8, obstacleContainer, Obstacle.OBSTACLE_PADDING);
    public void _on_SuncakeTimer_timeout() => createObject(suncakeScene, 4, pickupContainer, Suncake.SUNCAKE_PADDING, true);

    private void createObject(PackedScene scene, int additionalSamples, Node container, float padding, bool reverseSampling = false)
    {
        var noiseRange = getNoiseRange(padding);
        float threshold = (noiseRange.Item1 + (noiseRange.Item2 - noiseRange.Item1) * (1f - obstacleFrequency));
        Vector2 position = getRandomScrollingObjectPos(padding);
        Vector2 offset = Vector2.Right * now * obstacleMovementSpeed;
        float value = obstacleCreationNoise.GetNoise2dv(position + offset);
        if (reverseSampling ? (value > threshold) : (value < threshold))
        {
            return;
        }
        for (int sample = 0; sample < additionalSamples; sample++)
        {
            Vector2 otherPosition = getRandomScrollingObjectPos(padding);
            float currentValue = obstacleCreationNoise.GetNoise2dv(otherPosition + offset);
            if (reverseSampling ? (currentValue < value) : (currentValue > value))
            {
                position = otherPosition;
                value = currentValue;
            }
        }
        var createdObject = scene.Instance<ScrollingObject>();
        createdObject.ScrollSpeed = obstacleMovementSpeed;
        createdObject.Position = position;
        if (createdObject is Obstacle)
        {
            (createdObject as Obstacle).TargetViewport = GetNode<Viewport>("Obstacles/Viewport");
        }
        container.AddChild(createdObject as Node);
    }

    private Vector2 getRandomScrollingObjectPos(float padding)
    {
        return new Vector2(
            (int)ProjectSettings.GetSetting("display/window/size/width") + padding,
            TOP_MARGIN + GD.Randf() * ((int)ProjectSettings.GetSetting("display/window/size/height") - TOP_MARGIN)
        );
    }

    public override void _PhysicsProcess(float delta)
    {
        now += delta;
        obstacleNoiseTexture.NoiseOffset = new Vector2(now * obstacleMovementSpeed, 0f);


        // if (hasPlayerWon())
        // {
        //     if (Global.TryWinLevel(teleports))
        //     {
        //         GetTree().ChangeScene(Global.CurrentLevelPath);
        //     }
        //     else
        //     {
        //         EmitSignal(nameof(GameWon));
        //     }
        // }
    }
}

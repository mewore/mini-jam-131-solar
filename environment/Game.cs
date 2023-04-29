using System;
using System.Collections.Generic;
using Godot;

public class Game : Node2D
{
    private const int SAMPLES_FOR_NOISE_ADJUSTMENT = 10;
    private const float OBSTACLE_PADDING = 64;

    [Signal]
    public delegate void GameWon();

    [Export]
    private PackedScene pickupScene = null;

    private Node pickupContainer;

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

    private List<Node2D> obstacles = new List<Node2D>();

    private NoiseTexture obstacleNoiseTexture;

    private float now = 0f;

    private float obstacleX;
    private float maxObstacleY;

    public override void _Ready()
    {
        obstacleContainer = GetNode("Obstacles");
        obstacleNoiseTexture = obstacleContainer.GetNode<Sprite>("ObstacleNoise").Texture as NoiseTexture;
        obstacleCreationNoise = obstacleNoiseTexture.Noise;
    }

    private (float, float) getNoiseRange()
    {
        float min = 1f;
        float max = -1f;
        float sum = 0f;
        for (int sample = 0; sample < SAMPLES_FOR_NOISE_ADJUSTMENT; sample++)
        {
            Vector2 position = getRandomObstaclePos();
            float value = obstacleCreationNoise.GetNoise2d(position.x + now * obstacleMovementSpeed, position.y);
            min = Mathf.Min(min, value);
            max = Mathf.Max(max, value);
            sum += value;
        }
        return (min, max);
    }

    public void _on_SmallObstacleCreationTimer_timeout() => createObstacle(smallObstacleScene, 1);
    public void _on_ObstacleCreationTimer_timeout() => createObstacle(obstacleScene, 2);
    public void _on_BigObstacleCreationTimer_timeout() => createObstacle(bigObstacleScene, 8);

    private void createObstacle(PackedScene scene, int additionalSamples)
    {
        var noiseRange = getNoiseRange();
        float threshold = noiseRange.Item1 + (noiseRange.Item2 - noiseRange.Item1) * (1f - obstacleFrequency);
        Vector2 position = getRandomObstaclePos();
        Vector2 offset = Vector2.Right * now * obstacleMovementSpeed;
        float value = obstacleCreationNoise.GetNoise2dv(position + offset);
        if (value < threshold)
        {
            return;
        }
        for (int sample = 0; sample < additionalSamples; sample++)
        {
            Vector2 otherPosition = getRandomObstaclePos();
            if (obstacleCreationNoise.GetNoise2dv(otherPosition + offset) > value)
            {
                position = otherPosition;
            }
        }
        Node2D obstacle = scene.Instance<Node2D>();
        obstacle.Position = position;
        obstacles.Add(obstacle);
        obstacleContainer.AddChild(obstacle);
    }

    private Vector2 getRandomObstaclePos()
    {
        return new Vector2(
            (int)ProjectSettings.GetSetting("display/window/size/width") + OBSTACLE_PADDING,
            GD.Randf() * (int)ProjectSettings.GetSetting("display/window/size/height")
        );
    }

    public override void _PhysicsProcess(float delta)
    {
        now += delta;
        for (int index = 0; index < obstacles.Count; index++)
        {
            Node2D obstacle = obstacles[index];
            obstacle.Position += Vector2.Left * (obstacleMovementSpeed * delta);
            if (obstacle.Position.x < -OBSTACLE_PADDING)
            {
                obstacle.QueueFree();
                obstacles[index] = obstacles[obstacles.Count - 1];
                obstacles.RemoveAt(obstacles.Count - 1);
                index--;
                GD.Print(obstacles.Count);
            }
        }
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

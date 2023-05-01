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
    private float obstacleFrequency = .8f;

    [Export]
    private float obstacleMovementSpeed = 100f;

    private Node obstacleContainer;

    private NoiseTexture obstacleNoiseTexture;

    [Export]
    private float suncakeFrequency = .5f;

    [Export]
    private PackedScene suncakeScene = null;
    private Node pickupContainer;
    private int suncakeIndex = 0;

    private float now = 0f;

    private float obstacleX;
    private float maxObstacleY;

    private int destroyedObstacles = 0;
    public int DestroyedObstacles => destroyedObstacles;

    private readonly HashSet<int> pickedUpSuncakes = new HashSet<int>();
    public HashSet<int> PickedUpSuncakes => pickedUpSuncakes;

    private Hud hud;

    private float Danger => Mathf.Lerp(Global.StartDanger, Global.TargetDanger, hud.Completion);

    public override void _Ready()
    {
        obstacleContainer = GetNode("Obstacles");
        obstacleNoiseTexture = obstacleContainer.GetNode<Sprite>("ObstacleNoise").Texture as NoiseTexture;
        obstacleCreationNoise = obstacleNoiseTexture.Noise;
        int seed = Global.FlightId.GetHashCode();
        obstacleCreationNoise.Seed = seed;
        pickupContainer = GetNode("Pickups");
        hud = GetNode<Hud>("Hud");
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

    public void _on_SmallObstacleCreationTimer_timeout() => createObject(smallObstacleScene, 1, obstacleContainer, Obstacle.OBSTACLE_PADDING, obstacleFrequency * Danger);
    public void _on_ObstacleCreationTimer_timeout() => createObject(obstacleScene, 2, obstacleContainer, Obstacle.OBSTACLE_PADDING, obstacleFrequency * Danger);
    public void _on_BigObstacleCreationTimer_timeout() => createObject(bigObstacleScene, 8, obstacleContainer, Obstacle.OBSTACLE_PADDING, obstacleFrequency * Danger);
    public void _on_SuncakeTimer_timeout()
    {
        if (!Global.IsSuncakePickedUp(suncakeIndex))
        {
            Suncake suncake = createObject(suncakeScene, 4, pickupContainer, Suncake.SUNCAKE_PADDING, suncakeFrequency, true) as Suncake;
            if (suncake != null)
            {
                suncake.Index = suncakeIndex;
                suncake.Connect(nameof(Suncake.PickedUp), this, nameof(_on_Suncake_pickedUp));
            }
        }
        else
        {
            GD.Print("Skipping creation of suncake!");
        }
        suncakeIndex++;
    }

    public void _on_Suncake_pickedUp(int index)
    {
        pickedUpSuncakes.Add(index);
    }

    private void linkObstacleDeath(Obstacle obstacle)
    {
        obstacle.Connect(nameof(Obstacle.Destroyed), this, nameof(_on_Obstacle_Destroyed));
    }

    public void _on_Obstacle_Destroyed()
    {
        ++destroyedObstacles;
    }

    private ScrollingObject createObject(PackedScene scene, int additionalSamples, Node container, float padding, float frequency, bool reverseSampling = false)
    {
        var noiseRange = getNoiseRange(padding);
        float threshold = (noiseRange.Item1 + (noiseRange.Item2 - noiseRange.Item1) * (1f - frequency));
        Vector2 position = getRandomScrollingObjectPos(padding);
        Vector2 offset = Vector2.Right * now * obstacleMovementSpeed;
        float value = obstacleCreationNoise.GetNoise2dv(position + offset);
        if (reverseSampling ? (value > threshold) : (value < threshold))
        {
            return null;
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
        createdObject.ScrollSpeed = obstacleMovementSpeed * Mathf.Lerp(1f, 2f, Danger);
        createdObject.Position = position;
        if (createdObject is Obstacle)
        {
            (createdObject as Obstacle).TargetViewport = GetNode<Viewport>("Obstacles/Viewport");
            linkObstacleDeath(createdObject as Obstacle);
        }
        container.AddChild(createdObject as Node);
        return createdObject;
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

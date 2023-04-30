using Godot;
using System;

public class Obstacle : Node2D, ScrollingObject
{
    public const float OBSTACLE_PADDING = 64;
    private const float SLOWDOWN_PER_HIT = .1f;

    private Vector2 velocity = Vector2.Zero;
    public float ScrollSpeed { set => velocity = Vector2.Left * value; }

    public Viewport TargetViewport;
    private Sprite targetViewportSprite;

    [Export]
    private int hp = 2;

    public override void _Ready()
    {
        var sprite = GetNode<Sprite>("Sprite");
        targetViewportSprite = sprite.Duplicate() as Sprite;
        targetViewportSprite.Position = Position;
        if (TargetViewport != null)
        {
            TargetViewport.AddChild(targetViewportSprite);
            sprite.Visible = false;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        Position += velocity * delta;
        targetViewportSprite.Position = Position.Round();
        if (Position.x < -OBSTACLE_PADDING)
        {
            QueueFree();
            targetViewportSprite.QueueFree();
        }
    }

    public void TakeHit(Vector2 position)
    {
        velocity += (Position - position).Normalized() * (velocity.Length() * SLOWDOWN_PER_HIT);
        if (--hp <= 0)
        {
            QueueFree();
            targetViewportSprite.QueueFree();
        }
    }
}

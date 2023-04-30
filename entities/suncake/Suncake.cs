using Godot;
using System;

public class Suncake : Node2D, Pickup, ScrollingObject
{
    public const float SUNCAKE_PADDING = 32;

    private Vector2 velocity = Vector2.Zero;
    public float ScrollSpeed { set => velocity = Vector2.Left * value; }

    private int index;
    public int Index { set => index = value; }

    private bool pickable = true;
    public bool Pickable => pickable;

    public override void _PhysicsProcess(float delta)
    {
        Position += velocity * delta;
        if (Position.x < -SUNCAKE_PADDING)
        {
            QueueFree();
        }
    }

    public void Disappear()
    {
        pickable = false;
        Global.PickSuncakeUp(index);
        QueueFree();
    }
}

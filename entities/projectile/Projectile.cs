
using Godot;

public class Projectile : Node2D
{
    private const float PADDING = 64;

    public Vector2 Velocity;

    private readonly float windowWidth = (int)ProjectSettings.GetSetting("display/window/size/width");
    private readonly float windowHeight = (int)ProjectSettings.GetSetting("display/window/size/height");

    protected void Move(float delta)
    {
        Position += Velocity * delta;
        if (Position.x < -PADDING || Position.x > windowWidth + PADDING
            || Position.y < -PADDING || Position.y > windowHeight + PADDING)
        {
            QueueFree();
        }
    }
}

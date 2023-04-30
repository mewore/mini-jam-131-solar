using Godot;
using System;

public class PlayerProjectile : Projectile
{
    private const float RAYCAST_PADDING = 2f;

    private RayCast2D rayCast;

    public override void _Ready()
    {
        rayCast = GetNode<RayCast2D>("RayCast2D");
    }

    public override void _PhysicsProcess(float delta)
    {
        rayCast.CastTo = Velocity * (delta * (RAYCAST_PADDING + 1f));
        rayCast.Position = Velocity * (delta * -RAYCAST_PADDING);
        rayCast.ForceRaycastUpdate();
        if (rayCast.IsColliding())
        {
            var collider = (rayCast.GetCollider() as Area2D).Owner;
            if (collider is Obstacle)
            {
                (collider as Obstacle).TakeHit(rayCast.GetCollisionPoint(), Global.Damage);
            }
            QueueFree();
            return;
        }
        Move(delta);
    }
}

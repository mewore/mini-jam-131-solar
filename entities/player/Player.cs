using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
    [Signal]
    public delegate void HpChanged(int newHp);

    [Signal]
    public delegate void AmmoChanged(int newHp);


    private Vector2 velocity = Vector2.Zero;

    // Movement
    [Export]
    private float acceleration = 1000.0f;

    [Export]
    private float maxSpeed = 100.0f;

    [Export(PropertyHint.Range, "(0,1)")]
    private float sneakSpeed = .5f;

    private int hp = Global.MaxHp;
    public int Hp => hp;

    private Sprite sprite;
    private float now = 0f;
    private float blinkTime = .1f;
    private bool invulnerable = false;
    private int overlappingProjectiles = 0;

    private float lastShotAt = -Mathf.Inf;

    [Export]
    private PackedScene projectileScene = null;

    [Export]
    private NodePath projectileContainer = null;
    private Node projectileContainerNode;

    private int ammoLeft = Global.MaxAmmo;
    private readonly Vector2 bulletVelocity = Vector2.Right * Global.ProjectilSpeed;

    public override void _Ready()
    {
        sprite = GetNode<Sprite>("Sprite");
        projectileContainerNode = projectileContainer != null ? GetNode(projectileContainer) : GetParent();
        if (projectileScene == null)
        {
            lastShotAt = Mathf.Inf;
        }
    }

    public override void _Process(float delta)
    {
        if (invulnerable)
        {
            sprite.Visible = Mathf.Sin(now * Mathf.Pi / blinkTime) > 0f;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        now += delta;
    }

    public void Move(float delta, bool canControl = true)
    {
        Vector2 desiredVelocity = Vector2.Zero;
        if (canControl)
        {
            desiredVelocity = new Vector2(
                Input.GetAxis("move_left", "move_right"),
                Input.GetAxis("move_up", "move_down")
            ) * maxSpeed * (Input.IsActionPressed("sneak") ? sneakSpeed : 1f);
        }

        velocity = velocity.MoveToward(desiredVelocity, acceleration * delta);
        velocity = MoveAndSlide(velocity);
    }

    public void TryShoot()
    {
        if (now <= lastShotAt + Global.ShootCooldown || ammoLeft <= 0)
        {
            return;
        }
        EmitSignal(nameof(AmmoChanged), --ammoLeft);
        lastShotAt = now;
        var bullet = projectileScene.Instance<Projectile>();
        bullet.Velocity = bulletVelocity;
        bullet.Position = Position;
        projectileContainerNode.AddChild(bullet);
    }

    public void _on_HarmArea_area_entered(Area2D area)
    {
        overlappingProjectiles++;
        checkForHit();
    }

    public void _on_HarmArea_area_exited(Area2D area)
    {
        overlappingProjectiles--;
    }

    public void _on_HarmCooldown_timeout()
    {
        invulnerable = false;
        sprite.Visible = true;
        checkForHit();
    }

    public void checkForHit()
    {
        if (invulnerable || overlappingProjectiles == 0)
        {
            return;
        }
        invulnerable = true;
        EmitSignal(nameof(HpChanged), --hp);
        GetNode<Timer>("HarmCooldown").Start();
    }

    public void GetPickup(Pickup pickup)
    {
        pickup.Disappear();
    }
}

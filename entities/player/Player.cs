using Godot;
using System;

public class Player : KinematicBody2D
{
    [Signal]
    public delegate void HpChanged(int newHp);


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

    public override void _Ready()
    {
        EmitSignal(nameof(HpChanged), hp);
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
}

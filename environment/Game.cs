using System;
using Godot;

public class Game : Node2D
{
    [Signal]
    public delegate void GameWon();

    [Export]
    private PackedScene pickupScene = null;

    private Node pickupContainer;

    public override void _Ready()
    {
        // player = GetNode<Player>("Player");

        // pickupContainer = GetNode<Node>("Pickups");
    }

    // public override void _PhysicsProcess(float delta)
    // {
    //     if (hasPlayerWon())
    //     {
    //         if (Global.TryWinLevel(teleports))
    //         {
    //             GetTree().ChangeScene(Global.CurrentLevelPath);
    //         }
    //         else
    //         {
    //             EmitSignal(nameof(GameWon));
    //         }
    //     }
    // }
}

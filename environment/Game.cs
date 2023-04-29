using Godot;

enum Direction { LEFT, RIGHT, UP, DOWN }

public class Game : Node2D
{
    [Signal]
    public delegate void GameWon();

    [Export]
    private PackedScene pickupScene = null;

    [Export]
    private NodePath teleportButton = null;
    private Button teleportButtonNode;

    [Export]
    private NodePath teleportLabel = null;
    private Label teleportLabelNode;

    private Timer resetPositionTimer;

    private Vector2 startPosition;
    private Direction winDirection;
    private Vector2 minPosition;
    private Vector2 maxPosition;

    private int teleports = 0;

    private Node pickupContainer;

    public override void _Ready()
    {
        teleportButtonNode = GetNode<Button>(teleportButton);
        teleportLabelNode = GetNode<Label>(teleportLabel);
        resetPositionTimer = GetNode<Timer>("ResetPositionTimer");
        // player = GetNode<Player>("Player");

        minPosition = GetNode<Node2D>("OffscreenBoundaries/Minimum").GlobalPosition;
        maxPosition = GetNode<Node2D>("OffscreenBoundaries/Maximum").GlobalPosition;

        pickupContainer = GetNode<Node>("Pickups");

        teleportButtonNode.Disabled = true;
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

    public override void _Process(float delta)
    {
        teleportLabelNode.SelfModulate = new Color(teleportLabelNode.SelfModulate, (teleportLabelNode.SelfModulate.a * .8f + .5f * .2f));
    }
}

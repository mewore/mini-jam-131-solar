using Godot;

public class Flight : Node2D
{
    public void _on_DestinationTimer_timeout()
    {
        GetTree().ChangeScene("res://scenes/MainMenu.tscn");
        Global.CurrentLocation = Global.TargetLocation;
    }

    public void _on_AbortButton_pressed()
    {
        GetTree().ChangeScene("res://scenes/MainMenu.tscn");
    }
}

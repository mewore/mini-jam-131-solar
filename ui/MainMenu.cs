using Godot;

public class MainMenu : VBoxContainer
{
    public override void _Ready()
    {
        Global.LoadGameData();
        Global.SaveGameData();
    }
}

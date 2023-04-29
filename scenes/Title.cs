using Godot;

public class Title : Label
{
    public override void _Ready()
    {
        Text = ProjectSettings.GetSetting("application/config/name").ToString().Replace(":", "\n");
    }
}

using Godot;
using System;

public class JourneyInfo : Node2D
{
    private Label label;
    private string labelTemplate;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        labelTemplate = label.Text;
    }

    public void Update(string target, float distance)
    {
        label.Text = labelTemplate
            .Replace("<TARGET>", target)
            .Replace("<DISTANCE>", ((int)distance).ToString());
    }
}

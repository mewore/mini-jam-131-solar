using Godot;
using System;

public class JourneyInfo : Node2D
{
    private Label label;
    private string labelTemplate;

    public Color TextColor { set => label.SelfModulate = value; }

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        labelTemplate = label.Text;
    }

    public void Update(string target, float distance, float danger)
    {
        label.Text = labelTemplate
            .Replace("<TARGET>", target)
            .Replace("<DISTANCE>", ((int)distance).ToString())
            .Replace("<DANGER>", getDangerLabel(danger));
    }

    private static string getDangerLabel(float danger)
    {
        if (danger > .8f) return "DEATH";
        if (danger > .6f) return "EXTREME";
        if (danger > .4f) return "High";
        if (danger > .2f) return "Medium";
        return "Low";
    }
}

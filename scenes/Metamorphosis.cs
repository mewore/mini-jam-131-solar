using Godot;
using System;

public class Metamorphosis : Control
{
    private string xpEarnedTemplate;
    private Label xpEarnedLabel;

    private string upgradeQuestionTemplate;
    private Label upgradeQuestionLabel;

    public override void _Ready()
    {
        var eatSuncakeButton = GetNode<Button>("EatSuncakeButton");
        if (Global.SuncakeEaten)
        {
            eatSuncakeButton.Disabled = true;
            eatSuncakeButton.Text = "Suncake eaten... delicious (x2 earned XP)";
        }
        else if (Global.Suncakes <= 0)
        {
            eatSuncakeButton.Disabled = true;
            eatSuncakeButton.Text = "No suncakes to eat";
        }
        else if (Global.EarnedExperience <= 0)
        {
            eatSuncakeButton.Disabled = true;
            eatSuncakeButton.Text = "Eating suncakes would be a waste...";
        }
        xpEarnedLabel = GetNode<Label>("XpEarnedLabel");
        xpEarnedTemplate = xpEarnedLabel.Text;
        upgradeQuestionLabel = GetNode<Label>("UpgradeQuestion");
        upgradeQuestionTemplate = upgradeQuestionLabel.Text;
        UpdateXpText();
        switch (Global.FlightResult)
        {
            case FlightResult.SUCCEEDED: GetNode<CanvasItem>("SuccessLabel").Visible = true; break;
            case FlightResult.FAILED: GetNode<CanvasItem>("FailureLabel").Visible = true; break;
            case FlightResult.ABORTED: GetNode<CanvasItem>("AbortLabel").Visible = true; break;
        }
    }

    public void _on_EatSuncakeButton_pressed()
    {
        Global.Experience += Global.EarnedExperience;
        Global.EarnedExperience *= 2;
        GetNode<Button>("EatSuncakeButton").Disabled = Global.SuncakeEaten = true;
        UpdateXpText();
    }

    private void UpdateXpText()
    {
        xpEarnedLabel.Text = xpEarnedTemplate.Replace("<XP_EARNED>", Global.EarnedExperience.ToString());
        GD.Print(xpEarnedLabel.Text);
        upgradeQuestionLabel.Text = upgradeQuestionTemplate.Replace("<EXPERIENCE>", Global.Experience.ToString());
    }

    public void _on_GoToMapButton_pressed()
    {
        GetTree().ChangeScene("res://scenes/MainMenu.tscn");
    }
}

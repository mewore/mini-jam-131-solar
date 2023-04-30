using Godot;
using System;
using System.Collections.Generic;

public class Metamorphosis : Control
{
    private string xpEarnedTemplate;
    private Label xpEarnedLabel;

    private string upgradeQuestionTemplate;
    private Label upgradeQuestionLabel;

    private readonly List<(Button, SkillState)> upgradeButtons = new List<(Button, SkillState)>();
    private string upgradeInfoTemplate;
    private Label upgradeInfoLabel;

    [Export]
    private float upgradeInfoFadeSpeed = .2f;

    private Color targetUpgradeInfoColor = new Color(1f, 1f, 1f, 0f);

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

        upgradeInfoLabel = GetNode<Label>("UpgradeInfo");
        upgradeInfoTemplate = upgradeInfoLabel.Text;
        upgradeInfoLabel.SelfModulate = targetUpgradeInfoColor;
        foreach (Button upgradeButton in GetNode("Upgrades").GetChildren())
        {
            SkillState skill = getSkill(upgradeButton);
            upgradeButton.Connect("pressed", this, nameof(_on_UpgradeButton_pressed), new Godot.Collections.Array(new object[] { upgradeButtons.Count }));
            upgradeButtons.Add((upgradeButton, skill));
        }
        refreshUpgradeButtons();
        refreshUpgradeInfo(GetGlobalMousePosition());
    }

    public void _on_UpgradeButton_pressed(int index)
    {
        SkillState skill = upgradeButtons[index].Item2;
        Global.Experience -= skill.NextCost;
        skill.Upgrade();
        UpdateXpText();
        refreshUpgradeButtons();
        refreshUpgradeInfo(GetGlobalMousePosition());
    }

    public override void _Process(float delta)
    {
        refreshUpgradeInfo(GetGlobalMousePosition());
        upgradeInfoLabel.SelfModulate = upgradeInfoLabel.SelfModulate.LinearInterpolate(targetUpgradeInfoColor, upgradeInfoFadeSpeed);
    }

    private void refreshUpgradeButtons()
    {
        foreach (Button upgradeButton in GetNode("Upgrades").GetChildren())
        {
            upgradeButton.Disabled = !isUpgradeAvailable(upgradeButton);
        }
    }

    private void refreshUpgradeInfo(Vector2 mousePos)
    {
        foreach ((Button, SkillState) upgrade in upgradeButtons)
        {
            Vector2 rectPos = upgrade.Item1.RectGlobalPosition;
            Vector2 rectSize = upgrade.Item1.RectSize;
            if (new Rect2(rectPos, rectSize).HasPoint(mousePos))
            {
                SkillState skill = upgrade.Item2;
                string warning = "";
                targetUpgradeInfoColor = new Color(1f, 1f, 1f);
                if (!skill.CanUpgrade)
                {
                    warning = "Already at MAX!";
                    upgradeInfoLabel.Text = skill.Format(skill.Value) + "\n" + warning;
                    targetUpgradeInfoColor = new Color(.5f, .5f, .5f, .5f);
                }
                else
                {
                    if (skill.NextCost > Global.Experience)
                    {
                        warning = "Not enough experience!";
                        targetUpgradeInfoColor = new Color(1f, .3f, .2f);
                    }
                    upgradeInfoLabel.Text = upgradeInfoTemplate
                        .Replace("<OLD>", skill.Format(skill.Value))
                        .Replace("<NEW>", skill.Format(skill.NextValue))
                        .Replace("<COST>", skill.NextCost.ToString())
                        .Replace("<WARNING>", warning)
                    ;
                }
                if (upgradeInfoLabel.SelfModulate.a < .01f)
                {
                    upgradeInfoLabel.SelfModulate = new Color(targetUpgradeInfoColor, upgradeInfoLabel.SelfModulate.a);
                }
                return;
            }
        }
        targetUpgradeInfoColor = new Color(targetUpgradeInfoColor, 0f);
    }

    private bool isUpgradeAvailable(Button button)
    {
        SkillState skill = getSkill(button);
        return skill == null ? false : (skill.CanUpgrade && Global.Experience >= skill.NextCost);
    }

    private SkillState getSkill(Button button)
    {
        switch (button.Name)
        {
            case "FireRate": return Global.FireRateSkill;
            default:
                GD.PushWarning("Unrecognized upgrade button name: " + button.Name);
                return null;
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

using Godot;
using System;
using System.Collections.Generic;

public class Hud : CanvasLayer
{
    private Label timerLabel;
    private Timer destinationTimer;
    public float Completion => 1f - destinationTimer.TimeLeft / destinationTimer.WaitTime;
    private string timerTextTemplate;
    private string flightId = Global.FlightId;

    private Line2D timerLine;

    private Label ammoLabel;
    private Label hpLabel;

    private float defaultLabelOpacity;

    [Export]
    private Color warningColor;

    [Export]
    private Color badColor;

    [Export]
    private float opacityConvergence = .25f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        (destinationTimer = GetNode<Timer>("DestinationTimer")).Start(Global.DistanceToTarget);
        timerLabel = GetNode<Label>("TimerLabel");
        timerTextTemplate = timerLabel.Text;
        timerLine = GetNode<Line2D>("TimerOutline/TimerInner");
        updateTimerLine();
        updateTimerText();

        hpLabel = GetNode<Label>("HpLabel");
        hpLabel.Text = getHpText(Global.MaxHp);
        ammoLabel = GetNode<Label>("AmmoLabel");
        ammoLabel.Text = String.Format("Ammo: {0}", Global.MaxAmmo);
        defaultLabelOpacity = hpLabel.Modulate.a;
    }

    public override void _Process(float delta)
    {
        updateTimerLine();
        updateOpacity(hpLabel);
        updateOpacity(ammoLabel);
    }

    public int GetEarnedExperience()
    {
        return Mathf.RoundToInt(destinationTimer.WaitTime - destinationTimer.TimeLeft);
    }

    private void updateOpacity(CanvasItem label)
    {
        if (label.Modulate.a > defaultLabelOpacity)
        {
            label.Modulate = new Color(label.Modulate,
                label.Modulate.a * (1f - opacityConvergence) + defaultLabelOpacity * opacityConvergence);
        }
    }

    private void updateTimerLine()
    {
        timerLine.Scale = new Vector2(Completion, timerLine.Scale.y);
    }

    private void updateTimerText()
    {
        timerLabel.Text = timerTextTemplate
            .Replace("<FLIGHT_ID>", flightId)
            .Replace("<TIME_LEFT>", Mathf.RoundToInt(destinationTimer.TimeLeft).ToString());
    }

    public void _on_RefreshTimer_timeout() => updateTimerText();

    public void _on_Player_HpChanged(int newHp)
    {
        hpLabel.Text = getHpText(newHp);
        hpLabel.Modulate = new Color(hpLabel.Modulate, 1f);

        if (newHp <= 1)
        {
            hpLabel.SelfModulate = badColor;
        }
        else if (newHp <= 2)
        {
            hpLabel.SelfModulate = warningColor;
        }
    }

    private string getHpText(int hp)
    {
        List<string> hpParts = new List<string>(Global.MaxHp);
        for (int index = 0; index < hp; index++)
        {
            hpParts.Add("|||");
        }
        for (int index = hp; index < Global.MaxHp; index++)
        {
            hpParts.Add("...");
        }
        return String.Format("HP: {0}", String.Join(" ", hpParts));
    }

    public void _on_Player_AmmoChanged(int newAmmo)
    {
        ammoLabel.Text = String.Format("Ammo: {0}", newAmmo);
        ammoLabel.Modulate = new Color(ammoLabel.Modulate, 1f);
        if (newAmmo < 10)
        {
            ammoLabel.SelfModulate = badColor;
        }
        else if (newAmmo < 20)
        {
            ammoLabel.SelfModulate = warningColor;
        }
    }
}

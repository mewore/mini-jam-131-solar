using Godot;
using System;

public class Hud : CanvasLayer
{
    private Label timerLabel;
    private Timer destinationTimer;
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
        hpLabel.Text = String.Format("HP: {0}/{1}", Global.MaxHp, Global.MaxHp);
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
        timerLine.Scale = new Vector2(1f - destinationTimer.TimeLeft / destinationTimer.WaitTime, timerLine.Scale.y);
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
        hpLabel.Text = String.Format("HP: {0}/{1}", newHp, Global.MaxHp);
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

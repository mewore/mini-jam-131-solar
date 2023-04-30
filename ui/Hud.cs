using Godot;
using System;

public class Hud : CanvasLayer
{
    private Label timerLabel;
    private Timer destinationTimer;
    private string timerTextTemplate;
    private string flightId = Global.FlightId;

    private Line2D timerLine;

    private Label hpLabel;

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
    }

    public override void _Process(float delta)
    {
        updateTimerLine();
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
    }
}

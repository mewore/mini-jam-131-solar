using Godot;
using System;

public class Hud : CanvasLayer
{
    private Label timerLabel;
    private Timer destinationTimer;
    private string timerTextTemplate;

    private Line2D timerLine;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        (destinationTimer = GetNode<Timer>("DestinationTimer")).Start(Global.DistanceToTarget);
        timerLabel = GetNode<Label>("TimerLabel");
        timerTextTemplate = timerLabel.Text;
        timerLine = GetNode<Line2D>("TimerOutline/TimerInner");
        updateTimerLine();
        updateTimerText();
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
        timerLabel.Text = timerTextTemplate.Replace("<TIME_LEFT>", Mathf.RoundToInt(destinationTimer.TimeLeft).ToString());
    }

    public void _on_RefreshTimer_timeout() => updateTimerText();
}

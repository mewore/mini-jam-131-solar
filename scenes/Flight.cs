using Godot;

public class Flight : Node2D
{
    private const int DEATH_EXPERIENCE_MULTIPLIER = 5;

    public void _on_DestinationTimer_timeout()
    {
        Global.CurrentLocation = Global.TargetLocation;
        endFlight(FlightResult.SUCCEEDED, 1);
    }

    public void _on_AbortButton_pressed()
    {
        endFlight(FlightResult.ABORTED, 0);
    }

    public void _on_Player_HpChanged(int newHp)
    {
        if (newHp <= 0)
        {
            endFlight(FlightResult.FAILED, DEATH_EXPERIENCE_MULTIPLIER);
        }
    }

    private void endFlight(FlightResult flightResult, int xpMultiplier)
    {
        Global.FlightResult = flightResult;
        var game = GetNode<Game>("Game");
        Global.EarnedExperience = (GetNode<Hud>("Game/Hud").GetEarnedExperience() + game.DestroyedObstacles) * xpMultiplier;
        Global.Experience += Global.EarnedExperience;
        if (flightResult != FlightResult.ABORTED)
        {
            Global.PickSuncakesUp(game.PickedUpSuncakes);
        }
        if (flightResult == FlightResult.SUCCEEDED)
        {
            Global.PastLocations.Add(Global.CurrentLocation);
            Global.CurrentLocation = Global.TargetLocation;
        }
        GetTree().ChangeScene("res://scenes/Metamorphosis.tscn");
    }
}

using System;
using System.Collections.Generic;
using Godot;

public class MainMenu : VBoxContainer
{
    private const string CURRENT_LOCATION = "Earth";
    private const int SELECTION_DISTANCE = 24;
    private static readonly int SELECTION_DISTANCE_SQUARED = SELECTION_DISTANCE * SELECTION_DISTANCE;

    private Node2D currentBody = null;
    private Line2D flightLine;
    private readonly List<Node2D> bodies = new List<Node2D>();
    private Node2D targetBody = null;

    public override void _Ready()
    {
        Global.LoadGameData();
        Global.SaveGameData();

        flightLine = GetNode<Line2D>("CelestialBodies/FlightLine");
        foreach (Node child in GetNode("CelestialBodies").GetChildren())
        {
            if (child is Node2D && child != flightLine)
            {
                bodies.Add(child as Node2D);
            }
        }

        foreach (Node2D body in bodies)
        {
            if (body.Name == CURRENT_LOCATION)
            {
                currentBody = body;
                break;
            }
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouse)
        {
            Node2D oldTargetBody = targetBody;
            targetBody = getTargetBody((@event as InputEventMouse).Position);
            if (targetBody != oldTargetBody)
            {
                flightLine.Visible = targetBody != null;
                MouseDefaultCursorShape = targetBody != null ? CursorShape.PointingHand : CursorShape.Arrow;
                if (targetBody != null)
                {
                    flightLine.Points = new Vector2[] { currentBody.Position, targetBody.Position };
                }
            }
            if (@event.IsActionPressed("ui_navigate") && targetBody != null)
            {
                GD.Print(String.Format("Navigating from {0} to {1} (distance: {2:F0})",
                    currentBody.Name, targetBody.Name,
                    currentBody.Position.DistanceTo(targetBody.Position)));
                GetTree().ChangeScene("res://scenes/Flight.tscn");
            }
        }
    }

    private Node2D getTargetBody(Vector2 position)
    {
        float bestDistanceSquared = SELECTION_DISTANCE_SQUARED;
        Node2D result = null;
        foreach (Node2D body in bodies)
        {
            if (body != currentBody)
            {
                float distanceSquared = body.Position.DistanceSquaredTo(position);
                if (distanceSquared < bestDistanceSquared)
                {
                    result = body;
                    bestDistanceSquared = distanceSquared;
                }
            }
        }
        return result;
    }
}

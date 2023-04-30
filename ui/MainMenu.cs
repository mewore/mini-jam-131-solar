using System;
using System.Collections.Generic;
using Godot;

public class MainMenu : VBoxContainer
{
    private const int SELECTION_DISTANCE = 24;
    private static readonly int SELECTION_DISTANCE_SQUARED = SELECTION_DISTANCE * SELECTION_DISTANCE;

    [Export(PropertyHint.ExpEasing)]
    private float targetDisplayEase = .5f;

    [Export]
    private float targetDisplayTime = 2f;

    private float startedDisplayingTargetAt = 0f;

    private JourneyInfo journeyInfo;

    private float now = 0f;

    private Node2D currentBody = null;
    private Line2D flightLine;
    private readonly Dictionary<string, Node2D> bodies = new Dictionary<string, Node2D>();
    private Node2D targetBody = null;

    public override void _Ready()
    {
        Global.LoadGameData();
        Global.SaveGameData();

        var flightLineContainer = GetNode("FlightLines");
        flightLine = flightLineContainer.GetNode<Line2D>("FlightLine");
        foreach (Node2D body in GetNode("CelestialBodies").GetChildren())
        {
            bodies[body.Name] = body;
        }
        currentBody = bodies.ContainsKey(Global.CurrentLocation) ? bodies[Global.CurrentLocation] : null;
        Node2D startingBody = GetNode("CelestialBodies").GetChild(0) as Node2D;
        if (currentBody == null)
        {
            GD.PushError("There's no celestial body with name '" + Global.CurrentLocation + "'");
            currentBody = startingBody;
            Global.CurrentLocation = currentBody.Name;
        }
        GetNode<Node2D>("YouAreHere").Position = currentBody.Position;

        Node2D lastBody = startingBody;
        Stack<Line2D> pastLineStack = new Stack<Line2D>(Global.PastLocations.Count);
        foreach (string bodyName in Global.PastLocations)
        {
            Node2D newBody = bodies.ContainsKey(bodyName) ? bodies[bodyName] : null;
            if (newBody != null)
            {
                if (lastBody != null)
                {
                    var line = new Line2D();
                    line.Points = new Vector2[] { lastBody.Position, newBody.Position };
                    line.DefaultColor = flightLine.DefaultColor;
                    line.Width = flightLine.Width / 2;
                    pastLineStack.Push(line);
                }
                lastBody = newBody;
            }
        }
        if (lastBody != null)
        {
            var line = new Line2D();
            line.Points = new Vector2[] { lastBody.Position, currentBody.Position };
            line.DefaultColor = flightLine.DefaultColor;
            line.Width = flightLine.Width / 2;
            pastLineStack.Push(line);
        }
        float minOpacity = .2f;
        float opacityDecrement = (1f - minOpacity) / pastLineStack.Count;
        float opacity = 1f;
        float minWidth = 1f;
        float widthDecrement = (flightLine.Width - minWidth) / pastLineStack.Count;
        float width = flightLine.Width;
        while (pastLineStack.Count > 0)
        {
            Line2D line = pastLineStack.Pop();
            line.SelfModulate = new Color(line.SelfModulate, opacity -= opacityDecrement);
            line.Width = width -= widthDecrement;
            flightLineContainer.AddChild(line);
        }

        GetNode<Button>("Options/MetamorphosisButton").Visible = Global.Experience > 0;

        journeyInfo = GetNode<JourneyInfo>("JourneyInfo");
    }

    public override void _Process(float delta)
    {
        now += delta;
        if (flightLine.Visible = journeyInfo.Visible = targetBody != null)
        {
            updateTargetInfo();
        }
    }

    private void updateTargetInfo()
    {
        float amount = Mathf.Ease(Mathf.Min((now - startedDisplayingTargetAt) / targetDisplayTime, 1f), targetDisplayEase);
        Vector2 position = currentBody.Position.LinearInterpolate(targetBody.Position, amount);
        flightLine.Points = new Vector2[] { currentBody.Position, position };
        journeyInfo.Position = (currentBody.Position + position) * .5f;
        journeyInfo.Update(targetBody.Name, currentBody.Position.DistanceTo(position));
        journeyInfo.Modulate = new Color(journeyInfo.Modulate, amount);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouse)
        {
            Node2D oldTargetBody = targetBody;
            targetBody = getTargetBody((@event as InputEventMouse).Position);
            if (targetBody != oldTargetBody)
            {
                MouseDefaultCursorShape = targetBody != null ? CursorShape.PointingHand : CursorShape.Arrow;
                if (targetBody != null)
                {
                    startedDisplayingTargetAt = now;
                }
            }
            if (@event.IsActionPressed("ui_navigate") && targetBody != null)
            {
                // TODO: Autosave
                Global.TargetLocation = targetBody.Name;
                Global.DistanceToTarget = currentBody.Position.DistanceTo(targetBody.Position);
                Global.EarnedExperience = 0;
                Global.SuncakeEaten = false;
                Global.FlightResult = FlightResult.ABORTED;
                GetTree().ChangeScene("res://scenes/Flight.tscn");
            }
        }
    }

    private Node2D getTargetBody(Vector2 position)
    {
        float bestDistanceSquared = SELECTION_DISTANCE_SQUARED;
        Node2D result = null;
        foreach (Node2D body in bodies.Values)
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

    public void _on_MetamorphosisButton_pressed()
    {
        GetTree().ChangeScene("res://scenes/Metamorphosis.tscn");
    }
}

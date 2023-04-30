using Godot;

public partial class PlayerActive : PlayerState
{
    public override void PhysicsProcess(float delta)
    {
        player.Move(delta);
    }

    public void _on_PickupArea_area_entered(Area2D pickupArea)
    {
        player.GetPickup(pickupArea.Owner as Pickup);
    }
}

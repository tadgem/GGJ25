using Godot;
using System;

public partial class LockedDoor : Node3D
{
	[Export]
	public BubbleSwitch Switch;

	private StaticBody3D _door;
	// Called when the node enters the scene tree for the first time.

    private void OnSwitchToggled(string tag, bool active)
    {
		if(active)
		{
			Disable();
		}
		else
		{
			Enable();
		}
    }


	public void Enable()
	{
		GD.Print("Enabling Door");
		_door.SetProcess(true);
		_door.SetPhysicsProcess(true);
		_door.SetCollisionLayerValue(1, true);
		_door.Show();
	}

	public void Disable()
	{
		GD.Print("Disabling Door");
		_door.SetProcess(false);
		_door.SetPhysicsProcess(false);
		_door.SetCollisionLayerValue(1, false);
		_door.Hide();
	}


	public override void _Ready()
	{
		_door = GetNode<StaticBody3D>("DoorRoot");

		if(Switch == null)
		{
			GD.PrintErr("Door has no switch to activate!");
		}

		Switch.OnSwitchToggled += OnSwitchToggled;
	}

}

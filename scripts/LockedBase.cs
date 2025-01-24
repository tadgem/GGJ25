using Godot;
using System;

public partial class LockedBase : Node3D
{
	[Export]
	public BubbleSwitch Switch;

    private void OnSwitchToggled(string tag, bool active)
    {
		if(active)
		{
			OnDisable();
		}
		else
		{
			OnEnable();
		}
    }


	public virtual void OnEnable()
	{
		GD.Print("LockedBase : Enabling Door");
		// _door.SetProcess(true);
		// _door.SetPhysicsProcess(true);
		// _door.SetCollisionLayerValue(1, true);
		// _door.Show();
	}

	public virtual void OnDisable()
	{
		GD.Print("LockedBase : Disabling Door");
		// _door.SetProcess(false);
		// _door.SetPhysicsProcess(false);
		// _door.SetCollisionLayerValue(1, false);
		// _door.Hide();
	}


	public override void _Ready()
	{
		if(Switch == null)
		{
			GD.PrintErr("Door has no switch to activate!");
		}

		Switch.OnSwitchToggled += OnSwitchToggled;
	}
}

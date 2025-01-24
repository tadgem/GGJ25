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
	}

	public virtual void OnDisable()
	{
		GD.Print("LockedBase : Disabling Door");
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

using Godot;
using System;

public partial class LockedDoor : LockedBase
{
	
	private StaticBody3D _door;
	
	public override void OnEnable()
	{
		base.OnEnable();
		GD.Print("LockedDoor : OnEnable");
		_door.SetProcess(true);
		_door.SetPhysicsProcess(true);
		_door.SetCollisionLayerValue(1, true);
		_door.Show();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		GD.Print("LockedDoor : OnDisable");
		_door.SetProcess(false);
		_door.SetPhysicsProcess(false);
		_door.SetCollisionLayerValue(1, false);
		_door.Hide();
	}


	public override void _Ready()
	{
		base._Ready();
		GD.Print("LockedDoor : _Ready");
		_door = GetNode<StaticBody3D>("DoorRoot");
	}

}

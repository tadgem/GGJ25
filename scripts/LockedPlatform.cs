using Godot;
using System;

public partial class LockedPlatform : LockedBase
{
	private Node3D _hideNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		_hideNode = GetNode<Node3D>("HideNode");
	}
	public override void OnEnable()
	{
		base.OnEnable();
		GD.Print("LockedPlatform : OnEnable");
		_hideNode.SetProcess(true);
		_hideNode.SetPhysicsProcess(true);
		_hideNode.Show();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		GD.Print("LockedPlatform : OnDisable");
		_hideNode.SetProcess(false);
		_hideNode.SetPhysicsProcess(false);
		_hideNode.Hide();
	}
}

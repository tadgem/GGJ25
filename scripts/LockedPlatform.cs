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
		_hideNode.Hide();
		RecurseTogglePhysicsNodes(this, false);
	}

	private void RecurseTogglePhysicsNodes(Node n, bool active)
	{
		var children = n.GetChildren();

		foreach(Node child in children)
		{
			if(child is PhysicsBody3D body)
			{
				GD.Print($"Disabling collision for node : {n.Name}");
				body.SetCollisionLayerValue(1, active);
			}
			RecurseTogglePhysicsNodes(child, active);
		}
	}

	private void EnableChildColliders()
	{
		RecurseTogglePhysicsNodes(this, true);
	}

	private void DisableChildColliders()
	{
		RecurseTogglePhysicsNodes(this, false);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		GD.Print("LockedPlatform : OnEnable");
		_hideNode.SetProcess(false);
		_hideNode.Hide();
		DisableChildColliders();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		GD.Print("LockedPlatform : OnDisable");
		_hideNode.SetProcess(true);
		_hideNode.Show();
		EnableChildColliders();
	}
}

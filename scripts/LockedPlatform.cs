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
		RecurseTogglePhysicsNodesAndBubbleVisibility(this, false);
	}

	private void RecurseTogglePhysicsNodesAndBubbleVisibility(Node n, bool active)
	{
		var children = n.GetChildren();

		foreach(Node child in children)
		{
			if(child is PhysicsBody3D body)
			{
				GD.Print($"Setting collision for node : {n.Name} to {active}");
				body.SetCollisionLayerValue(1, active);
				body.SetProcess(active);
				body.SetPhysicsProcess(active);
			}

			if(child is Bubble b)
			{
				b.Enable(active);
			}
			
			RecurseTogglePhysicsNodesAndBubbleVisibility(child, active);
		}
	}

	private void EnableChildColliders()
	{
		RecurseTogglePhysicsNodesAndBubbleVisibility(this, true);
	}

	private void DisableChildColliders()
	{
		RecurseTogglePhysicsNodesAndBubbleVisibility(this, false);
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

using Godot;
using System;

public partial class MouseController : RigidBody3D
{
	AnimationTree _animTree;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animTree = GetNode<AnimationTree>("Animated_Mouse_Root/AnimationTree");
		GD.Print("Allo");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

using Godot;
using System;

public partial class SlowLerp : Node3D
{
	[Export]
	public Node3D Target;
	
	[Export]
	public float Speed = 1.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = Position.Lerp(Target.Position, (float)delta * Speed);
		//Rotation = Rotation.Lerp(Target.Rotation, (float) delta * Speed);
	}
}

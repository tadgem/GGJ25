using Godot;
using System;

public partial class SlowLerpCam : Camera3D
{

	[Export]
	public Node3D Target;

	[Export]
	public float LerpSpeed = 2.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition, (float) delta * LerpSpeed);
		// GlobalRotation = GlobalRotation.Lerp(Target.GlobalRotation, (float) delta * LerpSpeed);
		GlobalTransform = GlobalTransform.InterpolateWith(Target.GlobalTransform, (float) delta * LerpSpeed);
	}
}

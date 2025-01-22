using Godot;
using System;

public partial class FollowPath : PathFollow3D
{
	[Export]
	public float FollowSpeed = 2.0f;

	public override void _PhysicsProcess(double delta)
	{
		Progress += (float) delta * FollowSpeed;

		if(ProgressRatio > 1.0f)
		{
			ProgressRatio -= 1.0f;
		}
	}
}

using Godot;
using System;

public partial class Bubble : Area3D
{
	[Export]
	public float BounceStrength = 1.0f;

	[Export]
	public bool Bounce = true;

	[Export]
	public bool Solid = true;

	private void OnAreaEntered(Node3D body)
	{
		if (body is MouseCharacter player)
		{
			if(Bounce)
			{
				player.Bounce(BounceStrength);
			}
		}
	}
	private void OnAreaExited(Node3D body)
	{
		if (body is MouseCharacter player)
		{
			
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnAreaEntered;
		BodyExited 	+= OnAreaExited;

		if(!Solid)
		{
			StaticBody3D s = GetNode<StaticBody3D>("StaticBody3D");
			s.QueueFree();
		}
		
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

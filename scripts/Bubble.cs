using Godot;
using System;

public partial class Bubble : CharacterBody3D
{
	[Export]
	public float BounceStrength = 1.0f;

	[Export]
	public bool Bounce = true;

	[Export]
	public bool Solid = true;

	[Export]
	public bool IsTimed = false;

	[Export]
	public float Lifetime = 5.0f;
	private float _timeAlive = 0.0f;


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
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += OnAreaEntered;
		area.BodyExited += OnAreaExited;

		if(!Solid)
		{
			SetCollisionLayerValue(1, false);
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		Vector3 pos = GlobalPosition;
		_timeAlive += (float) delta;

		if(IsTimed && _timeAlive > Lifetime)
		{
			QueueFree();
		}
    }


}

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

	private Vector3 _velocity;
	private Vector3 _lastGlobalPosition;

	private MouseCharacter _player = null;

	private bool MaintainVelocityForPlayer
	{
		get => _player != null;
	}

	private void OnAreaEntered(Node3D body)
	{
		if (body is MouseCharacter player)
		{
			if(Bounce)
			{
				player.Bounce(BounceStrength);
			}
			else
			{
				_player = player;
			}
		}
	}
	private void OnAreaExited(Node3D body)
	{
		if (body is MouseCharacter player)
		{
			_player = null;
			
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnAreaEntered;
		BodyExited 	+= OnAreaExited;

		if(!Solid)
		{
			CharacterBody3D c = GetNode<CharacterBody3D>("CharacterBody3D");
			c.QueueFree();
		}
		_lastGlobalPosition = Vector3.Zero;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		Vector3 pos = GlobalPosition;
		_velocity = pos - _lastGlobalPosition;
		_lastGlobalPosition = pos;

		if(MaintainVelocityForPlayer)
		{
			_player.VelocityOffset = _velocity * 50.0f;
		}


    }


}

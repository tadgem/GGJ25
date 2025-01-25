using Godot;
using System;

public partial class GiveBubblesTrigger : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

    private void OnBodyEntered(Node3D body)
    {
		if(body is MouseCharacter mouse)
		{
			mouse.GiveBubbles();
			QueueFree();
		}
    }
}

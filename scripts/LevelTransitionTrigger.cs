using Godot;
using System;

public partial class LevelTransitionTrigger : Area3D
{	
	[Export]
	public string LevelToMoveTo;

	private LooneyTransition _transition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Calling LevelTransitionTriggerReady");
		_transition = GetNode<LooneyTransition>("LooneyTransition");

		BodyEntered += OnAreaEntered;
		
	}

    private void OnAreaEntered(Node3D other)
    {
		GD.Print("Something Entered LevelTransitionTrigger");
		if(other is MouseCharacter _)
		{
			GD.Print("Mouse Entered level transition");
			_transition.Show();
			_transition.PlayOutTransition();
			_transition.OnTransitionFinished += ChangeToNewLevel;
		}
    }

    private void ChangeToNewLevel(StringName animName)
    {
		var error = GetTree().ChangeSceneToFile(LevelToMoveTo);
		if(error != Error.Ok)
		{
			GD.Print($"OnLooneyFinished : Failed to change scene to packed : {error}");
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Process(double delta)
	{
	}
}

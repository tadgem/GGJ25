using Godot;
using System;

public partial class ResetBox : Area3D
{
	[Export]
	public Vector3 ResetLocation;

	private LooneyTransition _transition;

	private MouseCharacter _mouse =  null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		_transition = GetNode<LooneyTransition>("LooneyTransition");
		_transition.Hide();
	}

    private void OnBodyEntered(Node3D body)
    {
		if(body is MouseCharacter mouse)
		{
			_transition.Show();
			_transition.OnTransitionFinished += OnFadedOut;
			_mouse = mouse;
			_transition.PlayOutTransition();
		}
    }

    private void OnFadedOut(StringName animName)
    {
		_transition.OnTransitionFinished -= OnFadedOut;
		_mouse.Position = ResetLocation;
		_mouse.Rotation = Vector3.Zero;
		_transition.PlayInTransition();
		_transition.OnTransitionFinished += OnFadedIn;
    }

    private void OnFadedIn(StringName animName)
    {
		_transition.Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.


    public override void _Process(double delta)
	{
	}
}

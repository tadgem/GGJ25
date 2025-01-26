using Godot;
using System;

public partial class ResetBox : Area3D
{

	private LooneyTransition _inTransition;
	private LooneyTransition _outTransition;

	private Node3D _respawn;

	private MouseCharacter _mouse =  null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		_respawn = GetNode<Node3D>("RespawnLocation");
		_inTransition = GetNode<LooneyTransition>("InTransition");
		_outTransition = GetNode<LooneyTransition>("OutTransition");
		_inTransition.Hide();
		_outTransition.Hide();

		_outTransition.OnTransitionFinished += OnFadedOut;
		_inTransition.OnTransitionFinished += OnFadedIn;
	}

    private void OnBodyEntered(Node3D body)
    {
		if(body is MouseCharacter mouse)
		{
			GD.Print("Mouse Entered a ResetBox");
			_outTransition.Show();
			_mouse = mouse;
			_outTransition.PlayOutTransition();
		}
    }

    private void OnFadedOut(StringName animName)
    {
		GD.Print("ResetBox : Faded out");
		_mouse.GlobalTransform = _respawn.GlobalTransform;
		_inTransition.Show();
		_inTransition.PlayInTransition();
		_outTransition.Hide();
    }

    private void OnFadedIn(StringName animName)
    {
		GD.Print("ResetBox : Faded in");
		_inTransition.Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.


    public override void _Process(double delta)
	{
	}
}

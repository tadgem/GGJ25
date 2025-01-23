using Godot;
using System;
using static Godot.AnimationMixer;

public partial class LooneyTransition : Control
{

	[Export]
	public bool TransitionIn = true;

	[Export]
	public bool DestroyOnFinish = false;


	[Export]
	public bool Autoplay = false;
	private StyleBoxFlat _styleBox;
	private AnimationPlayer _anim;

	public event AnimationFinishedEventHandler OnTransitionFinished;

    private void AnimationFinishedShim(StringName animName)
    {
		OnTransitionFinished?.Invoke(animName);

		if(DestroyOnFinish)
		{
			QueueFree();
		}
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Control circleNode = GetNode<Control>("Circle");
		circleNode.Scale = new Vector2(20.0f, 20.0f);
		_styleBox = (StyleBoxFlat) circleNode.GetThemeStylebox("Panel");
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_anim.Stop();
		_anim.AnimationFinished += AnimationFinishedShim;

		GD.Print($"Found Style Box? : {_styleBox != null}");
		if(!Autoplay)
		{
			return;
		}
		if(TransitionIn)
		{
			PlayInTransition();
		}
		else
		{
			PlayOutTransition();
		}
	}

    private bool IsPlayingTransition()
	{
		return _anim.IsPlaying();
	}

	internal void PlayInTransition()
	{
		_anim.Play("looney_in");
	}

	internal void PlayOutTransition()
	{
		_anim.Play("looney_out");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

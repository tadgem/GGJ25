using Godot;
using System;
using System.Net.Http.Headers;

public partial class GiveBubblesTrigger : Area3D
{

	[Export]
	public AudioStream OnGiveBubblesAudio;

	private AudioStreamPlayer _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		_player = new AudioStreamPlayer();
		AddChild(_player);
	}

    private void OnBodyEntered(Node3D body)
    {
		if(body is MouseCharacter mouse)
		{
			mouse.GiveBubbles();
			_player.Stream = OnGiveBubblesAudio;
			_player.Play();
			_player.Finished += OnAudioFinished;
		}
    }

    private void OnAudioFinished()
    {
		QueueFree();
    }

}

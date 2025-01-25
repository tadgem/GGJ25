using Godot;
using System;

public partial class SoundButton : Button
{

	[Export]
	public AudioStream HighlightAudio;

	[Export]
	public AudioStream AcceptAudio;
	
	private AudioStreamPlayer2D _streamPlayer;

	private bool _wasHoveredLastFrame = false;
	private bool _visible = false;
	// Called when the node enters the scene tree for the first time.

	private void OnAccept()
    {
		_streamPlayer.Stream = AcceptAudio;
		_streamPlayer.Play();
    }
	private void OnHighlight()
    {
		_streamPlayer.Stream = HighlightAudio;
		_streamPlayer.Play();
    }
	public override void _Ready()
	{
		Pressed += OnAccept;
		VisibilityChanged += OnVisibilityChanged;
		
		_streamPlayer = new AudioStreamPlayer2D();
		AddChild(_streamPlayer);
	}

    private void OnVisibilityChanged()
    {
		_visible = !_visible;
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Process(double delta)
	{
		bool currently_hovered = IsHovered();
		bool hover_invoke = _wasHoveredLastFrame == false && currently_hovered;
		bool controller_invoke = 
			Input.IsActionJustPressed("ui_up") 		|| 
			Input.IsActionJustPressed("ui_down")	||
			Input.IsActionJustPressed("ui_right")	||
			Input.IsActionJustPressed("ui_left");
		
		if((hover_invoke || controller_invoke) && _visible)
		{
			// play sound
			OnHighlight();
		}
		_wasHoveredLastFrame = currently_hovered;
	}
}

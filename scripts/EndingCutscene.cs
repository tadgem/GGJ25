using Godot;
using System;

public partial class EndingCutscene : Control
{
	[Export]
	public AudioStream EndingAudio
	;
	[Export]
	public float FadeSpeed = 5.0f;

	[Export]
	public string LevelToLoadOnFinish;


	private TextureRect _endingImage;
	private RichTextLabel _endingText;

	private AudioStreamPlayer _audio;
	private LooneyTransition _inTransition;
	private LooneyTransition _outTransition;

	private RichTextLabel _currentLabel = null;
	private TextureRect _currentTextureToFade = null;
	private AudioStream _currentAudio = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_audio = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		_inTransition = GetNode<LooneyTransition>("InTransition");
		_outTransition= GetNode<LooneyTransition>("OutTransition");
		_endingImage 			= GetNode<TextureRect>("Control/EndingImage");

		_inTransition.OnTransitionFinished += OnInTransitionFinished;
		
		_endingText = GetNode<RichTextLabel>("Control/EndingImage/ColorRect/RichTextLabel");

		_endingText.VisibleRatio = 0.0f;
	}

    private void OnInTransitionFinished(StringName animName)
    {
		_audio.Stream = EndingAudio;
		_audio.Play();
		_audio.Finished += OnEndingAudioFinished;
		_currentLabel = _endingText;
		_currentAudio = EndingAudio;
    }

    private void OnEndingAudioFinished()
    {
		GD.Print("Finished from main menu audio called");
		_audio.Finished -= OnEndingAudioFinished;
		_outTransition.Show();
		_outTransition.PlayOutTransition();
		_outTransition.OnTransitionFinished += OnOutTransitionFinished;
    }

    private void OnOutTransitionFinished(StringName animName)
    {
		// change scene
		if(LevelToLoadOnFinish != string.Empty)
		{
			var error = GetTree().ChangeSceneToFile(LevelToLoadOnFinish);
			if(error != Error.Ok)
			{
				GD.Print($"OnLooneyFinished : Failed to change scene to packed : {error}");
			}
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.




    public override void _Process(double delta)
	{
		if(_currentLabel != null && _currentAudio != null)
		{
			_currentLabel.VisibleRatio = (_audio.GetPlaybackPosition() / (float) _currentAudio.GetLength()) * 1.2f;
		}

		if(_currentTextureToFade != null)
		{
			Color current = _currentTextureToFade.Modulate;
			current.A -=  (float) delta * FadeSpeed;
			_currentTextureToFade.Modulate = current;
			GD.Print($"Fading Texture : Current Colour After Decrement: {current}");
			if(current.A <= 0)
			{
				_currentTextureToFade = null;
			}
		}
	}
}

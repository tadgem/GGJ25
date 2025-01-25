using Godot;
using System;

public partial class OpeningCutscene : Control
{
	[Export]
	public AudioStream MainMenuAudio;
	[Export]
	public AudioStream NestDestroyedAudio;

	[Export]
	public AudioStream FarmlandCottageAudio;

	[Export]
	public float FadeSpeed = 5.0f;

	[Export]
	public string LevelToLoadOnFinish;


	private TextureRect _mainMenuImage;
	private RichTextLabel _mainMenuText;
	private TextureRect _nestDestoryedImage;
	private RichTextLabel _nestDestroyedText;
	private TextureRect _farmlandCottageImage;
	private RichTextLabel _farmlandCottageText;

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
		_mainMenuImage 			= GetNode<TextureRect>("Control/MainMenuImage");
		_nestDestoryedImage 	= GetNode<TextureRect>("Control/NestDestroyedImage");
		_farmlandCottageImage	= GetNode<TextureRect>("Control/FarmlandCottageImage");

		_inTransition.OnTransitionFinished += OnInTransitionFinished;
		
		_mainMenuText = GetNode<RichTextLabel>("Control/MainMenuImage/ColorRect/RichTextLabel");
		_nestDestroyedText = GetNode<RichTextLabel>("Control/NestDestroyedImage/ColorRect/RichTextLabel");
		_farmlandCottageText = GetNode<RichTextLabel>("Control/FarmlandCottageImage/ColorRect/RichTextLabel");

		_mainMenuText.VisibleRatio = 0.0f;
		_nestDestroyedText.VisibleRatio = 0.0f;
		_farmlandCottageText.VisibleRatio = 0.0f;

		_currentAudio = MainMenuAudio;

	}

    private void OnInTransitionFinished(StringName animName)
    {
		_audio.Stream = MainMenuAudio;
		_audio.Play();
		_audio.Finished += OnMainMenuAudioFinished;
		_currentLabel = _mainMenuText;
    }

    private void OnMainMenuAudioFinished()
    {
		GD.Print("Finished from main menu audio called");
		_audio.Finished -= OnMainMenuAudioFinished;
		_audio.Stream = NestDestroyedAudio;
		_audio.Finished += OnNestDestroyedAudioFinished;
		_currentLabel = _nestDestroyedText;
		_currentTextureToFade = _mainMenuImage;
		_currentAudio = NestDestroyedAudio;
		_audio.Play();
    }

    private void OnNestDestroyedAudioFinished()
    {
		GD.Print("Finished from nest destroyed audio called");

		_audio.Finished -= OnNestDestroyedAudioFinished;
		_audio.Stream = FarmlandCottageAudio;
		_audio.Finished += OnFarmlandCottageAudioFinished;
		_currentLabel = _farmlandCottageText;
		_currentTextureToFade = _nestDestoryedImage;
		_currentAudio = FarmlandCottageAudio;
		_audio.Play();
    }

    private void OnFarmlandCottageAudioFinished()
    {
		GD.Print("Finished from farmland cottage audio called");
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

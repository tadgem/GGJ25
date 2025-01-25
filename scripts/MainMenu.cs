using Godot;
using System;

public partial class MainMenu : Control
{
	private Button _startGameButton;
	private Button _quitGameButton;
	private LooneyTransition _looneyTransition;
	private LooneyTransition _looneyInTransition;
	
	[Export]
	public string LevelToLoadOnStartPath;

	[Export]
	public float MusicFadeDbInSeconds = 18.0f;

	private Node _loadedScene = null;
	private AudioStreamPlayer _menuMusic = null;

	Timer _loadLevelTimer = new Timer();
	private bool _transitioning = false;

	private void OnLooneyFinished(StringName animName)
	{
		if(LevelToLoadOnStartPath != string.Empty && _transitioning)
		{
			GD.Print($"Changing Level from Pause Menu to : {LevelToLoadOnStartPath}");
			var error = GetTree().ChangeSceneToFile(LevelToLoadOnStartPath);
			if(error != Error.Ok)
			{
				GD.Print($"OnLooneyFinished : Failed to change scene to packed : {error}");
			}
		}
	}
	
	internal void GrabMenuFocus()
	{
		_startGameButton.GrabFocus();
	}
	
	private void StartTransition()
	{
		if(_transitioning)
		{
			return;
		}
		GD.Print("Start Transition");
		_transitioning = true;
		_looneyTransition.Show();
		_looneyTransition.PlayOutTransition();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Calling Main Menu READY");

		_startGameButton = GetNode<Button>("VBoxContainer/StartButton");
		_quitGameButton = GetNode<Button>("VBoxContainer/QuitButton");
		_looneyTransition = GetNode<LooneyTransition>("LooneyTransition");
		_looneyInTransition = GetNode<LooneyTransition>("EnterTransition");
		_menuMusic = GetNode<AudioStreamPlayer>("MainMenuMusic");
		_looneyTransition.OnTransitionFinished += OnLooneyFinished;

		GD.Print($"_looneyTransition is null? : {_looneyTransition == null}");
		GD.Print($"_looneyInTransition is null? : {_looneyInTransition == null}");
		// LooneyTransition enter = GetNode<LooneyTransition>("EnterTransition");
		_startGameButton.Pressed += StartTransition;
		GrabMenuFocus();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{		
		if(_transitioning)
		{
			_menuMusic.VolumeDb -= MusicFadeDbInSeconds * (float) delta;
		}

		//GD.Print($"Is Animation Player Current Anim Name : {_looneyInTransition.Animator.CurrentAnimation}");
		//GD.Print($"Is Animation Player Current Anim Length : {_looneyInTransition.Animator.CurrentAnimationLength}");
		//GD.Print($"Is Animation Player Current Anim Pos : {_looneyInTransition.Animator.CurrentAnimationPosition}");
		//GD.Print($"Is Animation Player Current Anim Quit on Finish: {_looneyInTransition.Animator.MovieQuitOnFinish}");		
	}
}

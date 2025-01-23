using Godot;
using System;

public partial class MainMenu : Control
{
	private Button _startGameButton;
	private Button _quitGameButton;
	private LooneyTransition _looneyTransition;
	[Export]
	public PackedScene LevelToLoadOnStart;

	private Node _loadedScene = null;

	Timer _loadLevelTimer = new Timer();
	private bool _transitioning = false;
	private void OnLooneyFinished(StringName animName)
	{
		if(LevelToLoadOnStart != null && _transitioning)
		{
			// where is the shitty behaviour coming from
			//GetTree().Paused = true;
			GetTree().ChangeSceneToPacked(LevelToLoadOnStart);
			_loadLevelTimer.Start();			
		}
	}

	private void OnTimerFinished()
    {
		GD.Print("Load Level Timer Finished");
		QueueFree();
		// GetViewport().AddChild(_loadedScene);
    }

	private void StartTransition()
	{
		if(_transitioning)
		{
			return;
		}
		GD.Print("Start Transition");
		//GetTree().Paused = true;
		_transitioning = true;
		_looneyTransition.Show();
		_looneyTransition.PlayOutTransition();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startGameButton = GetNode<Button>("VBoxContainer/StartButton");
		_quitGameButton = GetNode<Button>("VBoxContainer/QuitButton");
		_looneyTransition = GetNode<LooneyTransition>("LooneyTransition");
		_looneyTransition.OnTransitionFinished += OnLooneyFinished;

		_startGameButton.Pressed += StartTransition;

		//_loadLevelTimer.WaitTime = 0.1f;
		//_loadLevelTimer.Timeout += OnTimerFinished;
		// AddChild(_loadLevelTimer);

	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		
	}
}

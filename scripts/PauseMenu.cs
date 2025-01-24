using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export]
	public string MainMenuScenePath ;
	
	private Button _resumeButton;
	private Button _quitButton;

	private MouseCharacter _player;
	private LooneyTransition _transition;
	// Called when the node enters the scene tree for the first time.

	private void OnResumePressed()
	{
		_player.TogglePause();
		GD.Print("Toggle Pause");
	}

	private void LoadMainMenuLevel()
	{
		GD.Print($"Changing Level from Pause Menu to : {MainMenuScenePath}");
		Engine.TimeScale = 1.0f;
		var error = GetTree().ChangeSceneToFile(MainMenuScenePath);
		if(error != Error.Ok)
		{
			GD.Print($"LoadMainMenuLevel : Failed to change scene to packed : {error}");
		}
		
	}
	
    private void OnQuitPressed()
    {
		// GD.Print("On Quit Pressed");
		// _transition.Show();
		// _transition.PlayOutTransition();
		// GD.Print("On Quit Logic Finished");
		// TODO: Do transition, not working atm :( 
		LoadMainMenuLevel();
    }

	internal void GrabMenuFocus()
	{
		_resumeButton.GrabFocus();
	}
	

	public override void _Ready()
	{
		_resumeButton = GetNode<Button>("MarginContainer/VBoxContainer/ResumeButton");
		_quitButton = GetNode<Button>("MarginContainer/VBoxContainer/MainMenuButton");
		_transition = GetNode<LooneyTransition>("LooneyTransitionOut");
		_player = GetNode<MouseCharacter>("../");

		if(_player == null)
		{
			GD.Print("Pause Menu : Did not find player");
		}

		_resumeButton.Pressed += OnResumePressed;
		_quitButton.Pressed += OnQuitPressed;		
	}


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}

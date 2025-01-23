using Godot;
using System;

public partial class PauseMenu : Control
{
	private Button _resumeButton;
	private Button _quitButton;

	private MouseCharacter _player;
	// Called when the node enters the scene tree for the first time.

	private void OnResumePressed()
	{
		_player.TogglePause();
		GD.Print("Toggle Pause");
	}

	internal void GrabMenuFocus()
	{
		_resumeButton.GrabFocus();
	}
	

	public override void _Ready()
	{
		_resumeButton = GetNode<Button>("MarginContainer/VBoxContainer/ResumeButton");
		_quitButton = GetNode<Button>("MarginContainer/VBoxContainer/MainMenuButton");
		_player = GetNode<MouseCharacter>("../");

		if(_player == null)
		{
			GD.Print("Pause Menu : Did not find player");
		}

		_resumeButton.Pressed += OnResumePressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

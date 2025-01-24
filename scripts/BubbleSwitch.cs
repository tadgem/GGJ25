using Godot;
using System;

public partial class BubbleSwitch : Area3D
{
	public delegate void BubbleSwitchActivatedHandler(string tag);

	[Export]
	public string Tag = "";

	public event BubbleSwitchActivatedHandler OnSwitchActivated;


	private MeshInstance3D _upMesh;
	private MeshInstance3D _downMesh;

	private bool _active = false;
	

	private void ToggleSwitch()
	{
		if(!_active)
		{
			_upMesh.Hide();
			_downMesh.Show();
			OnSwitchActivated?.Invoke(Tag);
		}
		else
		{
			_upMesh.Show();
			_downMesh.Hide();
		}
		_active = !_active;
	}

	private void OnAreaEntered(Node3D other)
	{
		GD.Print($"Something entered the switch : {other.Name}");
		if(other is Area3D potential_bubble)
		{
			Bubble b = potential_bubble.GetNode<Bubble>("..");
			if(b != null)
			{
				GD.Print("It was a bubble!");
				ToggleSwitch();
			}
		}
	}

	private void OnAreaExited(Node3D other)
	{

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_upMesh = GetNode<MeshInstance3D>("SwitchUp");
		_downMesh = GetNode<MeshInstance3D>("SwitchDown");

		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		_upMesh.Show();
		_downMesh.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

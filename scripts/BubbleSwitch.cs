using Godot;
using System;

public partial class BubbleSwitch : Area3D
{
	public delegate void BubbleSwitchToggledHandler(string tag, bool enabled);

	[Export]
	public string Tag = "";

	[Export]
	public bool Timed = false;

	[Export]
	public float DoorTime = 5.0f;

	public event BubbleSwitchToggledHandler OnSwitchToggled;


	private MeshInstance3D _upMesh;
	private MeshInstance3D _downMesh;
	private TextureProgressBar _progress;

	private bool _active = false;
	private bool _timerActive = false;
	private float _timerTime = 0.0f;
	private void StartTimer()
	{
		if(!Timed)
		{
			return;
		}
		_timerTime = DoorTime;
		_timerActive = true;
	}

	private void ResetTimer()
	{
		if(!Timed)
		{
			return;
		}
		_timerActive = false;
		_timerTime = 0.0f;
	}

	private void ToggleSwitch()
	{
		_active = !_active;
		if(_active)
		{
			_upMesh.Hide();
			_downMesh.Show();
			if(Timed)
			{
				_progress.Show();
				StartTimer();
			}
		}
		else
		{
			_upMesh.Show();
			_downMesh.Hide();
			if(Timed)
			{
				_progress.Hide();
				ResetTimer();
			}
		}
		OnSwitchToggled?.Invoke(Tag, _active);
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
		_progress = GetNode<TextureProgressBar>("Control/TextureProgressBar");

		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
		_active = false;
		_upMesh.Show();
		_downMesh.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Timed || !_timerActive)
		{
			return;
		}

		_timerTime -= (float) delta;

		_progress.Value = (double) (_timerTime / DoorTime) * 100.0f;

		if(_timerTime <= 0.0f)
		{
			GD.Print("Timer Expired, toggling switch");
			ToggleSwitch();
		}
	}
}

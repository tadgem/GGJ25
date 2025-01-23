using Godot;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Cryptography;

public partial class MouseCharacter : CharacterBody3D
{
	[Export]
	public float MouseSensitivity = 0.33f;
	[Export]
	public float Acceleration = 30.0f;
	[Export]
	public float MoveSpeed = 30.0f;
	[Export]
	public float MovementDeadZone = 0.2f;
	[Export]
	public float RotationSpeed = 12.0f;
	[Export]
	public float JumpForce = 12.0f;
	[Export]
	public float AimSpeed = 2.0f;
	[Export]
	public float FirePlatformSpeed = 4.0f;
	[Export]
	public float FireProjectileSpeed = 200.0f;
	[Export]
	public float MinSqueekTime = 17.0f;
	[Export]
	public float MiaxSqueekTime = 35.0f;
	[Export]
	public float ControllerLookSensitivity = 2.0f;
	[Export]
	public float ProjectileBubbleTimeAlive = 4.0f;
	[Export]
	public float PlatformBubbleTimeAlive = 4.0f;
	
	[Export]
	public Vector3 AimPivot = new Vector3(0.0f, 3.25f, - 0.5f);

	[Export]
	public PackedScene Bubble;


	private Vector2 _mouseDir = Vector2.Zero;
	private Vector3 _lastMovementDir = Vector3.Back;
	private Vector3 _defaultPivot = Vector3.Zero;


	private Node3D _pivot = null;
	private Node3D _playerModel = null;
	private Camera3D _cam = null;
	private AnimationTree _anim = null;
	private AudioStreamPlayer _squeekAudio;
	private AudioStreamPlayer _jumpAudio;
	private Control _crosshairControl;
	private Control _pauseMenuControl;
	private float _gravity = -30.0f;
	private bool _bounce = false;
	private float _bounceStrength = 0.0f;
	private float _mouseSqueekTimer = 0.0f;

	private TextureRect[] _bubbleUiTextures = new TextureRect[3];
	private float[]	_bubbleUiTimers = new float[3];
	private const int MAX_ACTIVE_BUBBLES = 3;

	private bool _paused = false;

	internal void Bounce(float strength_multiplier = 1.0f)
	{
		_bounce = true;
		_bounceStrength = strength_multiplier;
	}


	private void HandleAnimationParams()
	{
		if(!IsOnFloor())
		{
			_anim.Set("parameters/conditions/moving", false);
			_anim.Set("parameters/conditions/idle", false);
			_anim.Set("parameters/conditions/jump", true);
			return;
		}
		if(Velocity.Length() > MovementDeadZone)
		{
			_anim.Set("parameters/conditions/moving", true);
			_anim.Set("parameters/conditions/idle", false);
			_anim.Set("parameters/conditions/jump", false);
		}
		else
		{
			_anim.Set("parameters/conditions/moving", false);
			_anim.Set("parameters/conditions/idle", true);
			_anim.Set("parameters/conditions/jump", false);
		}
	}

	private void HandleSqueekAudio(float delta)
	{
		_mouseSqueekTimer -= delta;
		if(_mouseSqueekTimer <= 0.0f)
		{
			_squeekAudio.Play();
			_mouseSqueekTimer = (float) GD.RandRange(MinSqueekTime, MiaxSqueekTime);
		}
	}

    public override void _Ready()
    {
        base._Ready();
		_pivot = GetNode<Node3D>("CamPivot");
		_cam = GetNode<Camera3D>("CamPivot/SpringArm3D/Camera3D");
		_playerModel = GetNode<Node3D>("Model");
		_anim = GetNode<AnimationTree>("Model/AnimationTree");
		_crosshairControl = GetNode<Control>("Control");
		_crosshairControl.Hide();
		_pauseMenuControl = GetNode<Control>("PauseMenu");
		_pauseMenuControl.Hide();
		_squeekAudio = GetNode<AudioStreamPlayer>("SqueekAudio");
		_jumpAudio = GetNode<AudioStreamPlayer>("JumpAudio");
		_anim.Active = true;

		_bubbleUiTextures[0] = GetNode<TextureRect>("BubblesUI/Container/Bubble1");
		_bubbleUiTextures[1] = GetNode<TextureRect>("BubblesUI/Container/Bubble2");
		_bubbleUiTextures[2] = GetNode<TextureRect>("BubblesUI/Container/Bubble3");

		_bubbleUiTimers[0] = 0.0f;
		_bubbleUiTimers[1] = 0.0f;
		_bubbleUiTimers[2] = 0.0f;

		_defaultPivot = _pivot.Position;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);


		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.IsPressed())
		{
			if(mouseButtonEvent.ButtonIndex == MouseButton.Left)
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
		
		if(@event is InputEventKey keyEvent)
		{
			if(keyEvent.Keycode == Key.Escape && keyEvent.IsPressed())
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}

		if (@event is InputEventMouseMotion mouseMotionEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			_mouseDir = mouseMotionEvent.ScreenRelative * MouseSensitivity;	
		}

    }

	private void HandleCameraRotation(float delta)
	{
		if(_mouseDir == Vector2.Zero)
		{
			_mouseDir = Input.GetVector("look_left", "look_right", "look_up", "look_down") * ControllerLookSensitivity;
		}
		Vector3 euler = _pivot.Rotation;
		euler.X += _mouseDir.Y * (float) delta;
		euler.Y -= _mouseDir.X * (float) delta;

		euler.X = Mathf.Clamp(euler.X, (float) -Math.PI / 6.0f, (float)Math.PI / 3.0f);
		_pivot.Rotation = euler;

		_mouseDir = Vector2.Zero;

	}

	private bool CanFire()
	{
		return 	_bubbleUiTimers[0] <= 0.0f ||
				_bubbleUiTimers[1] <= 0.0f || 
				_bubbleUiTimers[2] <= 0.0f ;
	}

	private int GetBubbleFireIndex()
	{
		if(_bubbleUiTimers[0] <= 0.0f)
		{
			return 0;
		}

		if(_bubbleUiTimers[1] <= 0.0f)
		{
			return 1;
		}

		if(_bubbleUiTimers[2] <= 0.0f)
		{
			return 2;
		}

		return -1;
	}

	private void OnBubbleFired(float timeAlive)
	{
		int bubbleIndex = GetBubbleFireIndex();

		if(bubbleIndex < 0)
		{
			GD.Print("Failed to fire, should not be able to get here!");
			return;
		}

		_bubbleUiTimers[bubbleIndex] = timeAlive;
		_bubbleUiTextures[bubbleIndex].Hide();
		_bubbleUiTextures[bubbleIndex].Visible = false;
	}

	private void FirePlatform()
	{
		Bubble bubble_instance = (Bubble) Bubble.Instantiate();	
		bubble_instance.IsTimed = true;
		bubble_instance.Lifetime = PlatformBubbleTimeAlive;
		bubble_instance.IsProjectile = true;
		GetTree().CurrentScene.AddChild(bubble_instance);
		bubble_instance.Position = Position + Vector3.Up +(-_cam.GlobalBasis.Z * 4.0f);
		bubble_instance.Velocity = -_cam.GlobalBasis.Z * FirePlatformSpeed;	
		OnBubbleFired(PlatformBubbleTimeAlive);
	}

	private void FireProjectile()
	{
		Bubble bubble_instance = (Bubble) Bubble.Instantiate();	
		bubble_instance.IsTimed = true;
		bubble_instance.Lifetime = ProjectileBubbleTimeAlive;
		bubble_instance.IsProjectile = true;
		GetTree().CurrentScene.AddChild(bubble_instance);
		bubble_instance.Position = Position + Vector3.Up +(-_cam.GlobalBasis.Z * 4.0f);
		bubble_instance.Velocity = -_cam.GlobalBasis.Z * FireProjectileSpeed;	
		OnBubbleFired(ProjectileBubbleTimeAlive);

	}

	private void HandleAiming(float delta)
	{
		bool aim = Input.IsActionPressed("aim");

		if(aim)
		{
			_crosshairControl.Show();
			_pivot.Position = _pivot.Position.Lerp(AimPivot, (float) delta * AimSpeed);


			bool fire_platform = Input.IsActionJustPressed("fire_platform");
			bool fire_projectile = Input.IsActionJustPressed("fire_projectile");
			
			if(fire_platform)
			{
				if(CanFire())
				{
					FirePlatform();	
				}
			}
			if(fire_projectile)
			{
				if(CanFire())
				{
					FireProjectile();
				}
			}
		}
		else
		{
			_crosshairControl.Hide();
			_pivot.Position = _pivot.Position.Lerp(_defaultPivot, (float) delta * AimSpeed);
		}
	}

	private Vector3 HandleCharacterVelocity(float delta)
	{
		var raw_input = Input.GetVector("move_left", "move_right", "move_backward", "move_forward");
		Vector3 forward = -_cam.GlobalBasis.Z;
		Vector3 right = _cam.GlobalBasis.X;

		Vector3 move_dir = (forward * raw_input.Y) + (right * raw_input.X);
		move_dir.Y = 0.0f;
		move_dir = move_dir.Normalized();

		Vector3 v = Velocity;
		float y_vel = v.Y;
		v.Y = 0.0f;
		v = Velocity.MoveToward(move_dir * MoveSpeed, Acceleration *  delta);
		v.Y = y_vel + _gravity *  delta;

		bool jump = Input.IsActionJustPressed("jump") && IsOnFloor();

		if(jump)
		{
			_jumpAudio.Play();
			v.Y += JumpForce;
		}

		if(_bounce)
		{
			v.Y = Mathf.Clamp(v.Y, 0.0f, float.MaxValue);
			v.Y += JumpForce * _bounceStrength;
			_bounce = false;
			_bounceStrength = 1.0f;
		}

		Velocity = v;

		return move_dir;
	}

	private void HandleCharacterRotation(Vector3 move_dir, float delta)
	{
		if(move_dir.Length() > MovementDeadZone)
		{
			_lastMovementDir = move_dir;
			float target_angle = Vector3.Back.SignedAngleTo(_lastMovementDir, Vector3.Up);
			Vector3 playerEuler = _playerModel.GlobalRotation;
			playerEuler.Y = Mathf.LerpAngle(_playerModel.Rotation.Y, target_angle, RotationSpeed * (float) delta);
			playerEuler.X = 0.0f;

			_playerModel.GlobalRotation = playerEuler;
		}
	}

	private void HandleActiveBubbles(float delta)
	{

		for(int i = 0; i < MAX_ACTIVE_BUBBLES; i++)
		{
			if(_bubbleUiTimers[i] <= 0.0f)
			{
				_bubbleUiTextures[i].Show();
				_bubbleUiTextures[i].Visible = true;
			}
			else{
				_bubbleUiTimers[i] -= delta;
			}
		}
	}

	internal void TogglePause()
	{
		if(_paused)
			{
				Engine.TimeScale = 1.0f;
				_pauseMenuControl.Hide();
			}
			else
			{
				Engine.TimeScale = 0.0f;
				_pauseMenuControl.Show();
			}
			_paused = !_paused;
	}

	private void HandlePauseMenu()
	{
		if(Input.IsActionJustPressed("pause"))
		{
			TogglePause();
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		float d = (float) delta;

		HandleCameraRotation(d);
		Vector3 move_dir = HandleCharacterVelocity(d);
		MoveAndSlide();
		HandleAnimationParams();
		HandleActiveBubbles(d);
		HandleAiming(d);
		HandleCharacterRotation(move_dir, d);
		HandleSqueekAudio(d);
		HandlePauseMenu();
    }
}
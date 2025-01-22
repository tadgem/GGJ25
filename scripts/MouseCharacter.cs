using Godot;
using System;
using System.Reflection.Metadata;

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
	public float ProjectileSpeed = 400.0f;
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
	private Control _crosshairControl;
	private float _gravity = -30.0f;
	private bool _bounce = false;
	private float _bounceStrength = 0.0f;

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

    public override void _Ready()
    {
        base._Ready();
		_pivot = GetNode<Node3D>("CamPivot");
		_cam = GetNode<Camera3D>("CamPivot/SpringArm3D/Camera3D");
		_playerModel = GetNode<Node3D>("Model");
		_anim = GetNode<AnimationTree>("Model/AnimationTree");
		_crosshairControl = GetNode<Control>("Control");
		_crosshairControl.Hide();
		_anim.Active = true;

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
		Vector3 euler = _pivot.Rotation;
		euler.X += _mouseDir.Y * (float) delta;
		euler.Y -= _mouseDir.X * (float) delta;

		euler.X = Mathf.Clamp(euler.X, (float) -Math.PI / 6.0f, (float)Math.PI / 3.0f);
		_pivot.Rotation = euler;

		_mouseDir = Vector2.Zero;

	}

	private void HandleAiming(float delta)
	{
		bool aim = Input.IsActionPressed("aim");

		if(aim)
		{
			_crosshairControl.Show();
			_pivot.Position = _pivot.Position.Lerp(AimPivot, (float) delta * AimSpeed);

			bool fire = Input.IsActionJustPressed("fire");

			if(fire)
			{
				Bubble bubble_instance = (Bubble) Bubble.Instantiate();	
				bubble_instance.IsTimed = true;
				bubble_instance.IsProjectile = true;
				GetTree().CurrentScene.AddChild(bubble_instance);
				bubble_instance.Position = Position + Vector3.Up +(-_cam.GlobalBasis.Z * 4.0f);
				bubble_instance.Velocity = -_cam.GlobalBasis.Z * ProjectileSpeed;			

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

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		float d = (float) delta;

		HandleCameraRotation(d);
		Vector3 move_dir = HandleCharacterVelocity(d);
		MoveAndSlide();
		HandleAnimationParams();
		HandleAiming(d);
		HandleCharacterRotation(move_dir, d);
    }
}
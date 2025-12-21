using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float DecelerationSpeed = Speed * 0.1f;
	public const float JumpForce = 10;
	public float MouseSensitivity = 0.2f;
	float GravitySpeed = 20.0f;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

	public override void _Input(InputEvent inputEvent)
	{
	   if (inputEvent is InputEventMouseMotion eventMouseMotion)
		{
			int width = DisplayServer.WindowGetSize().X;
			float change = (float)width / 360.0f;

			float angle = eventMouseMotion.Relative.X * change * this.MouseSensitivity;
			RotateY(-Mathf.DegToRad(angle));
			
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (IsOnFloor())
		{
			velocity.Y = 0;
		}
		else
		{
			velocity.Y -= this.GravitySpeed * (float)delta;
		}
		Godot.Vector3 movement = new Godot.Vector3(0.0f,0.0f, 0.0f);

		// Handle Jump.
		if (Input.IsActionJustPressed("move_up") && IsOnFloor())
		{
			velocity.Y = JumpForce;
		}

		
		if (Input.IsActionPressed("move_forward"))
		{
			movement.X += 1.0f;
		}
		if (Input.IsActionPressed("move_backward"))
		{
			movement.X += -1.0f;
		}
		if (Input.IsActionPressed("move_left"))
		{
			movement.Z += -1.0f;
		}
		if (Input.IsActionPressed("move_right"))
		{
			movement.Z += 1.0f;
		}
		movement *= Speed;

		movement = movement.Rotated(Godot.Vector3.Up, this.Rotation.Y + Mathf.DegToRad(90));
		if (movement.Z != 0.0f)
		{
			velocity.Z = movement.Z;
		}
		else
		{
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, DecelerationSpeed);
		}

		if (movement.X != 0.0f)
		{
			velocity.X = movement.X;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, DecelerationSpeed);
		}

		Velocity = velocity;
		
		MoveAndSlide();

	}
}

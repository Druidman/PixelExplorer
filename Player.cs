using Godot;
using System;


public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float DecelerationSpeed = Speed * 0.1f;
	public const float JumpForce = 10;
	public float MouseSensitivity = 0.2f;
	float GravitySpeed = 20.0f;
	MeshInstance3D character;
	CollisionShape3D characterCollider;
	Camera camera;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Confined;
		this.character = (MeshInstance3D)GetNode("Character");
		this.characterCollider = (CollisionShape3D)GetNode("CharacterCollider");
		this.camera = (Camera)GetNode("Camera3D");


		
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseMotion eventMouseMotion)
		{
			Godot.Vector2 mousePos = GetViewport().GetMousePosition(); 

			Godot.Vector3 origin = this.camera.ProjectRayOrigin(mousePos);
			Godot.Vector3 end = origin + this.camera.ProjectRayNormal(mousePos) * 1000;


			PhysicsDirectSpaceState3D space_state = this.GetWorld3D().DirectSpaceState;
			PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, end);
			var result = space_state.IntersectRay(query);
			if (result.Count > 0)
			{
				var hitPos = (Godot.Vector3)result["position"];	

				var angle = new Vector2(this.Position.X, this.Position.Z).AngleToPoint(new Vector2(hitPos.X, hitPos.Z));
				this.character.RotateY(-(angle + this.character.Rotation.Y));
				this.characterCollider.RotateY(-(angle + this.character.Rotation.Y));
				
			}
			
		}
		if (inputEvent.IsActionPressed("exit"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
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

		movement = movement.Rotated(Godot.Vector3.Up, this.character.Rotation.Y);
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

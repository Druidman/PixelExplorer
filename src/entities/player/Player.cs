using Godot;
using System;


public partial class Player : CharacterBody3D
{
	public float MouseSensitivity = 0.2f;
	float GravitySpeed = 20.0f;
	MeshInstance3D character;
	CollisionShape3D characterCollider;
	Camera camera;
	bool DebugMode = false;
	public World world = null;


    public override void _EnterTree()
    {
        GlobalPosition = GameGlobals.PlayerStartPos;
    }
	public override void _Ready()
	{
		
		this.character = (MeshInstance3D)GetNode("Character");
		this.characterCollider = (CollisionShape3D)GetNode("CharacterCollider");
		this.camera = (Camera)GetNode("Camera3D");

		this.GlobalPosition = GameGlobals.PlayerStartPos;

	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseMotion eventMouseMotion)
		{
			Godot.Vector2 mousePos = GetViewport().GetMousePosition(); 

			var Player2DPos = new Godot.Vector2(this.Position.X, this.Position.Z);

			var mousePointPos = Player2DPos + (mousePos - (DisplayServer.WindowGetSize() / 2));

		

			var angle = Player2DPos.AngleToPoint(mousePointPos);
			
			this.character.RotateY(-(angle + this.character.Rotation.Y));
			this.characterCollider.RotateY(-(angle + this.character.Rotation.Y));
				
			
			
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		
		// Add the gravity.
		if (!DebugMode)
		{	
			// Area3D area = GetNode<Area3D>("Area");
			// GD.Print(area.GetOverlappingBodies().Count);
			if (IsOnFloor() )//|| area.GetOverlappingBodies().Count > 1)
			{
				velocity.Y = 0;
			}
			else
			{
				velocity.Y -= this.GravitySpeed * (float)delta;
			}
		}
		
		Godot.Vector3 movement = new Godot.Vector3(0.0f,0.0f, 0.0f);

		// Handle Jump.
		if (Input.IsActionJustPressed("move_up") && IsOnFloor())
		{
			velocity.Y = GameGlobals.PlayerJumpForce;
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
		movement *= GameGlobals.PlayerSpeed;

		movement = movement.Normalized();

		movement = movement.Rotated(Godot.Vector3.Up, this.character.Rotation.Y);


		if (movement.Z != 0.0f)
		{
			velocity.Z = movement.Z* GameGlobals.PlayerSpeed;
		}
		else
		{
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, GameGlobals.PlayerDecelerationSpeed);
		}

		if (movement.X != 0.0f)
		{
			velocity.X = movement.X * GameGlobals.PlayerSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, GameGlobals.PlayerDecelerationSpeed);
		}

		Velocity = velocity;
		
		MoveAndSlide();

	

	}
}

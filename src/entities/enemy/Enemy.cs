using Godot;
using System;

public partial class Enemy : CharacterBody3D
{


	public Godot.Vector3 moveDirection = new Godot.Vector3(0,0,0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 velocity = new Godot.Vector3(this.moveDirection.X * GameGlobals.PlayerSpeed * 1.1f, Velocity.Y, this.moveDirection.Z  * GameGlobals.PlayerSpeed * 1.1f);
		if (IsOnFloor())
		{
			velocity.Y = 0;
		}
		else
		{
			velocity.Y -= GameGlobals.GravitySpeed * (float)delta;
		}

		if (IsOnWall())
		{
			velocity.Y = GameGlobals.PlayerJumpForce;
		}
		if (IsOnFloor())
		{
			Velocity = velocity;	
		}
		else
		{
			velocity.X *= 0.5f;
			velocity.Z *= 0.5f;
			Velocity = velocity;
		}

		

		
		MoveAndSlide();
	}
}

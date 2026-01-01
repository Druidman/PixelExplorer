using Godot;
using System;

public partial class Enemy : CharacterBody3D
{

	float GravitySpeed = 20.0f;
	public Godot.Vector3 moveDirection = new Godot.Vector3(0,0,0);
	public float EnemySpeed = 10f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 velocity = new Godot.Vector3(this.moveDirection.X * EnemySpeed, Velocity.Y, this.moveDirection.Z  * EnemySpeed);
		if (IsOnFloor())
		{
			velocity.Y = 0;
		}
		else
		{
			velocity.Y -= this.GravitySpeed * (float)delta;
		}

		if (IsOnWall())
		{
			velocity.Y += 1;
		}
		if (IsOnFloor())
		{
			Velocity = velocity;	
		}
		else
		{
			Velocity = velocity;
		}

		

		
		MoveAndSlide();
	}
}

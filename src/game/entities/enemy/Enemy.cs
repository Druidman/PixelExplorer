using Godot;
using System;

public partial class Enemy : CharacterBody3D
{

	float GravitySpeed = 20.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 velocity = Velocity;
		if (IsOnFloor())
		{
			velocity.Y = 0;
		}
		else
		{
			velocity.Y -= this.GravitySpeed * (float)delta;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}

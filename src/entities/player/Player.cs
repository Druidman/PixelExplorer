using Godot;
using System;

public enum PlayerMovementStyle
{
	KeyboardMouse,
	MousePointer,
	MousePointerNoAutoJump,
	DestinationBased

}
public partial class Player : CharacterBody3D
{
	float GravitySpeed = 20.0f;
	public MeshInstance3D character;
	public CollisionShape3D characterCollider;
	public Camera camera;


	private Movement movement = null;

	public Player()
	{
		movement = new MovementKeyboardMouse(this);
	}

	public override void _EnterTree()
	{
		GlobalPosition = GameGlobals.PlayerStartPos;
		
	}
	public override void _Ready()
	{
		this.character = GetNode<MeshInstance3D>("Character");
		this.characterCollider = GetNode<CollisionShape3D>("CharacterCollider");
		this.camera = GetNode<Camera>("Camera");
	}

	
	public override void _Input(InputEvent inputEvent)
	{
		movement.HandleInputEvent(inputEvent);
	}

	public override void _PhysicsProcess(double delta)
	{

		movement.HandleProcess(delta);
	
		MoveAndSlide();
	}
}

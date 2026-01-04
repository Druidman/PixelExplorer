using Godot;
using System;


public partial class Player : CharacterBody3D
{
	public float MouseSensitivity = 0.2f;
	float GravitySpeed = 20.0f;
	public MeshInstance3D character;
	public CollisionShape3D characterCollider;
	public Camera camera;
	public bool DebugMode = false;
	public World world = null;

	Movement movement;

	Godot.Vector3 soldierPos = new Godot.Vector3(0,0,0);
	Godot.Vector3 soldierPosIncrement = new Godot.Vector3(3,0,0);
	int SoldierPosRotationAngle = 0;
	int SoldierLayerAmount = 0;


	private int coins = 0;
	public override void _EnterTree()
	{
		GlobalPosition = GameGlobals.PlayerStartPos;
	}
	public override void _Ready()
	{
		
		this.character = (MeshInstance3D)GetNode("Character");
		this.characterCollider = (CollisionShape3D)GetNode("CharacterCollider");
		this.camera = (Camera)GetNode("Camera");
		movement = new MovementKeyboardMouse(this);
		
	}

	public int GetCoins()
	{
		
		return coins;
	}

	public void AddCoins(int coinsToAdd)
	{
		this.coins += coinsToAdd;

	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseMotion eventMouseMotion)
		{
			movement.HandleInputEvent(inputEvent);
		}
	}

	private void SpawnSoldier()
	{
		if ((float)SoldierPosRotationAngle / 360f == SoldierPosRotationAngle / 360)
		{
			soldierPos += soldierPosIncrement;
			SoldierLayerAmount += 6;
		}
		Soldier soldier = GameGlobals.soldierScene.Instantiate<Soldier>();
		soldier.Initialize(this, soldierPos.Rotated(Godot.Vector3.Up,Mathf.DegToRad(SoldierPosRotationAngle)));
		AddSibling(soldier);
		SoldierPosRotationAngle += 360 / SoldierLayerAmount;

	}

	public override void _PhysicsProcess(double delta)
	{

		
		movement.HandleProcess(delta);

		if (Input.IsActionPressed("spawn_soldier"))
		{
			SpawnSoldier();
		}
	
		MoveAndSlide();
		
	}


	
}

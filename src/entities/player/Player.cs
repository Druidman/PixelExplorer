using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class Player : CharacterBody3D
{
	public float MouseSensitivity = 0.2f;
	float GravitySpeed = 20.0f;
	public MeshInstance3D character;
	public CollisionShape3D characterCollider;
	public Camera camera;
	public bool DebugMode = false;

	[Export]
	public World world = null;

	Movement movement;

	[Export]
	SoldierManager soldierManager;


	public List<SoldierHome> houses = new List<SoldierHome>();


	private int coins = GameGlobals.PlayerStartCoins;

	public int SoldierSlots = 2;

	public bool canMove = true;
	public override void _EnterTree()
	{
		GlobalPosition = GameGlobals.PlayerStartPos;
		soldierManager.Initialize(this);
	}
	public override void _Ready()
	{
		
		this.character = (MeshInstance3D)GetNode("Character");
		this.characterCollider = (CollisionShape3D)GetNode("CharacterCollider");
		this.camera = (Camera)GetNode("Camera");
		movement = new MouseGuidedMovement(this);
		
	}

	public int GetCoinCount()
	{
		
		return coins;
	}
	public int GetSoldierCount()
	{
		return this.soldierManager.soldiers.Count;
	}
	public int GetMaxSoldierCount()
	{
		return this.SoldierSlots;
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

	

	public void removeCoins(int coinsToRemove)
	{
		this.coins -= coinsToRemove;
	}


	public override void _PhysicsProcess(double delta)
	{

		
		movement.HandleProcess(delta);

		if (Input.IsActionPressed("spawn_soldier") && this.coins >= GameGlobals.SoldierCost)
		{
			this.soldierManager.SpawnSoldier();
			
		}
		if (Input.IsActionJustPressed("attack"))
		{
			soldierManager.SetDestroyObjective(this.houses.ElementAtOrDefault(0)); // TODO
		}
		if (Input.IsActionJustReleased("attack"))
		{
			soldierManager.SetDestroyObjective(null);
		}

	
		MoveAndSlide();
		soldierManager.Update((float)delta, this.characterCollider.Rotation);
		
	}

	

	public void ExpandSoldierSlots(int slotsDelta)
	{
		this.SoldierSlots += slotsDelta;
		if (this.SoldierSlots < 0)
		{
			this.SoldierSlots = 0;
		}
	}

	


	
}

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
	public bool isMouseButtonEventFree = true;
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
		if (inputEvent is InputEventMouseButton inputEventMouseButton && isMouseButtonEventFree)
		{
			var spaceState = GetWorld3D().DirectSpaceState;
			var cam = GetViewport().GetCamera3D();
			var mousePos = GetViewport().GetMousePosition();

			var origin = cam.ProjectRayOrigin(mousePos);
			var end = origin + cam.ProjectRayNormal(mousePos) * 1000; // TODO add normal length
			var query = PhysicsRayQueryParameters3D.Create(origin, end);
			query.CollideWithAreas = true;

			var result = spaceState.IntersectRay(query);	
			Godot.GodotObject godotObject = (Godot.GodotObject)result["collider"];
			if (godotObject is Building buildingObject)
			{
				soldierManager.SetDestroyObjective(buildingObject); // TODO
				
			}
			else
			{
				soldierManager.SetDestroyObjective(null);
			}
				
			
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

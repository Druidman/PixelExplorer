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
	public SoldierManager soldierManager;


	[Export]
	public ChunkRenderer chunkRenderer = null;


	[Export]
	public Godot.Collections.Array<PlayerAction> playerActions = new Godot.Collections.Array<PlayerAction>
	{
	};


	public PlayerAction currentlyActiveAction = null;

	public float speed = 0.0f;
	public List<SoldierHome> houses = new List<SoldierHome>();
	public List<ArcherTurret> archerTowers = new List<ArcherTurret>();
	public List<MagicTurret> magicTurrets = new List<MagicTurret>();


	private int coins = GameGlobals.PlayerStartCoins;

	public int SoldierSlots = 2;

	public float healthPoints = 20;

	public void TakeHealth(float delta)
	{
		if (delta < 0)
		{
			return;
		}

		this.healthPoints -= delta;
		if (this.healthPoints <= 0){
			this.world.GameLoose();
		}
		if (GameGlobals.punchSound != null) GameGlobals.punchSound.Play();
	}

	public bool canMove {
		get
		{
			return (this.currentlyActiveAction?.blocksMovement != null) ? !this.currentlyActiveAction.blocksMovement : true;
		}
	}

	public bool allowDefaultActions {
		get
		{
			return (this.currentlyActiveAction?.blocksDefaultPlayerActions != null) ? !this.currentlyActiveAction.blocksDefaultPlayerActions : true;
		}
	}
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
		foreach (PlayerAction action in this.playerActions)
		{
			if (inputEvent.IsActionPressed(action.actionName))
			{
				if (this.currentlyActiveAction == null)
				{
					this.currentlyActiveAction = action;	
					this.currentlyActiveAction.OnStart();
				}
				else if (this.currentlyActiveAction == action)
				{
					this.currentlyActiveAction.OnEnd();
					this.currentlyActiveAction = null;
				}
				else
				{
					this.currentlyActiveAction.OnEnd();
					this.currentlyActiveAction = action;	
					this.currentlyActiveAction.OnStart();
				}
				
				
				break;
			}
		}
		
		if (inputEvent is InputEventMouseMotion eventMouseMotion)
		{
			movement.HandleInputEvent(inputEvent);
		}	

		// here default events
		if (allowDefaultActions)
		{
			
		}
		

		currentlyActiveAction?.HandleInput(inputEvent);
		

		
	
		
	}

	

	public void removeCoins(int coinsToRemove)
	{
		this.coins -= coinsToRemove;
	}


	public override void _PhysicsProcess(double delta)
	{
		if (chunkRenderer.firstGen) return;
		
		if (!this.world.CheckIfValidGlobalPosition(this.GlobalPosition))
		{
			this.GlobalPosition -= this.Velocity;
		}
		movement.HandleProcess(delta);

		if (allowDefaultActions)
		{
			if (Input.IsActionPressed("spawn_soldier") && this.coins >= GameGlobals.SoldierCost)
			{
				this.soldierManager.SpawnSoldier();
				
			}	
		}
		

	
		MoveAndSlide();
		soldierManager.Update((float)delta, this.characterCollider.Rotation);


		currentlyActiveAction?.Update(delta);
		
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

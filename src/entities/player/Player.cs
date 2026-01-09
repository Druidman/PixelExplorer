using Godot;
using System;
using System.Collections.Generic;


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


	private int coins = 0;
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

	

	public override void _PhysicsProcess(double delta)
	{

		
		movement.HandleProcess(delta);

		if (Input.IsActionPressed("spawn_soldier") && this.coins >= GameGlobals.SoldierCost)
		{
			this.soldierManager.SpawnSoldier();
			this.coins -= GameGlobals.SoldierCost;
		}
		if (Input.IsActionJustPressed("SetGoldMine"))
		{
			
			List<Ore> ores = this.world.GetChunkOres(this.GlobalPosition);
			if (ores != null)
			{
				if (ores.Count > 0)
				{
					GD.Print((ores != null) ? ores : "No mines nearby");

					Ore selectedOre = ores[0];
					float CurMinDist = 100000f;
					foreach (Ore ore in ores)
					{
						float dist = ore.GlobalPosition.DistanceSquaredTo(this.GlobalPosition);
						if (dist < CurMinDist)
						{
							CurMinDist = dist;
							selectedOre = ore;
						}
					}
	
	
					Node3D mine = GameGlobals.GoldMineScene.Instantiate<Node3D>();
					this.world.AddChild(mine);
					mine.GlobalPosition = selectedOre.GlobalPosition;	
				}
				
			}
			
		}

	
		MoveAndSlide();
		soldierManager.Update((float)delta, this.characterCollider.Rotation);
		
	}


	
}

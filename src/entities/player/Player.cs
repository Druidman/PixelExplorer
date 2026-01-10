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

	public int SoldierSlots = 2;
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
		if (Input.IsActionJustPressed("SetGoldMine"))
		{
			this.PlaceGoldMine();
		}
		if (Input.IsActionJustPressed("SetSoldierHome"))
		{
			this.StartSoldierHomePlacing();
		}

	
		MoveAndSlide();
		soldierManager.Update((float)delta, this.characterCollider.Rotation);
		
	}

	private void StartSoldierHomePlacing()
	{
		var spaceState = GetWorld3D().DirectSpaceState;
		var cam = this.camera;
		var mousePos = GetViewport().GetMousePosition();

		var origin = cam.ProjectRayOrigin(mousePos);
		var end = origin + cam.ProjectRayNormal(mousePos) * 1000; // TODO add normal length
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollideWithAreas = true;

		var result = spaceState.IntersectRay(query);
		Godot.Vector3 hitPos = (Godot.Vector3)result.GetValueOrDefault("position");
		hitPos.Y = MathF.Round(hitPos.Y,1) - 0.01f;

		Chunk chunk = this.world.GetChunkAtPos(hitPos);

		GD.Print(chunk);
		int row = chunk.getRowGlobalZ(hitPos.Z);
		int col = chunk.getColGlobalX(hitPos.X);
		int platform = chunk.getPlatformGlobalY(hitPos.Y);

		if (chunk.CheckIfTileExists(platform, row, col) == null)
		{
			return;
		}
		
		Godot.Vector3 blockPosition = chunk.getGlobalPositionOfTile(platform, row, col);

		
		GD.Print(hitPos);
		GD.Print(blockPosition);

		Godot.Vector3 homePos = blockPosition;
		homePos.Y += 0.5f;

		SoldierHome home = GameGlobals.SoldierHomeScene.Instantiate<SoldierHome>();
		home.Initialize(this, homePos);
		chunk.AddChild(home);

	}

	public void ExpandSoldierSlots(int slotsDelta)
	{
		this.SoldierSlots += slotsDelta;
		if (this.SoldierSlots < 0)
		{
			this.SoldierSlots = 0;
		}
	}

	private void PlaceGoldMine()
	{

		if (this.coins < GameGlobals.GoldMineCost)
		{
			return;
		}
		
		List<Ore> ores = this.world.GetChunkOres(this.GlobalPosition);
		if (ores == null) return;
		if (ores.Count < 0) return;



		Ore selectedOre = ores[0];

		// select closest ore
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


	

		GoldMine mine = GameGlobals.GoldMineScene.Instantiate<GoldMine>();
		mine.Initialize(this, selectedOre.GlobalPosition);

		selectedOre.AddChild(mine);

		this.coins -= GameGlobals.GoldMineCost;
		
		
	}


	
}

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

	public bool isPlacingHome = false;

	[Export]
	public World world = null;

	Movement movement;

	[Export]
	SoldierManager soldierManager;

	[Export]
	SoldierHomePlacer homePlacer;


	private int coins = 0;

	public int SoldierSlots = 2;
	public override void _EnterTree()
	{
		GlobalPosition = GameGlobals.PlayerStartPos;
		soldierManager.Initialize(this);
		this.TurnOffHomePlacer();
	}
	public override void _Ready()
	{
		
		this.character = (MeshInstance3D)GetNode("Character");
		this.characterCollider = (CollisionShape3D)GetNode("CharacterCollider");
		this.camera = (Camera)GetNode("Camera");
		movement = new MouseGuidedMovement(this);
		
	}

	public int GetCoins()
	{
		
		return coins;
	}

	public void AddCoins(int coinsToAdd)
	{
		this.coins += coinsToAdd;

	}

	private void AddHome(Godot.Vector3 pos)
	{
		SoldierHome home = GameGlobals.SoldierHomeScene.Instantiate<SoldierHome>();
		home.Initialize(this,pos);
		Chunk chunk = this.world.GetChunkAtPos(pos);
		chunk.AddChild(home);
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseMotion eventMouseMotion)
		{
			movement.HandleInputEvent(inputEvent);
		}
		if (inputEvent is InputEventMouseButton inputEventMouse)
		{
			if (inputEventMouse.IsPressed() && isPlacingHome == true)
			{
				
				PhysicsShapeQueryParameters3D parameters3D = new PhysicsShapeQueryParameters3D();
				parameters3D.Shape = this.homePlacer.GetNode<CollisionShape3D>("CollisionShape3D").Shape;

				Transform3D transform = this.homePlacer.GlobalTransform;
				parameters3D.Transform = transform;
				parameters3D.CollisionMask = 1;
				

				var spaceState = GetWorld3D().DirectSpaceState;
				var results = spaceState.IntersectShape(parameters3D, 2);
				if (results.Count <= 1)
				{
					AddHome( this.homePlacer.GlobalPosition);
					TurnOffHomePlacer();
				}
				
				
			}
		}
	}

	private void TurnOffHomePlacer()
	{
		if (!isPlacingHome) return;
		this.homePlacer.Visible = false;
		this.homePlacer.ProcessMode = ProcessModeEnum.Disabled;
		this.isPlacingHome = false;
	
	}
	private void TurnOnHomePlacer()
	{
		if (isPlacingHome) return;
		this.homePlacer.Visible = true;
		this.homePlacer.ProcessMode = ProcessModeEnum.Inherit;
		this.isPlacingHome = true;

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
			TurnOnHomePlacer();
			
		}
		if (Input.IsActionPressed("SetSoldierHome"))
		{
			this.HandleSoldierHomePlacing();
		}
		else
		{
			TurnOffHomePlacer();
		}

	
		MoveAndSlide();
		soldierManager.Update((float)delta, this.characterCollider.Rotation);
		
	}

	private void HandleSoldierHomePlacing()
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
		hitPos.Y -= 0.01f; // so that it would point to actual block later 
	

		Chunk chunk = this.world.GetChunkAtPos(hitPos);

		if (chunk.GetTileAtPos((Godot.Vector3I)hitPos) == null)
		{
			return;
		}
		
	
		this.homePlacer.GlobalPosition = hitPos;	
		


		

		

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
		
		List<Ore> ores = this.world.GetChunkOres((Godot.Vector3I)this.GlobalPosition);
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

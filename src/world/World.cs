
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;



public partial class World : Node3D
{

	public static int WorldWidth = 500;
	public Godot.Vector3I MaxWorldTopLeftGlobal = new Godot.Vector3I(-WorldWidth / 2,0,-WorldWidth / 2);
	public Godot.Vector3I MaxWorldBottomRightGlobal = new Godot.Vector3I(WorldWidth / 2,0,WorldWidth / 2);



	private WorldNoise noise = new WorldNoise();
	private Godot.Vector3I WorldPos = GameGlobals.StartWorldMiddle;
	private Random r = new Random();

	private Dictionary<Godot.Vector3I, Chunk> chunks = new Dictionary<Godot.Vector3I, Chunk>();

	[Export]
	OreManager oreManager;

	[Export]
	CoinManager coinManager;

	[Export]
	WorldBaseObjectsManager worldBaseObjectsManager;

	[Export]
	GameEndScreen endScreen;

	public override void _Ready()
	{
		if (this.GlobalPosition.X !=0 || this.GlobalPosition.Y != 0 || this.GlobalPosition.Z != 0) 
			throw new Exception("World not in middle 0,0,0");
	}

	public Dictionary<Godot.Vector3I, Ore> GetChunkOres(Godot.Vector3I chunkPosition)
	{
		return this.oreManager.GetObjectsAtChunkPos(this.GetChunkPositionFromGlobalPos(chunkPosition));
	}
	public Ore GetOreAtExactGlobalPosition(Godot.Vector3I orePos)
	{
		return this.oreManager.GetObjectAtPos(orePos);
	}
	public void Initialize()
	{
		this.oreManager.GenerateObjects();
		this.coinManager.GenerateObjects();
		this.worldBaseObjectsManager.GenerateObjects();
	}

	public bool CheckIfFreeSpace(Godot.Vector3I tilePosition)
	{
		Chunk chunk = this.GetChunkAtPos(tilePosition);
		if (chunk == null)
		{
			return true;
		}
		WorldTile tile = chunk.GetTileAtPos(tilePosition);
		if (tile == null)
		{
			return true;
		}
		if (chunk.CheckIfSpaceOccupied(tilePosition))
		{
			return false;
		}
		return true;
	}
	public bool CheckIfFreeSpace(List<Godot.Vector3I> tilePositions)
	{
		foreach (Godot.Vector3I tilePos in tilePositions)
		{
			if (!CheckIfFreeSpace(tilePos))
			{
				return false;
			}	
		}
		return true;
	}
	
	
	public Chunk GetChunkAtPos(Godot.Vector3 globalPosition)
	{
		return this.GetChunkAtExactPos(this.GetChunkPositionFromGlobalPos(globalPosition));
	}
	public Chunk GetChunkAtExactPos(Godot.Vector3I chunkGlobalPosition)
	{

		return this.chunks.GetValueOrDefault(chunkGlobalPosition);
	}

	public Chunk CreateChunkAtPosition(Godot.Vector3I chunkGlobalPosition)
	{

		if (
			!this.CheckIfValidGlobalPosition(chunkGlobalPosition) ||
			this.GetChunkPositionFromGlobalPos(chunkGlobalPosition) != chunkGlobalPosition
		) return null;
		

		Chunk chunk = GameGlobals.chunkScene.Instantiate<Chunk>();
		chunk.Initialize(chunkGlobalPosition, this);

		return chunk;
	}

	public WorldTile GetTileAtExactGlobalPosition(Godot.Vector3I exactTileGlobalPosition)
	{
		Chunk chunk = this.GetChunkAtPos(exactTileGlobalPosition);
		if (chunk == null)
		{
			return null;
		}
		WorldTile tile = chunk.GetTileAtPos((Godot.Vector3I)chunk.ConvertToLocalPosition(exactTileGlobalPosition));
		if (tile == null)
		{
			return null;
		}
		return tile;
	}

	
	public WorldTile GetTileAtGlobalPosition(Godot.Vector3 globalPosition)
	{
		
		return this.GetTileAtExactGlobalPosition(this.GetTilePosition(globalPosition)); 
	}


	public bool UpdateChunkAtPosition(Godot.Vector3I chunkGlobalPosition, Chunk chunk)
	{

		if (
			!this.CheckIfValidGlobalPosition(chunkGlobalPosition) ||
			this.GetChunkPositionFromGlobalPos(chunkGlobalPosition) != chunkGlobalPosition
		) return false;
	
		this.chunks[chunkGlobalPosition] = chunk;
		return true;
		
	}

	public List<Godot.Vector3I> GetAvailableChunkPositions()
	{
		return this.chunks.Keys.ToList();
	}
	

	public Godot.Vector3 getWorldPos()
	{
		return this.WorldPos;
	}

	private float getNoiseValue(float x, float y)
	{
		return noise.GetValue(x,y);
	}

	public int getBlockHeightAtPos(float x, float z)
	{
		float y = getNoiseValue(x,z) * 15f;
		// now y is a float which we don't like for our world so we put it in 0-1-2-3..-50range for tiling
		return (int)y;

		// return 5;
	}

	public Godot.Vector3I GetRandomBlockPosInWorld()
	{
		int x = r.Next((int)this.MaxWorldTopLeftGlobal.X + 1, (int)this.MaxWorldBottomRightGlobal.X - 1);
		int z = r.Next((int)this.MaxWorldTopLeftGlobal.Z + 1, (int)this.MaxWorldBottomRightGlobal.Z - 1);

		return new Godot.Vector3I(x,this.getBlockHeightAtPos(x,z),z);
	}
	public void GameEnd()
	{
		endScreen.Visible = true;
		GetTree().Paused = true;
	}

	public bool ShowChunkObjects(Chunk chunk)
	{
		this.oreManager.ShowChunkObjects(chunk.chunkPos);
		this.coinManager.ShowChunkObjects(chunk.chunkPos);

		return true;
	}

	public bool HideChunkObjects(Chunk chunk)
	{
		this.oreManager.ShowChunkObjects(chunk.chunkPos);
		this.coinManager.ShowChunkObjects(chunk.chunkPos);

		return true;
	}





	
}


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
	public OreManager oreManager = null;
	public CoinManager coinManager = null;

    public override void _Ready()
    {
        MaxWorldTopLeftGlobal = (Godot.Vector3I)(this.GlobalPosition + new Godot.Vector3I(-WorldWidth / 2,0,-WorldWidth / 2));
    	MaxWorldBottomRightGlobal = (Godot.Vector3I)(this.GlobalPosition + new Godot.Vector3I(WorldWidth / 2,0,WorldWidth / 2));
    }

	public Dictionary<Godot.Vector3I, Ore> GetChunkOres(Godot.Vector3I chunkPosition)
	{
		return this.oreManager.GetOresAtChunkPos(this.GetChunkPositionFromGlobalPos(chunkPosition));
	}
	public Ore GetOreAtExactGlobalPosition(Godot.Vector3I orePos)
	{
		return this.oreManager.GetOreAtPos(orePos);
	}
	public void Initialize()
	{
		this.oreManager = new OreManager(this);
		this.oreManager.GenerateOres();
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


	public bool UpdateChunkAtPosition(Godot.Vector3I chunkWorldPosition, Chunk chunk)
	{
		if (this.CheckIfValidGlobalPosition(chunkPosition))
		{
			this.chunks[chunkPosition] = chunk;
			return true;
		}
		return false;
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




	
}

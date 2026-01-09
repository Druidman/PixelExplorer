
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;



public partial class World : Node3D
{
	private WorldNoise noise = new WorldNoise();
	private Godot.Vector3 WorldPos = GameGlobals.StartWorldMiddle;
	private Random r = new Random();

	private Dictionary<Godot.Vector3, Chunk> chunks = new Dictionary<Godot.Vector3, Chunk>();
	public OreManager oreManager = null;


	public List<Ore> GetChunkOres(Godot.Vector3 position)
	{
		return this.oreManager.GetOresAtChunkPos(this.GetChunkPos(position));
	}
	public void Initialize()
	{
		this.oreManager = new OreManager(this);
		this.oreManager.GenerateOres();
	}

	private Godot.Vector3 GetChunkPositionFromGlobalPos(Godot.Vector3 pos)
	{
		return (Godot.Vector3I)(new Godot.Vector3(pos.X, 0, pos.Z) / (int)GameGlobals.ChunkWidth) * GameGlobals.ChunkWidth;
	}

	public bool CheckIfPosFitsInWorld(Godot.Vector3 pos)
	{
		if (
			pos.X < GameGlobals.MaxWorldTopLeft.X ||
			pos.X > GameGlobals.MaxWorldBottomRight.X ||

			pos.Z < GameGlobals.MaxWorldTopLeft.Z ||
			pos.Z > GameGlobals.MaxWorldBottomRight.Z
		)
		{
			return false;
		}		
		return true;
	}

	public Godot.Vector3 GetChunkPos(Godot.Vector3 position)
	{
		Godot.Vector3 exactLookUpPos = position / GameGlobals.ChunkWidth;
		exactLookUpPos.X = MathF.Round(exactLookUpPos.X);
		exactLookUpPos.Y = MathF.Round(exactLookUpPos.Y);
		exactLookUpPos.Z = MathF.Round(exactLookUpPos.Z);

		exactLookUpPos *= GameGlobals.ChunkWidth;
		return exactLookUpPos;
	}
	public Chunk GetChunkAtPos(Godot.Vector3 position)
	{
		

		return this.GetChunkAtExactPos(this.GetChunkPos(position));
	}
	public Chunk GetChunkAtExactPos(Godot.Vector3 chunkPosition)
	{
		
		return this.chunks.GetValueOrDefault(chunkPosition);
	}


	public bool UpdateChunkAtPos(Godot.Vector3 position, Chunk chunk)
	{
		if (this.CheckIfPosFitsInWorld(position))
		{
			this.chunks[position] = chunk;
			return true;
		}
		return false;
	}

	public List<Godot.Vector3> GetAvailableChunkPositions()
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
	}

	public Godot.Vector3 GetRandomPosInWorld()
	{
		int x = r.Next((int)GameGlobals.MaxWorldTopLeft.X + 1, (int)GameGlobals.MaxWorldBottomRight.X - 1);
		int z = r.Next((int)GameGlobals.MaxWorldTopLeft.Z + 1, (int)GameGlobals.MaxWorldBottomRight.Z - 1);

		return new Godot.Vector3(x,this.getBlockHeightAtPos(x,z),z);
	}

	public void PlaceMine(GoldMine mine, Chunk chunk)
	{
		chunk.AddChild(mine);
	}


	
}

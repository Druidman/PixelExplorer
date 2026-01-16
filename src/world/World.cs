
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;



public partial class World : Node3D
{
	private WorldNoise noise = new WorldNoise();
	private Godot.Vector3I WorldPos = GameGlobals.StartWorldMiddle;
	private Random r = new Random();

	private Dictionary<Godot.Vector3I, Chunk> chunks = new Dictionary<Godot.Vector3I, Chunk>();
	public OreManager oreManager = null;


	public List<Ore> GetChunkOres(Godot.Vector3I chunkPosition)
	{
		return this.oreManager.GetOresAtChunkPos(this.GetChunkPositionFromGlobalPos(chunkPosition));
	}
	public void Initialize()
	{
		this.oreManager = new OreManager(this);
		this.oreManager.GenerateOres();
	}

	public Godot.Vector3I GetChunkPositionFromGlobalPos(Godot.Vector3 globalPosition)
	{
		// The way it works is as follows:
		// 1. we construct int vector so eg. -2.5 = -2, 2.5 = 2
		// 2. then we take given integers and divide them by width of chunk which result in us getting chunk index in every dimention
		// 3. Then Index multiplied by chunkWidth gives us chunkPosition (Godot.Vector3I)
		// You may be wondering why we divide and then multiply?
		// well look at this: let's say chunkWidth = 16
		// then we want to get chunk at position -20,0,-20
		// if we just bumped it to int we would still end up with -20,0,-20 which is not valid chunkPos because it should be -16,0,-16
		// SO -20 / 16 = -1 (in int division thats why we use ints)
		// Then -1 * 16 = -16, So we get proper position
		//
		// Wierd I didn't use ai to explain this XDD

		int x = (int)MathF.Round(globalPosition.X / (float)GameGlobals.ChunkWidth);
		int z = (int)MathF.Round(globalPosition.Z / (float)GameGlobals.ChunkWidth);



		return new Godot.Vector3I(x* GameGlobals.ChunkWidth, this.WorldPos.Y, z* GameGlobals.ChunkWidth);
	}

	public List<Godot.Vector3I> GenShapeTilePositions(Godot.Vector3I originPos, int tilesXZ = 1, int tilesY = 1)
	{
		List<Godot.Vector3I> tilePositions = new List<Vector3I>();	

		Godot.Vector3I shapeTopLeft = originPos - new Godot.Vector3I(GameGlobals.TileWidth * (tilesXZ - 1), GameGlobals.TileWidth * (tilesY - 1), GameGlobals.TileWidth * (tilesXZ - 1));
		Godot.Vector3I shapeBottomRight = originPos + new Godot.Vector3I(GameGlobals.TileWidth * (tilesXZ - 1), GameGlobals.TileWidth * (tilesY - 1), GameGlobals.TileWidth * (tilesXZ - 1));
		
		for (int x = shapeTopLeft.X; x <= shapeBottomRight.X; x += GameGlobals.TileWidth)
		{
			for (int y = shapeTopLeft.Y; y <= shapeBottomRight.Y; y += GameGlobals.TileWidth)
			{
				for (int z = shapeTopLeft.Z; z <= shapeBottomRight.Z; z += GameGlobals.TileWidth)
				{
					tilePositions.Add(new Godot.Vector3I(x,y,z));
				}
			}
			
		}
		return tilePositions;
	}

	public bool CheckIfFreeSpace(List<Godot.Vector3I> tilePositions)
	{
		foreach (Godot.Vector3I tilePos in tilePositions)
		{
			Chunk chunk = this.GetChunkAtPos(tilePos);
			if (chunk == null)
			{
				continue;
			}
			WorldTile tile = chunk.GetTileAtPos(tilePos);
			if (tile != null)
			{
				
				GD.Print("State: ", tile.state);
				GD.Print("Position: ", tile.position);
			}
			if (chunk.CheckIfSpaceOccupied(tilePos))
			{
				return false;
			}
		}
		return true;
	}
	

	public bool CheckIfValidPosition(Godot.Vector3 globalPos)
	{
		if (
			globalPos.X < GameGlobals.MaxWorldTopLeft.X ||
			globalPos.X > GameGlobals.MaxWorldBottomRight.X ||

			globalPos.Z < GameGlobals.MaxWorldTopLeft.Z ||
			globalPos.Z > GameGlobals.MaxWorldBottomRight.Z ||
			globalPos.Y < this.WorldPos.Y
		)
		{
			return false;
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

	public Godot.Vector3I GetTilePosition(Godot.Vector3 globalPos)
	{
		return (Godot.Vector3I)globalPos; // TODO this works just for tileSize = 1
	}
	public WorldTile GetTileAtGlobalPosition(Godot.Vector3 globalPosition)
	{
		
		return this.GetTileAtExactGlobalPosition(this.GetTilePosition(globalPosition)); 
	}


	public bool UpdateChunkAtPosition(Godot.Vector3I chunkPosition, Chunk chunk)
	{
		if (this.CheckIfValidPosition(chunkPosition))
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

	public Godot.Vector3I GetRandomPosInWorld()
	{
		int x = r.Next((int)GameGlobals.MaxWorldTopLeft.X + 1, (int)GameGlobals.MaxWorldBottomRight.X - 1);
		int z = r.Next((int)GameGlobals.MaxWorldTopLeft.Z + 1, (int)GameGlobals.MaxWorldBottomRight.Z - 1);

		return new Godot.Vector3I(x,this.getBlockHeightAtPos(x,z),z);
	}



	
}

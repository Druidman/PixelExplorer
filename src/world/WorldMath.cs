using Godot;
using System;
public partial class World : Node3D
{

	public Godot.Vector3 ConvertToLocalPos(Godot.Vector3 globalPos)
	{
		return globalPos - this.GlobalPosition;
	}
	
	public Godot.Vector3 ConvertToGlobalPos(Godot.Vector3 localPos)
	{
		return localPos + this.GlobalPosition;
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

    public bool CheckIfValidGlobalPosition(Godot.Vector3 globalPos)
	{
		if (
			globalPos.X < this.MaxWorldTopLeftGlobal.X ||
			globalPos.X > this.MaxWorldBottomRightGlobal.X ||

			globalPos.Z < this.MaxWorldTopLeftGlobal.Z ||
			globalPos.Z > this.MaxWorldBottomRightGlobal.Z ||
			globalPos.Y < this.WorldPos.Y
		)
		{
			return false;
		}		
		return true;
	}
	public bool CheckIfValidLocalPosition(Godot.Vector3 position)
	{
		return this.CheckIfValidGlobalPosition(ConvertToGlobalPos(position));
	}

    public Godot.Vector3I GetTilePosition(Godot.Vector3 globalPos)
	{
		int x = (int)MathF.Round(globalPos.X / (float)GameGlobals.TileWidth);
		int y = (int)MathF.Round(globalPos.Y / (float)GameGlobals.TileWidth);
		int z = (int)MathF.Round(globalPos.Z / (float)GameGlobals.TileWidth);

		return new Godot.Vector3I(x,y,z); // TODO this works just for tileSize = 1
	}

}
using Godot;
using System;
public partial class Chunk
{

    public bool CheckIfGlobalPosFits(Godot.Vector3 GlobalPos)
	{
		return CheckIfValidTileIndicies(
			getPlatformGlobalY(GlobalPos.Y),
			getRowGlobalZ(GlobalPos.Z),
			getColGlobalX(GlobalPos.X)
			
		);
		
	}
	public bool CheckIfLocalPosFits(Godot.Vector3 localPos)
	{
		return CheckIfGlobalPosFits(ConvertToGlobalPosition(localPos));
	
	}
    public int getPlatformGlobalY(float y)
	{	

		if (y < 0 || y >= Height) return -1;

		float topLeftBasedPos = y - this.chunkTopLeft.Y;
		

		return (int)MathF.Floor(topLeftBasedPos / (float)GameGlobals.TileWidth);
	}

	public int getRowGlobalZ(float z)
	{	

		float topLeftBasedPos = z - this.chunkTopLeft.Z;

		return (int)MathF.Floor(topLeftBasedPos / (float)GameGlobals.TileWidth);
	}
	public int getColGlobalX(float x)
	{	

		float topLeftBasedPos = x - this.chunkTopLeft.X;

		return (int)MathF.Floor(topLeftBasedPos / (float)GameGlobals.TileWidth);
	}
	public Godot.Vector3 getGlobalPositionOfTile(int platform, int row, int col)
	{
		return ConvertToGlobalPosition( getLocalPositionOfTile(platform, row, col) );
	}

	public Godot.Vector3 getLocalPositionOfTile(int platform, int row, int col)
	{
		return new Godot.Vector3(
			col + 0.5f, 
			platform, 
			row + 0.5f
		) + this.chunkTopLeft - this.chunkPos;
	}
	public Godot.Vector3 ConvertToLocalChunkPos(Godot.Vector3 globalPos)
	{
		return globalPos - this.chunkPos;
	}
	public Godot.Vector3 ConvertToGlobalPosition(Godot.Vector3 localPos)
	{
		return localPos + this.chunkPos;
	}
}
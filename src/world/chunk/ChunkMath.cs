using Godot;
using System;
public partial class Chunk
{

    public bool CheckIfValidGlobalPosition(Godot.Vector3 globalPosition)
	{

		if (
			globalPosition.Y < this.chunkPos.Y ||

			globalPosition.X < this.chunkTopLeft.X ||
			globalPosition.X > this.chunkBottomRight.X ||

			globalPosition.Z < this.chunkTopLeft.Z ||
			globalPosition.Z > this.chunkBottomRight.Z
		)
		{
			return false;
		}

		return true;
	}

	public bool CheckIfValidTileGlobalPosition(Godot.Vector3I globalTilePosition)
	{
		return this.CheckIfValidGlobalPosition(globalTilePosition);
	}


	public bool CheckIfValidLocalPosition(Godot.Vector3 localPosition)
	{
		return CheckIfValidGlobalPosition(
			ConvertToGlobalPosition(localPosition)
		);
	}
	public bool CheckIfValidLocalTilePosition(Godot.Vector3I localTilePosition)
	{
		return CheckIfValidLocalPosition(localTilePosition);
	}
	public Godot.Vector3 ConvertToLocalPosition(Godot.Vector3 globalPos)
	{
		return globalPos - this.chunkPos;
	}
	public Godot.Vector3 ConvertToGlobalPosition(Godot.Vector3 localPos)
	{
		return localPos + this.chunkPos;
	}
}

using System.Collections.Generic;

public partial class Chunk
{
	Dictionary<Godot.Vector3I, WorldTile> tiles = new Dictionary<Godot.Vector3I, WorldTile>();
    private bool UpdateTile(Godot.Vector3I localPosition,  WorldTile tile)
	{	
		if (!CheckIfValidLocalTilePosition(localPosition))
		{
			return false;
		}


		this.tiles[localPosition] = tile;
		return true;
	}
	public WorldTile GetTileAtPos(Godot.Vector3I globalPos)
	{
		return this.tiles.GetValueOrDefault(globalPos);
	}
    
	

}
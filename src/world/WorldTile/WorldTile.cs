public enum WorldTileState
{
    Occupied, Free
}
public enum WorldTileType
{
    BlockTile, WorldBodyTile, BuildingTile, None
}


// state means if something is in this tile for example:
// - we have a `class Home` now home is 1x1x1 this means that only tiles that it would be occupying are:
//      - tile at pos: Home.X, Home.Y, Home.Z
// globalPosition is Vector3I so that we won't have problems with for example 5 trailing zero difference in dictionaries
public class WorldTile
{  
    public WorldTileState state = WorldTileState.Free;
    public Godot.Vector3I position {get; private set;}

    public WorldTileType worldTileType = WorldTileType.None;
    public WorldTile(WorldTileState state, Godot.Vector3I position, WorldTileType worldTileType = WorldTileType.None)
    {
        this.state = state;
        this.position = position;
        this.worldTileType = worldTileType;
    }
}
using Godot;
using System.Collections.Generic;

public abstract partial class WorldObject : StaticBody3D
{

    protected World world = null;
    public Godot.Vector3 globalPos;
    public override void _EnterTree()
    {
        List<Godot.Vector3> tilesToOccupy = this.GetTiles();
        foreach (Godot.Vector3 localPos in tilesToOccupy)
        {
            Godot.Vector3 pos = localPos + this.globalPos;
            Chunk chunk = this.world.GetChunkAtPos(pos);
            if (chunk == null)
            {
                continue;
            }

            chunk.UpdateTile(
                chunk.ConvertToLocalPosition(this.world.GetTilePosition(pos)), 
                new WorldTile(WorldTileState.Occupied, this.world.GetTilePosition(pos))
            );
            GD.Print("TIle occupied!");

            
        }
    }
    public override void _ExitTree()
    {
        List<Godot.Vector3> tilesOccupied = this.GetTiles();
        foreach (Godot.Vector3 localPos in tilesOccupied)
        {
            Godot.Vector3 pos = localPos + this.globalPos;
            Chunk chunk = this.world.GetChunkAtPos(pos);
            if (chunk == null)
            {
                continue;
            }

            chunk.UpdateTile(
                chunk.ConvertToLocalPosition(this.world.GetTilePosition(pos)),
                new WorldTile(WorldTileState.Free, this.world.GetTilePosition(pos))
            );

            
        }
    }

    public abstract List<Godot.Vector3> GetTiles();
}





























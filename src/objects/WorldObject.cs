using Godot;
using System.Collections.Generic;
public interface IWorldObject
{
    public Godot.Vector3 GlobalPosition {get; set;}
    public Godot.Vector3 PositionOffset {get;}

    public Godot.Vector3 GlobalPos {get; set;}

    public List<Godot.Vector3> BaseTiles {get;}
    public List<Godot.Vector3> Tiles {
        get
        {
            List<Godot.Vector3> tiles = new List<Godot.Vector3>(BaseTiles);
            for (int i =0; i< tiles.Count; i++)
            {
                tiles[i] += this.GlobalPosition;
            }
            return tiles;
        }
    }

    public void Initialize(Godot.Vector3 globalPosition)
	{
		this.GlobalPos = globalPosition;
	}
    
}
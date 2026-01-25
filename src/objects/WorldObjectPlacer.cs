using System.Collections.Generic;
using System.Threading;
using Godot;

public abstract partial class WorldObjectPlacer : Node3D, IWorldObject
{
	public abstract List<Godot.Vector3> BaseTiles {get;}
	public abstract Godot.Vector3 PositionOffset {get;}
	public Godot.Vector3 GlobalPos {get; set;}
	public abstract bool PlaceObject(World world, Player player);

	public List<Godot.Vector3> GetTiles()
	{
		return ((IWorldObject)this).Tiles;
	}

	
}

using System.Collections.Generic;
using System.Threading;
using Godot;

public abstract partial class WorldObjectPlacerBase : Node3D
{
	public Godot.Vector3 GlobalPos {get; set;}
	public abstract bool PlaceObject(World world, Player player);

	public abstract List<Godot.Vector3I> GetTiles();
}


public abstract partial class WorldObjectPlacer<T> : WorldObjectPlacerBase, IWorldObject<T> where T : IWorldObjectDimensions<T> 
{
	public abstract override bool PlaceObject(World world, Player player);

	public override List<Godot.Vector3I> GetTiles()
	{
		return ((IWorldObject<T>)this).Tiles;
	}

	
}

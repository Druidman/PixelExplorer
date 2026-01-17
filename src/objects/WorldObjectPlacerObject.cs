using System.Collections.Generic;
using Godot;

public abstract partial class WorldObjectPlacerObject : Node3D
{
	public abstract Godot.Vector3 positionOffset { get; }
	public abstract List<Godot.Vector3> baseOccupiedTiles { get; }

	public List<Godot.Vector3> GetOccupiedTiles()
	{
		List<Godot.Vector3> occupiedTiles = new List<Godot.Vector3>(baseOccupiedTiles);
		for (int i =0; i< occupiedTiles.Count; i++)
		{
			occupiedTiles[i] += this.GlobalPosition;
		}
		return occupiedTiles;
	}

	public abstract bool PlaceObject(World world, Player player);
}

using Godot;
using System.Collections.Generic;

public partial class Ore : WorldObject
{
	
	
	public void Initialize(Godot.Vector3 globalPos, World world)
	{
		this.globalPos = globalPos;
		this.world = world;
	}

	public override void _Ready()
	{
		this.GlobalPosition = globalPos;
	}

	public override List<Godot.Vector3> GetTiles()
	{
		return GameGlobals.OreOccupiedTiles;
	}
}

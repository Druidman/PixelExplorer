using Godot;
using System.Collections.Generic;

public partial class Ore : WorldBody
{
	public override List<Godot.Vector3> BaseTiles {
		get
		{
			return GameGlobals.OreOccupiedTiles;
		}
	}
	public bool containsGoldMine = false;
	public new void Initialize(World world, Godot.Vector3 globalPos)
	{
		base.Initialize(world, globalPos);
	}
}

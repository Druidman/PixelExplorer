using Godot;
using System.Collections.Generic;

public class OreDimensions : IWorldObjectDimensions<OreDimensions>
{
	public static int TilesX => 1;
    public static int TilesY => 1;
    public static int TilesZ => 1;
}
public partial class Ore : WorldBody<OreDimensions>
{
	public bool containsGoldMine = false;
	public new void Initialize(World world, Godot.Vector3 globalPos)
	{
		base.Initialize(world, globalPos);
	}
}

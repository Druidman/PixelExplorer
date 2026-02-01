using System.Collections.Generic;
using Godot;

public partial class OreManager : ObjectManager<Ore>
{
	public override int ObjectsLimit{get; protected set;} = 100;

	protected override void InitializeObject(Ore ore)
	{
		ore.Initialize(world);
	}
	
	public override void GenerateObjects ()
	{
		this.GenerateObjectsRandomlyOnWorldBlocksSurface();
	}


}

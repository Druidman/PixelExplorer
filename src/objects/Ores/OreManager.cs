using System.Collections.Generic;
using Godot;

public class OreManager
{
	public List<Ore> ores = new List<Ore>();
	public World world = null;

	public OreManager(World world) {
		this.world = world;
	}
	public void GenerateOres()
	{
		for (int i = 0; i < 100; i++)
		{
			Godot.Vector3 pos = this.world.GetRandomPosInWorld();
			pos.Y += 1f;
			Ore ore = GameGlobals.GoldOreScene.Instantiate<Ore>();
			ore.Initialize(pos);
			this.ores.Add(ore);
		}
	}
}

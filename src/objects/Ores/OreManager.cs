using System.Collections.Generic;
using Godot;

public class OreManager
{
	private Dictionary<Godot.Vector3I, List<Ore>> ores = new Dictionary<Godot.Vector3I, List<Ore>>();
	public World world = null;

	public OreManager(World world) {
		this.world = world;
	}

	public List<Ore> GetOresAtChunkPos(Godot.Vector3I chunkPosition)
	{
		return this.ores.GetValueOrDefault(chunkPosition);
	}
	public void GenerateOres()
	{
		for (int i = 0; i < 100; i++)
		{
	
			
			
			Godot.Vector3I pos = this.world.GetRandomPosInWorld();
			pos.Y += 1;
	
			
			

			Ore ore = GameGlobals.GoldOreScene.Instantiate<Ore>();
			ore.Initialize(pos, this.world);

			Godot.Vector3I chunkPos = this.world.GetChunkPositionFromGlobalPos(pos);
			if (this.ores.GetValueOrDefault(chunkPos) == null)
			{
				this.ores[chunkPos] = new List<Ore>{ore};
			}
			else
			{
				this.ores[chunkPos].Add(ore);	
			}
			
		}
	}
}

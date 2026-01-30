using System.Collections.Generic;
using Godot;

public partial class OreManager : Node3D
{
	private Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Ore>> ores = new Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Ore>>();

	[Export]
	World world;

	public Dictionary<Godot.Vector3I, Ore> GetOresAtChunkPos(Godot.Vector3I chunkGlobalPosition)
	{
		return this.ores.GetValueOrDefault(chunkGlobalPosition);
	}

	public Godot.Vector3I GetLocalPosition(Godot.Vector3I globalPos)
	{
		return (Godot.Vector3I)(globalPos - this.GlobalPosition);
	}
	public Ore GetOreAtPos(Godot.Vector3I oreGlobalPos)
	{
		return this.ores.GetValueOrDefault(this.world.GetChunkPositionFromGlobalPos(oreGlobalPos))?.GetValueOrDefault(oreGlobalPos);
	}

	private bool CreateOre(Godot.Vector3I globalOrePos)
	{

		if (!this.world.CheckIfValidGlobalPosition(globalOrePos))
		{
			return false;
		}

		Ore ore = GameGlobals.GoldOreScene.Instantiate<Ore>();
		ore.Position = this.GetLocalPosition(globalOrePos);
		ore.Initialize(this.world);
		ore.Visible = false;

		Godot.Vector3I chunkPos = this.world.GetChunkPositionFromGlobalPos(globalOrePos);

		if (!this.ores.ContainsKey(chunkPos))
		{
			this.ores[chunkPos] = new Dictionary<Godot.Vector3I, Ore>();
		}

		this.ores[chunkPos][globalOrePos] = ore;

		AddChild(ore);	

		return true;
		
	}
	public void GenerateOres()
	{
		for (int i = 0; i < 100; i++)
		{
	
			Godot.Vector3I globalPos;

			do
			{
				globalPos = this.world.GetRandomBlockPosInWorld();
				globalPos.Y += 1;
			} while (
				this.ores.GetValueOrDefault(this.world.GetChunkPositionFromGlobalPos(globalPos))?.ContainsKey(globalPos) != false &&
				this.ores.GetValueOrDefault(this.world.GetChunkPositionFromGlobalPos(globalPos))?.ContainsKey(globalPos) != null
			);
		
			CreateOre(globalPos);
			
			
			
		}
	}


	public bool ShowChunkOres(Godot.Vector3I chunkGlobalPosition)
	{	

		Dictionary<Godot.Vector3I, Ore> oresToShow = this.ores.GetValueOrDefault(chunkGlobalPosition);
		if (oresToShow == null) return false;
		
		foreach (Godot.Vector3I orePos in oresToShow.Keys)
		{
			oresToShow[orePos].Visible = true;
		}
		return true;
	}

	public bool HideChunkOres(Godot.Vector3I chunkGlobalPosition)
	{	

		Dictionary<Godot.Vector3I, Ore> oresToHide = this.ores.GetValueOrDefault(chunkGlobalPosition);
		if (oresToHide == null) return false;
		
		foreach (Godot.Vector3I orePos in oresToHide.Keys)
		{
			oresToHide[orePos].Visible = false;
		}
		return true;
	}
}

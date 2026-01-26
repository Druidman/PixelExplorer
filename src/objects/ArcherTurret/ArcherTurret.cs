using Godot;
using System;
using System.Collections.Generic;

public class ArcherTurretDimensions : IWorldObjectDimensions<ArcherTurretDimensions>
{
	public static int TilesX => 3;
	public static int TilesY => 4;
	public static int TilesZ => 3;
}

public partial class ArcherTurret : Building<ArcherTurretDimensions>
{
	protected override void OnEnterSceneTree()
	{
		this.player.archerTowers.Add(this); // TODO, not elegant
	}
	protected override void OnExitSceneTree()
	{
		this.player.archerTowers.Remove(this); // TODO, fix performance somehow
	}

	public new void Initialize(Player player, Godot.Vector3 pos, World world)
	{
		base.Initialize(player, pos, world);
		
	}
	public void OnAreaEntered(Area3D area)
	{
		GD.Print(area);
	}




}

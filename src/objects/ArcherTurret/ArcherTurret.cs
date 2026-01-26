using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ArcherTurretDimensions : IWorldObjectDimensions<ArcherTurretDimensions>
{
	public static int TilesX => 3;
	public static int TilesY => 4;
	public static int TilesZ => 3;
}

public partial class ArcherTurret : Building<ArcherTurretDimensions>
{
	public static readonly float damageDealt = 1f;
	private List<Soldier> soldiersInAttackArea = new List<Soldier>();
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
		if (area is not Soldier)
		{
			return;
		}

		Soldier soldier = (Soldier)area;

		if (!this.soldiersInAttackArea.Contains(soldier)){
			this.soldiersInAttackArea.Add(soldier);
		}
	}
	public void OnAreaExited(Area3D area)
	{
		if (area is not Soldier s)
		{
			return;
		}
		Soldier soldier = (Soldier)area;

		if (this.soldiersInAttackArea.Contains(soldier)){
			this.soldiersInAttackArea.Remove(soldier);
		}
	}

	public void OnAttack()
	{
		Soldier soldier = this.soldiersInAttackArea.ElementAtOrDefault(0);
		if (soldier == null)
		{
			return;
		}
		GD.Print("Attacking soldier");

		soldier.TakeHealth(ArcherTurret.damageDealt);
	}




}

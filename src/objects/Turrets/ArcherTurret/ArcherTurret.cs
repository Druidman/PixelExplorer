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

public partial class ArcherTurret : Turret<ArcherTurretDimensions>
{
	public override float attackDmg {get; protected set;} = 2f;
	protected override void OnEnterSceneTree()
	{
		this.player.archerTowers.Add(this); // TODO, not elegant
	}
	protected override void OnExitSceneTree()
	{
		this.player.archerTowers.Remove(this); // TODO, fix performance somehow
	}

	public override void OnAreaEntered(Area3D area)
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
	public override void OnAreaExited(Area3D area)
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

	public override void OnAttack()
	{
		Soldier soldier = this.soldiersInAttackArea.ElementAtOrDefault(0);
		if (soldier == null)
		{
			return;
		}
		

		soldier.TakeHealth(this.attackDmg);
	}




}

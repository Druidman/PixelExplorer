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
	protected override void OnBuildingEnterTree()
	{
		this.player?.archerTowers.Add(this); // TODO, not elegant
	}
	protected override void OnBuildingExitTree()
	{
		this.player?.archerTowers.Remove(this); // TODO, fix performance somehow
	}

	protected override void OnEnemySoldierEntered(Soldier soldier)
	{

		if (!this.soldiersInAttackArea.Contains(soldier)){
			this.soldiersInAttackArea.Add(soldier);
		}
	}
	protected override void OnEnemySoldierExited(Soldier soldier)
	{

		if (this.soldiersInAttackArea.Contains(soldier)){
			this.soldiersInAttackArea.Remove(soldier);
		}
	}

	public override void OnAttack()
	{
		Soldier soldier = this.soldiersInAttackArea.ElementAtOrDefault(0);
		if (soldier == null)
		{
			// first attack soldiers
			GD.Print(enteredPlayer);
			if (enteredPlayer == null) return;
			enteredPlayer.TakeHealth(this.attackDmg);

			return;
		}
		

		soldier.TakeHealth(this.attackDmg);
	}




}

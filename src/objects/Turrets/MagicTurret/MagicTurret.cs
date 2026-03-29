using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MagicTurretDimensions : IWorldObjectDimensions<MagicTurretDimensions>
{
	
	public static int TilesX => 3;

	
	public static int TilesY => 2;

	
	public static int TilesZ => 3;
}

public partial class MagicTurret : Turret<MagicTurretDimensions>
{

	[Export]
	PackedScene turretBullet;

	

	public override float attackDmg {get; protected set;} = 2f;
	protected override void OnBuildingEnterTree()
	{
		this.player?.magicTurrets.Add(this); // TODO, not elegant
	}
	protected override void OnBuildingExitTree()
	{
		this.player?.magicTurrets.Remove(this); // TODO, fix performance somehow
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
			if (enteredPlayer == null) return;

			MagicTurretBullet bulletP = this.turretBullet.Instantiate<MagicTurretBullet>();
			bulletP.Instantiate(enteredPlayer.GlobalPosition, this.GlobalPosition, this.attackDmg);
			AddChild(bulletP);

			return;
		}
		
		MagicTurretBullet bullet = this.turretBullet.Instantiate<MagicTurretBullet>();
		bullet.Instantiate(soldier.GlobalPosition, this.GlobalPosition, this.attackDmg);
		AddChild(bullet);
	}




}

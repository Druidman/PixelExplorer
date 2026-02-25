using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ObjectiveC;


public enum ObjectiveType
{
	Home,
	ArcherTurret,
	MagicTurret,
	GoldMine,
	None
}
public partial class EnemyManager : Godot.Node3D
{

	[Export]
	World world;

	List<Soldier> soldiers = new List<Soldier>();

  IBuilding prevObj = null;
  public void onSpawnEnemy()
  {
		if (soldiers.Count >= 300) return;

		Soldier soldier = GameGlobals.soldierScene.Instantiate<Soldier>();
		soldier.Initialize(null, new Godot.Vector3(0,5,0), this.world, this.world.GetRandomBlockPosInWorld() + new Godot.Vector3(0,5,0), () =>
		{
			this.soldiers.Remove(soldier);
		});

		this.soldiers.Add(soldier);
		
		AddChild(soldier);
		
  }
  public override void _Process(double delta)
  {
		ObjectiveType objType = this.world.GetBuildingTypeToAttackByEnemy();
		IBuilding objective = null;
		switch (objType)
		{
			
			case ObjectiveType.Home:
				objective = this.world.GetPlayerHomeToAttackByEnemy();
				break;
			
			case ObjectiveType.ArcherTurret:
				objective = this.world.GetPlayerArcherTurretToAttackByEnemy();
				break;
			case ObjectiveType.MagicTurret:
				objective = this.world.GetPlayerMagicTurretToAttackByEnemy();
				break;
			default:
				break;
		}
		
		foreach (Soldier soldier in this.soldiers)
	{
	  if (soldier.destroyObjective == null && objective != null)
	  {
	  if (objective.healthPoints > 0)
	  {
		soldier.destroyObjective = objective;      
	  }
		  
	  }
	else
	{
	  if (!IsInstanceValid(soldier.destroyObjective as Node3D))
	  { 
		soldier.destroyObjective = null;
	  } 
	}
	  

	  soldier.Tick((float)delta, this.Rotation);
	}
	prevObj = objective;
  }
}

using Godot;
using System.Collections.Generic;




public abstract partial class Turret<T> : Building<T> where T : IWorldObjectDimensions<T>
{
	public abstract float attackDmg {get; protected set;}
	protected List<Soldier> soldiersInAttackArea = new List<Soldier>();

    public new void Initialize(Player player, Godot.Vector3 pos, World world)
	{
		base.Initialize(player, pos, world);
		
	}
	protected abstract override void OnEnterSceneTree();
	
	protected abstract override void OnExitSceneTree();


	private bool CheckIfIsEnemySoldier(Area3D area)
	{
		if (area is not Soldier soldier)
		{
			return false;
		}

		if (soldier.player == this.player) return false;
		return true;
	}

	protected abstract void OnEnemySoldierEntered(Soldier soldier);
	protected abstract void OnEnemySoldierExited(Soldier soldier);
	public void OnAreaEntered(Area3D area)
	{
		if (this.CheckIfIsEnemySoldier(area))
		{
			this.OnEnemySoldierEntered((Soldier)area);
		}

	}
	public void OnAreaExited(Area3D area)
	{
		if (this.CheckIfIsEnemySoldier(area))
		{
			this.OnEnemySoldierExited((Soldier)area);
		}
	}
    public abstract void OnAttack();

}

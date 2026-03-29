using Godot;
using System.Collections.Generic;




public abstract partial class Turret<T> : Building<T> where T : IWorldObjectDimensions<T>
{
	public abstract float attackDmg {get; protected set;}
	protected List<Soldier> soldiersInAttackArea = new List<Soldier>();
	protected Player enteredPlayer = null;

  public new void Initialize(Player player, World world)
	{
		base.Initialize(player, world);
		
	}
	protected abstract override void OnBuildingEnterTree();
	
	protected abstract override void OnBuildingExitTree();


	private bool CheckIfIsEnemySoldier(Area3D area)
	{
		if (area is not Soldier soldier)
		{
			return false;
		}
		if (this.player == null) return true;

		if (soldier.player == this.player) return false;
		return true;
	}


	protected abstract void OnEnemySoldierEntered(Soldier soldier);
	protected abstract void OnEnemySoldierExited(Soldier soldier);

	public void onBodyEntered(Node3D body)
	{
		GD.Print("whatever");
		if (body is Player player){
			GD.Print("hola");
			if (player == this.player) return;
			GD.Print("yeeeeeee");
			this.enteredPlayer = player;
		}

	}

	public void onBodyExited(Node3D body)
	{
		GD.Print("sema");
		if (body is Player player){
			GD.Print("siiiiiiiiiema");
			this.enteredPlayer = null;
		}
	}
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

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

	public abstract void OnAreaEntered(Area3D area);
	public abstract void OnAreaExited(Area3D area);

}

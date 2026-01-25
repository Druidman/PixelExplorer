using Godot;
using System;
using System.Collections.Generic;

public partial class SoldierHome : Building
{

	public override List<Godot.Vector3> BaseTiles
	{
		get
		{
			return GameGlobals.SoldierHomeOccupiedTiles;
		}
	}
	public override Godot.Vector3 PositionOffset
	{
		get
		{
			return GameGlobals.soldierHomePositionOffset;
		}
	}
	bool canRemoveSlots = false;


	
	protected override void OnEnterSceneTree()
	{
		this.player.houses.Add(this); // TODO, not elegant
		if (this.player != null) this.player.ExpandSoldierSlots(10);
		this.canRemoveSlots = true;
	}
	protected override void OnExitSceneTree()
	{
		this.player.houses.Remove(this); // TODO, fix performance somehow
		if (this.player != null && canRemoveSlots) this.player.ExpandSoldierSlots(-10);
	}

	public new void Initialize(Player player, Godot.Vector3 pos, World world)
	{
		base.Initialize(player, pos, world);
		
	}
	protected override void OnDestroy()
	{
		// TODO
		// for now nothing because we take back slots when we leave the tree
	}




}

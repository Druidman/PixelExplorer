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
		if (this.player != null) this.player.ExpandSoldierSlots(10);
		this.canRemoveSlots = true;
	}
	protected override void OnExitSceneTree()
	{
		if (this.player != null && canRemoveSlots) this.player.ExpandSoldierSlots(-10);
	}


}

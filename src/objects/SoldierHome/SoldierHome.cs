using Godot;
using System;
using System.Collections.Generic;

public partial class SoldierHome : WorldObject
{
	
	Player player = null;

	bool canRemoveSlots = false;

	public override List<Godot.Vector3> GetTiles()
	{
		return GameGlobals.SoldierHomeOccupiedTiles;
	}

	public void Initialize(Player player, Godot.Vector3 pos, World world)
	{
		this.player = player;
		this.globalPos = pos;
		this.world = world;
	}
	public override void _Ready()
	{
		this.GlobalPosition = globalPos;
		if (this.player != null) this.player.ExpandSoldierSlots(10);
		this.canRemoveSlots = true;

		
	}
	public override void _ExitTree()
	{
		if (this.player != null && canRemoveSlots) this.player.ExpandSoldierSlots(-10);
	}


}

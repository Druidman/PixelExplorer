using Godot;
using System;
using System.Collections.Generic;

public class SoldierHomeDimensions : IWorldObjectDimensions<SoldierHomeDimensions>
{
	public static int TilesX => 2;
	public static int TilesY => 3;
	public static int TilesZ => 2;
}

public partial class SoldierHome : Building<SoldierHomeDimensions>
{
	bool canRemoveSlots = false;
	protected override void OnBuildingEnterTree()
	{
		this.player.houses.Add(this); // TODO, not elegant
		if (this.player != null) this.player.ExpandSoldierSlots(10);
		this.canRemoveSlots = true;
	}
	protected override void OnBuildingExitTree()
	{
		this.player.houses.Remove(this); // TODO, fix performance somehow
		if (this.player != null && canRemoveSlots) this.player.ExpandSoldierSlots(-10);
	}

	public new void Initialize(Player player, World world)
	{
		base.Initialize(player, world);
		
	}
	protected override void OnDestroy()
	{
		// TODO
		// for now nothing because we take back slots when we leave the tree
	}




}

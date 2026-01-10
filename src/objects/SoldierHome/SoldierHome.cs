using Godot;
using System;

public partial class SoldierHome : StaticBody3D
{
	
	Godot.Vector3 pos;
	Player player = null;

	bool canRemoveSlots = false;

	public void Initialize(Player player, Godot.Vector3 pos)
	{
		this.player = player;
		this.pos = pos;
	}
	public override void _Ready()
	{
		this.GlobalPosition = pos;
		if (this.player != null) this.player.ExpandSoldierSlots(10);
		this.canRemoveSlots = true;

		
	}
	public override void _ExitTree()
	{
		if (this.player != null && canRemoveSlots) this.player.ExpandSoldierSlots(-10);
	}


}

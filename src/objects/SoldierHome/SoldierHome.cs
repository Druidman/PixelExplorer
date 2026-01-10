using Godot;
using System;

public partial class SoldierHome : StaticBody3D
{
	
	Godot.Vector3 pos;
	Player player = null;

	public void Initialize(Player player, Godot.Vector3 pos)
	{
		this.player = player;
		this.pos = pos;
	}
	public override void _Ready()
	{
		this.GlobalPosition = pos;
		this.player.ExpandSoldierSlots(10);
	}
	public override void _ExitTree()
	{
		this.player.ExpandSoldierSlots(-10);
	}


}

using Godot;
using System.Collections.Generic;


public partial class GoldMine : WorldObject
{
	Player player = null;

	public override List<Vector3> GetTiles()
	{
		return GameGlobals.GoldMineOccupiedTiles;
	}
	public override void _Ready()
	{
		this.GlobalPosition = globalPos;
		
	}
	public void Initialize(Player player, Godot.Vector3 position, World world)
	{
		this.player = player;
		this.globalPos = position;
		this.world = world;
	}

	public void OnTimerCall()
	{
		if (this.player == null) return;
		
		
		this.player.AddCoins(10);
	}
}

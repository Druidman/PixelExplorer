using Godot;
using System.Collections.Generic;
using System.Net;


public class GoldMineDimensions : IWorldObjectDimensions<GoldMineDimensions>
{
	public static int TilesX => 2;
	public static int TilesY => 2;
	public static int TilesZ => 2;
}
public partial class GoldMine : Building<GoldMineDimensions>
{
	Ore ore = null;
	public void OnTimerCall()
	{
		if (this.player == null) return;
		
		
		this.player.AddCoins(10);
	}
	public void Initialize(Player player, World world, Ore ore)
	{
		this.Initialize(player,world);
		this.ore = ore;
	}
	protected override void OnDestroy()
	{
		// TODO
		// for now nothing because timer will just stop triggering so player won't receive benefits
		this.ore.containsGoldMine = false;

		
		
	}
}

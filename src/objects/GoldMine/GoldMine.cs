using Godot;
using System.Collections.Generic;
using System.Net;


public partial class GoldMine : Building
{
	Ore ore = null;

	public override Vector3 PositionOffset { 
		get
		{
			return GameGlobals.goldMinePositionOffset;
		}
	}
	public override List<Godot.Vector3> BaseTiles {
		get
		{
			return GameGlobals.GoldMineOccupiedTiles;
		}
	}

	public void OnTimerCall()
	{
		if (this.player == null) return;
		
		
		this.player.AddCoins(10);
	}
	public void Initialize(Player player, Godot.Vector3 pos, World world, Ore ore)
	{
		this.Initialize(player,pos,world);
		this.ore = ore;
	}
	protected override void OnDestroy()
	{
		// TODO
		// for now nothing because timer will just stop triggering so player won't receive benefits
		this.ore.containsGoldMine = false;

		
		
	}
}

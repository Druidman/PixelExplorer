using Godot;
using System.Collections.Generic;


public partial class GoldMine : Building
{
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
	protected override void OnDestroy()
	{
		// TODO
		// for now nothing because timer will just stop triggering so player won't receive benefits
	}
}

using System.Collections.Generic;
using Godot;

public partial class GoldMinePlacer : WorldObjectPlacer
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
	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoinCount() < GameGlobals.GoldMineCost) return false;
		List<Godot.Vector3> listOfOccupiedTiles = this.GetTiles();
		GD.Print("Placing: ", listOfOccupiedTiles.ToString());
		foreach (Godot.Vector3 pos in listOfOccupiedTiles)
		{
			Ore ore = world.GetOreAtExactGlobalPosition((Godot.Vector3I)pos);
			if (ore != null)
			{
				if (ore.containsGoldMine)
				{
					continue;
				}
				
				GoldMine goldMine = GameGlobals.GoldMineScene.Instantiate<GoldMine>();
				
			
				goldMine.Initialize(player, ore.GlobalPosition, world, ore);
				ore.AddChild(goldMine);
		
			

				player.removeCoins(GameGlobals.GoldMineCost);
				ore.containsGoldMine = true;
				return true;
			}
		}
		return false;



		
		
	}
}

using System.Collections.Generic;
using Godot;

public partial class GoldMinePlacer : WorldObjectPlacer<GoldMineDimensions>
{
	
	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoinCount() < GameGlobals.GoldMineCost) return false;
		List<Godot.Vector3I> listOfOccupiedTiles = this.GetTiles();
		GD.Print("Placing: ", listOfOccupiedTiles.ToString());
		foreach (Godot.Vector3I pos in listOfOccupiedTiles)
		{
			Ore ore = world.GetOreAtExactGlobalPosition(pos);
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

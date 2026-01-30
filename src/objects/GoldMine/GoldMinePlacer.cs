using System.Collections.Generic;
using Godot;

public partial class GoldMinePlacer : WorldObjectPlacer<GoldMineDimensions>
{
	
	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoinCount() < GameGlobals.GoldMineCost) return false;
		List<Godot.Vector3I> listOfOccupiedTiles = this.GetTiles();
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
				
			
				goldMine.Initialize(player, world, ore);
				goldMine.Position = world.ConvertToLocalPos(ore.GlobalPosition);
				world.AddChild(goldMine);
		
			

				player.removeCoins(GameGlobals.GoldMineCost);
				ore.containsGoldMine = true;
				return true;
			}
		}
		return false;



		
		
	}
}

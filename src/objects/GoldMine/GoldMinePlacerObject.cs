using System.Collections.Generic;
using Godot;

public partial class GoldMinePlacerObject : WorldObjectPlacerObject
{
	public override Vector3 positionOffset { 
		get
		{
			return GameGlobals.goldMinePositionOffset;
		}
	}
	public override List<Vector3> baseOccupiedTiles { get
		{
			return GameGlobals.GoldMineOccupiedTiles;
		}
	}

	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoins() < GameGlobals.GoldMineCost) return false;
		List<Godot.Vector3> listOfOccupiedTiles = this.GetOccupiedTiles();
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
				goldMine.Initialize(player, ore.GlobalPosition, world);
				player.removeCoins(GameGlobals.GoldMineCost);
				ore.AddChild(goldMine);
                ore.containsGoldMine = true;
				return true;
			}
		}
		return false;



		
		
	}
}

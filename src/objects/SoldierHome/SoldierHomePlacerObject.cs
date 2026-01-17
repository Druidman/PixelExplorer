using System.Collections.Generic;
using Godot;

public partial class SoldierHomePlacerObject : WorldObjectPlacerObject
{
	public override Vector3 positionOffset { 
		get
		{
			return GameGlobals.soldierHomePositionOffset;
		}
	}
	public override List<Vector3> baseOccupiedTiles { get
		{
			return GameGlobals.SoldierHomeOccupiedTiles;
		}
	}

	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoins() < GameGlobals.housePrice) return false;
		if (!world.CheckIfFreeSpace(this.GetOccupiedTiles()))
		{
			return false;
		}
		SoldierHome home = GameGlobals.SoldierHomeScene.Instantiate<SoldierHome>();
		home.Initialize(player, this.GlobalPosition, world);
		player.removeCoins(GameGlobals.housePrice);
		world.AddChild(home);
		return true;
	}
}

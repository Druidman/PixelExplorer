using System.Collections.Generic;
using Godot;

public partial class SoldierHomePlacer : WorldObjectPlacer<SoldierHomeDimensions>
{
	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoinCount() < GameGlobals.housePrice) return false;

		GD.Print(this.GetTiles()[0]);
		if (!world.CheckIfFreeSpace(this.GetTiles()))
		{
			return false;
		}

		SoldierHome home = GameGlobals.SoldierHomeScene.Instantiate<SoldierHome>();
		home.Initialize(player, this.GlobalPosition, world);
		world.AddChild(home);
		player.removeCoins(GameGlobals.housePrice);
		return true;
	}
}

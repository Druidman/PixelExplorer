using System.Collections.Generic;
using Godot;

public partial class MagicTurretPlacer : WorldObjectPlacer<MagicTurretDimensions>
{
	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoinCount() < GameGlobals.magicTurretPrice) return false;

		if (!world.CheckIfFreeSpace(this.GetTiles()))
		{
			return false;
		}

		MagicTurret magicTurret = GameGlobals.MagicTurretScene.Instantiate<MagicTurret>();
		magicTurret.Initialize(player, world);
		magicTurret.Position = world.ConvertToLocalPos(this.GlobalPosition);

		world.AddChild(magicTurret);
		
		player.removeCoins(GameGlobals.magicTurretPrice);
		return true;
	}
}

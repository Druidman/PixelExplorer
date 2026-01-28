using System.Collections.Generic;
using Godot;

public partial class ArcherTurretPlacer : WorldObjectPlacer<ArcherTurretDimensions>
{
	public override bool PlaceObject(World world, Player player)
	{
		if (player.GetCoinCount() < GameGlobals.archerTurretPrice) return false;

		if (!world.CheckIfFreeSpace(this.GetTiles()))
		{
			return false;
		}

		ArcherTurret archerTurret = GameGlobals.ArcherTurretScene.Instantiate<ArcherTurret>();
		archerTurret.Initialize(player, this.GlobalPosition, world);
		world.AddChild(archerTurret);
		player.removeCoins(GameGlobals.archerTurretPrice);
		return true;
	}
}

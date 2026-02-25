using Godot;
using System;
using System.Collections.Generic;

public partial class WorldBaseObjectsManager : Node3D
{

	[Export]
	World world;

	List<MagicTurret> turrets = new List<MagicTurret>();
	public void GenerateObjects()
	{

		// MagicTurret turret = turretScene.InstantiateOrNull<MagicTurret>();
		Godot.Collections.Array<Node> children = GetChildren();
		if (children.Count == 0) throw new Exception("No children set in WorldBaseObjectsManager");

		foreach (Node child in children)
		{
			if (child is not MagicTurret turret) continue;

			turret.FreeTiles();
			turret.Position = new Godot.Vector3(turret.Position.X,this.world.getBlockHeightAtPos(turret.Position.X,turret.Position.Z) + 1,turret.Position.Z); //because world is in middle so no need to do conversion
			turret.OccupyTiles();
			turret.TreeExited += ()=>OnTurretDestroyed(turret);

			turrets.Add(turret);
		}
		

	}

	private void OnTurretDestroyed(MagicTurret turret){
		if (!this.turrets.Contains(turret)) throw new Exception("Something wrong with turrets in worldBaseManager *unexistent* turret was destroyed");

		this.turrets.Remove(turret);
		this.CheckGameState();
	}

	private void CheckGameState()
	{
		if (this.turrets.Count > 0) return;

		this.world.GameEnd();
	}



}

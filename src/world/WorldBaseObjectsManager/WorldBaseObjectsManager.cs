using Godot;
using System;

public partial class WorldBaseObjectsManager : Node3D
{

	[Export]
	World world;

	
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
			turret.TreeExited += OnTurretDestroyed;
		}
		

	}

	private void OnTurretDestroyed(){
		GD.Print("You won!");
	}



}

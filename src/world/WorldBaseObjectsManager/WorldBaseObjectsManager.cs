using Godot;
using System;

public partial class WorldBaseObjectsManager : Node3D
{

	[Export]
	World world;

	[Export]
	PackedScene turretScene;
	public void GenerateObjects()
	{

		MagicTurret turret = turretScene.InstantiateOrNull<MagicTurret>();
		if (turret == null) throw new Exception("No magicturret set in WorldBaseObjectsManager");

		Godot.Vector3I towerPos = new Godot.Vector3I(0,this.world.getBlockHeightAtPos(0,0) + 1,0);	
		turret.Position = towerPos; //because world is in middle so no need to do conversion
		turret.Initialize(null,this.world);
		
		turret.TreeExited += OnTurretDestroyed;

		AddChild(turret);
	}

	private void OnTurretDestroyed(){
		GD.Print("You won!");
	}



}

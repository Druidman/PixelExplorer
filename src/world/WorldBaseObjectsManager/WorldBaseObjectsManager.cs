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
		MagicTurret turret = this.turretScene.InstantiateOrNull<MagicTurret>();
		if (turret == null) throw new Exception("Smth wrong went during turret instantiation in WorldBaseObjectsManager. Probably not magicturret passed as packed scene");
		Godot.Vector3I towerPos = new Godot.Vector3I(0,this.world.getBlockHeightAtPos(0,0) + 1,0);	
		turret.Position = towerPos; //because world is in middle so no need to do conversion
		turret.Initialize(null,this.world);
		AddChild(turret);
	}


}

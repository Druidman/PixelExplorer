using Godot;
using System;

public partial class Game : Node3D
{

	
	[Export]
	public World world = null;

	[Export]
	public Player player = null;

	[Export]
	SfxPlayer coinCollectedSound;
	[Export]
	SfxPlayer buildingPlacedSound;
	[Export]
	SfxPlayer buildingDestroyedSound;
	[Export]
	SfxPlayer punchSound;

	[Export]
	SfxPlayer spawnSound;
	[Export]
	SfxPlayer dieSound;

	[Export]
	public ChunkRenderer chunkRenderer = null;
	public override void _Ready()
	{
		ThreadGuard.Initialize();
		if (world  == null ||  player == null) 
			throw new Exception("Game: player or world not assigned");

		this.world.Initialize();

		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);

		GameGlobals.coinCollectedSound = coinCollectedSound;
		GameGlobals.buildingPlacedSound = buildingPlacedSound;
		GameGlobals.buildingDestroyedSound = buildingDestroyedSound;
		GameGlobals.punchSound = punchSound;
		GameGlobals.spawnSound = spawnSound;
		GameGlobals.dieSound = dieSound;
	}
}

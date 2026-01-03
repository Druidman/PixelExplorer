using Godot;
using System;

public partial class Game : Node3D
{

	
	[Export]
	public World world = null;

	[Export]
	public Player player = null;
	public override void _Ready()
	{
		ThreadGuard.Initialize();
		if (world  == null ||  player == null) 
			throw new Exception("Game: player or world not assigned");

		

		GameGlobals.Initialize(this);


		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);
	}
}

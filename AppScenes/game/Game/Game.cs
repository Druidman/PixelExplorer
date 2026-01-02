using Godot;
using System;

public partial class Game : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);
	}
}

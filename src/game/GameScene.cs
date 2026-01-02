using Godot;
using System;

public partial class GameScene : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent ev)
	{
		// if (ev.IsActionPressed("exit"))
		// {
		// 	GetTree().Paused = !GetTree().Paused;
		// }
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

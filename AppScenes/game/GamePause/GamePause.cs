using Godot;
using System;

public partial class GamePause : Control
{
	// Called when the node enters the scene tree for the first time.
	public void resume()
	{
		Hide();
		GetTree().Paused = false;
		// DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);
	}

	public void pause()
	{
		Show();
		// DisplayServer.MouseSetMode(DisplayServer.MouseMode.Confined);
		GetTree().Paused = true;
	}


	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed("exit") && GetTree().Paused)
		{
			resume();
		}
		else if (ev.IsActionPressed("exit") && !GetTree().Paused)
		{
			pause();
		}
	}
}

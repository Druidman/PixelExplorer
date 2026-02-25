using Godot;
using System;

public partial class GameLooseScreen : Control
{

	public void onRetry()
	{
		GetTree().Paused = false;
	}
	public void onQuit()
	{
		GetTree().Quit();
	}
}

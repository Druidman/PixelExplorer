using Godot;
using System;

public partial class GameEndScreen : Control
{
	public void OnQuit(){
		GetTree().Quit();
	}
}

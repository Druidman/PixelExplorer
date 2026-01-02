using Godot;
using System;

public partial class quitButton : Button
{


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Pressed(){
		GetTree().Quit();
	}
}

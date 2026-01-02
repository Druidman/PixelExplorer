using Godot;
using System;

public partial class SceneSwitcher : Button
{
	// Called when the node enters the scene tree for the first time.

	[Export]
	public PackedScene scene = null;



	public override void _Pressed()
	{
		if (scene == null)
		{
			return;
		}
		GetTree().ChangeSceneToPacked(scene);

	}
   
   
	

}

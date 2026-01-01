using Godot;
using System;

public partial class ReturnButton : Button
{
	// Called when the node enters the scene tree for the first time.

	[Export]
	public string scenePath = "res://scenes/app/mainMenu/main_menu.tscn";
	public override void _Ready()
	{
		Size = new Godot.Vector2(100,50);
		Position = new Godot.Vector2(0,0);
		
	}

	public void _on_pressed()
	{
		GetTree().ChangeSceneToFile(scenePath);
	}
	

}

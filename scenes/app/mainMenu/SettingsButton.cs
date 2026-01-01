using Godot;
using System;

public partial class SettingsButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Size = new Godot.Vector2(100,50);
		Position = (DisplayServer.ScreenGetSize() / 2) + new Godot.Vector2(0,0) - (Size / 2);
		
	}

	public void _on_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/app/settings/settings.tscn");
	}
}

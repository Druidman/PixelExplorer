using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.

	[Export]
	public PackedScene settingsScene = null;

	private Control settingsInstance = null;

	public void on_settings_pressed()
	{

		if (settingsScene == null)
		{
			return;
		}

		if (settingsInstance == null)
		{
			settingsInstance = (Control)settingsScene.Instantiate();
			GetParent().AddChild(settingsInstance);
		}
		
		settingsInstance.Show();
		Hide();
	}
}

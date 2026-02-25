using Godot;
using System;

public partial class MainMenu : Control
{


	[Export]
	public PackedScene gameScene = null;


	[Export]
	private Control settingsPage = null;

	[Export]
	public Control menuPage = null;

	[Export]
	public SpinBox chunkRadiusBox = null;

	public void on_settings_pressed()
	{
		settingsPage.Show();
		menuPage.Hide();
	}

	public void onSettingsSave()
	{
		GameGlobals.chunkRadius = (int)chunkRadiusBox.Value;
		menuPage.Show();
		settingsPage.Hide();
	}

	public void on_start_pressed()
	{
		GetTree().ChangeSceneToPacked(gameScene);
	}

	public void on_exit_pressed()
	{
		GetTree().Quit();
	}
}

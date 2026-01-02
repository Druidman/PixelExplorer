using Godot;
using System;
using System.Collections.Generic;




public enum SceneEnum
{
	None,
	Settings,
	Game,
	MainMenu
}

public partial class SceneSwitcher : Button
{
	// Called when the node enters the scene tree for the first time.

	static private string baseScenePath = "res://AppScenes/";
	

	public Dictionary<SceneEnum, string> AvailableScenes = new Dictionary<SceneEnum, string>
	{
		{SceneEnum.None, ""},
		{SceneEnum.Settings, baseScenePath + "settings/Settings.tscn"},
		{SceneEnum.Game, baseScenePath + "game/GameScene.tscn"},
		{SceneEnum.MainMenu, baseScenePath + "mainMenu/main_menu.tscn"}
	};
	

	[Export]
	public SceneEnum scene = SceneEnum.None;

	[Signal]
	public delegate void BeforeSceneChangeEventHandler();


	public void on_pressed()
	{

		EmitSignal(SignalName.BeforeSceneChange);

		
		if (scene == SceneEnum.None)
		{
			return;
		}
		GetTree().ChangeSceneToFile(AvailableScenes[scene]);

	}
   
   
	

}

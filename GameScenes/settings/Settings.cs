using Godot;
using System;

public partial class Settings : Control
{
	public void OnReturnPressed()
	{
		Hide();
		Control mainMenu = GetParent().GetNodeOrNull<Control>("MainMenu");
		if (mainMenu != null) mainMenu.Show();
	}
}

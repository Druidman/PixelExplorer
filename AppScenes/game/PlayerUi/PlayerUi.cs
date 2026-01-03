using Godot;
using System;

public partial class PlayerUi : Control
{
	// Called when the node enters the scene tree for the first time
	[Export]
	public Label Coins;

	[Export]
	public Player player;
	
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Coins.Text = player.GetCoins().ToString();
	}
}

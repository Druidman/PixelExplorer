using Godot;
using System;

public partial class PlayerUi : Control
{
	// Called when the node enters the scene tree for the first time
	[Export]
	public Label Coins;

	[Export]
	public Label Soldiers;

	[Export]
	public Label MaxSoldiers;

	[Export]
	public ProgressBar speed;

	[Export]
	public Player player;
	
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Coins.Text = player.GetCoinCount().ToString();
		Soldiers.Text = player.GetSoldierCount().ToString();
		MaxSoldiers.Text = player.GetMaxSoldierCount().ToString();
		speed.Value = (this.player.speed / GameGlobals.PlayerSpeed) * 100;
	}
}

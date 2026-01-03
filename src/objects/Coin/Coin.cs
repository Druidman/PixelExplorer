using Godot;
using System;

public partial class Coin : Area3D
{
	public Action removeCallback = ()=>{};


	public void collected(Player player)
	{
		player.AddCoins(1);
		removeCallback();
	}
	public void OnBodyEntered(Player player)
	{
		collected(player);
	}

	
}

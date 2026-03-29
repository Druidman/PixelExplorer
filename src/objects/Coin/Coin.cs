using Godot;
using System;

public partial class Coin : Area3D
{

	private Action removeCallback = ()=>{};




	public void Initialize(Action removeCallback)
	{
		this.removeCallback = removeCallback;
	}
	public void collected(Player player)
	{
		player.AddCoins(1);
		if (GameGlobals.coinCollectedSound != null) GameGlobals.coinCollectedSound.Play();
		
		removeCallback();
	}
	public void OnBodyEntered(Player player)
	{
		collected(player);
	}

	
}


using System;
using System.Collections.Generic;
using Godot;

public abstract class CoinManager
{

	protected Dictionary<Godot.Vector3, Coin> coins = new Dictionary<Godot.Vector3, Coin>();
	protected Random random = new Random();
	protected Node3D parentNode;

	protected CoinManager(Node3D parentNode)
	{
		this.parentNode = parentNode;
	}
	public void SpawnCoin(Godot.Vector3 pos)
	{
		if (!ValidatePos(pos))
		{
			GD.Print("Does not fit!");
			return;
		}
		GD.Print("Passed");
		
		Coin coin = GameGlobals.coinScene.Instantiate<Coin>();
		coin.Position = pos;
		coin.removeCallback = ()=>this.RemoveCoin(pos);
		GD.Print("Adding coin");
		this.coins[pos] = coin;
		this.parentNode.CallDeferred(Node3D.MethodName.AddChild, coin);
	}
	public void RemoveCoin(Godot.Vector3 pos)
	{
		if (!ValidatePos(pos))
		{
			return;
		}
		this.parentNode.CallDeferred(Node3D.MethodName.RemoveChild, this.coins[pos]);
		this.coins[pos].QueueFree();
		this.coins.Remove(pos);
		UpdateCoins();
	}

	public virtual bool ValidatePos(Godot.Vector3 pos){return true;}

	public abstract void UpdateCoins();
}

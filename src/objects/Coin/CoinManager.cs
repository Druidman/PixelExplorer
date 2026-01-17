
using System;
using System.Collections.Generic;
using Godot;

public abstract class CoinManager
{

	protected Dictionary<Godot.Vector3I, Coin> coins = new Dictionary<Godot.Vector3I, Coin>();
	protected Random random = new Random();
	protected Node3D parentNode;

	protected CoinManager(Node3D parentNode)
	{
		this.parentNode = parentNode;
	}
	public void SpawnCoin(Godot.Vector3I localPos)
	{
		if (!ValidatePos(localPos))
		{

			return;
		}
	
		
		Coin coin = GameGlobals.coinScene.Instantiate<Coin>();
		coin.Position = localPos;
		coin.removeCallback = ()=>this.RemoveCoin(localPos);
		
		this.coins[localPos] = coin;
		this.parentNode.CallDeferred(Node3D.MethodName.AddChild, coin);
	}
	public void RemoveCoin(Godot.Vector3I localPos)
	{
		if (!ValidatePos(localPos))
		{
			return;
		}
		this.parentNode.CallDeferred(Node3D.MethodName.RemoveChild, this.coins[localPos]);
		this.coins[localPos].QueueFree();
		this.coins.Remove(localPos);
		UpdateCoins();
	}

	public virtual bool ValidatePos(Godot.Vector3 localPos){return true;}

	public abstract void UpdateCoins();
}

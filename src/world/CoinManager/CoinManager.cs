
using System;
using System.Collections.Generic;
using Godot;

public partial class CoinManager : Node3D
{

	protected Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Coin>> coins = new Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Coin>>();
	protected Dictionary<Godot.Vector3I, Coin> coinsItself = new Dictionary<Godot.Vector3I, Coin>();

	[Export]
	World world;

	public Coin GetCoinAtGlobalPos(Godot.Vector3I globalPos)
	{
		return this.coinsItself.GetValueOrDefault(globalPos);
	}
	public bool CreateCoin(Godot.Vector3I globalPos)
	{
		if (!this.world.CheckIfValidGlobalPosition(globalPos))
		{
			return false;
		}
	
		
		Coin coin = GameGlobals.coinScene.Instantiate<Coin>();
		coin.Position = this.GetLocalPosition(globalPos);
		coin.Initialize(()=>this.RemoveCoin(globalPos));
		coin.Visible = false;
		

		Godot.Vector3I chunkGlobalPos = this.world.GetChunkPositionFromGlobalPos(globalPos);
		if (!this.coins.ContainsKey(chunkGlobalPos))
		{
			this.coins[chunkGlobalPos] = new Dictionary<Vector3I, Coin>();
		}
		
		this.coins[chunkGlobalPos][globalPos] = coin;
		this.coinsItself[globalPos] = coin;

		AddChild(coin);
		
		return true;
	}

	public Godot.Vector3I GetLocalPosition(Godot.Vector3I globalPos)
	{
		return (Godot.Vector3I)(globalPos - this.GlobalPosition);
	}
	public bool RemoveCoin(Godot.Vector3I globalPos)
	{
		if (!this.world.CheckIfValidGlobalPosition(globalPos))
		{
			return false;
		}
		GD.Print(globalPos);
		this.GetCoinAtGlobalPos(globalPos)?.QueueFree(); // free node

		this.coins.GetValueOrDefault(this.world.GetChunkPositionFromGlobalPos(globalPos)).Remove(globalPos); // actual remove
		this.coinsItself.Remove(globalPos); // actual remove


		UpdateCoins(); // to make new coin
		return true;
	}

	

	public void UpdateCoins()
	{
		GD.Print(this.coinsItself.Count);
		if (this.coinsItself.Count >= GameGlobals.CoinLimit)
		{
			return;
		}

		int amountToGen = GameGlobals.CoinLimit - this.coinsItself.Count;


		for (int i=0; i < amountToGen; i++)
		{
			Godot.Vector3I globalPos;
			do
			{
				globalPos = this.world.GetRandomBlockPosInWorld();
				globalPos.Y += 1;
				
			} while ( this.coinsItself.ContainsKey(globalPos) );
			
		
			if (!CreateCoin(globalPos))
			{
				GD.Print("Smth wrong");	
			}
			
		}
		
	}

	public bool ShowChunkCoins(Godot.Vector3I chunkGlobalPosition)
	{	

		Dictionary<Godot.Vector3I, Coin> coinsToShow = this.coins.GetValueOrDefault(chunkGlobalPosition);
		if (coinsToShow == null) return false;
		
		foreach (Godot.Vector3I coinPos in coinsToShow.Keys)
		{
			coinsToShow[coinPos].Visible = true;
		}
		return true;
	}

	public bool HideChunkCoins(Godot.Vector3I chunkGlobalPosition)
	{	

		Dictionary<Godot.Vector3I, Coin> coinsToHide = this.coins.GetValueOrDefault(chunkGlobalPosition);
		if (coinsToHide == null) return false;
		
		foreach (Godot.Vector3I coinPos in coinsToHide.Keys)
		{
			coinsToHide[coinPos].Visible = false;
		}
		return true;
	}
}

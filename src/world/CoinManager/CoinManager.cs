
using System;
using System.Collections.Generic;
using Godot;

public class CoinManager
{

	protected Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Coin>> coins = new Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Coin>>();
	protected Random random = new Random();
	public World world;

	protected CoinManager(World world)
	{
		this.world = world;
	}

	public Coin GetCoinAtGlobalPos(Godot.Vector3I globalPos)
	{
		return this.coins.GetValueOrDefault(this.world.GetChunkPositionFromGlobalPos(globalPos)).GetValueOrDefault(globalPos);
	}
	public bool CreateCoin(Godot.Vector3I globalPos)
	{
		if (!ValidatePos(globalPos))
		{
			return false;
		}
	
		
		Coin coin = GameGlobals.coinScene.Instantiate<Coin>();
		coin.Position = this.GetCoinEngineLocalPosition(globalPos);
		coin.Initialize(()=>this.RemoveCoin(globalPos));
		

		Godot.Vector3I chunkGlobalPos = this.world.GetChunkPositionFromGlobalPos(globalPos);
		if (!this.coins.ContainsKey(chunkGlobalPos))
		{
			this.coins[chunkGlobalPos] = new Dictionary<Vector3I, Coin>();
		}
		
		this.coins[chunkGlobalPos][globalPos] = coin;
		
		

		
		
		return true;
	}

	public Godot.Vector3I GetCoinEngineLocalPosition(Godot.Vector3I globalPos)
	{
		return globalPos - this.world.GetChunkPositionFromGlobalPos(globalPos);
	}
	public bool RemoveCoin(Godot.Vector3I globalPos)
	{
		if (!ValidatePos(globalPos))
		{
			return false;
		}
		
		this.GetCoinAtGlobalPos(globalPos).QueueFree(); // free node

		this.coins.GetValueOrDefault(this.world.GetChunkPositionFromGlobalPos(globalPos)).Remove(globalPos); // actual remove


		UpdateCoins(); // to make new coin
		return true;
	}

	public bool ValidatePos(Godot.Vector3 globalPos){
		return this.world.CheckIfValidPosition(globalPos);
	}

	public void UpdateCoins()
	{
		if (this.coins.Count >= GameGlobals.ChunkCoinLimit)
		{
			return;
		}

		for (int i=0; i < GameGlobals.ChunkCoinLimit - this.coins.Count; i++)
		{
			Godot.Vector3I globalPos;

			do
			{
				globalPos = this.world.GetRandomBlockPosInWorld();
			} while (this.coins.ContainsKey(globalPos));
		
			CreateCoin(globalPos);
		}
	}
}

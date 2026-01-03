using System;
using System.Collections.Generic;
using Godot;

public class CoinManager
{

	Dictionary<Godot.Vector3, Coin> coins = new Dictionary<Godot.Vector3, Coin>();
	PackedScene coinScene;
	public CoinManager()
	{
		this.coinScene = GD.Load<PackedScene>("res://src/objects/Coin/Coin.tscn");

		
	}


	public void SpawnCoin(Godot.Vector3 pos)
	{
		Coin coin = coinScene.Instantiate<Coin>();
		coin.Position = pos;
		coin.removeCallback = ()=>this.RemoveCoin(pos);

		this.coins[pos] = coin;
		GameGlobals.game.world.CallDeferred(Node3D.MethodName.AddChild, coin);
	}
	public void RemoveCoin(Godot.Vector3 pos)
	{
		GameGlobals.game.world.CallDeferred(Node3D.MethodName.RemoveChild, this.coins[pos]);
		this.coins[pos].QueueFree();
		this.coins.Remove(pos);
	}

	public void UpdateCoins()
	{
		if (this.coins.Count >= GameGlobals.WorldCoinsLimit)
		{
			return;
		}
		// GD.Print("Hello from: ", this);
		
		Random r = new Random();
		int c = this.coins.Count;
		for (int i = 0; i<GameGlobals.WorldCoinsLimit - c; i++)
		{   
			float x = GameGlobals.game.player.GlobalPosition.X + r.Next(-5,5);
			float z = GameGlobals.game.player.GlobalPosition.Z + r.Next(-5,5);
			Godot.Vector3 pos = new Godot.Vector3(x, GameGlobals.game.world.getBlockHeightAtPos((int)x,(int)z) + 2, z);

            if (!this.coins.ContainsKey(pos))
            {
                SpawnCoin(pos);    
            }
			

		}
	}
	

}

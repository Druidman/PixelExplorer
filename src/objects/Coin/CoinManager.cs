using System.Collections.Generic;
using Godot;

public class CoinManager
{

    Dictionary<Godot.Vector3, Coin> coins = new Dictionary<Godot.Vector3, Coin>();
    World world = null;

    PackedScene coinScene;
    public CoinManager(World world)
    {
        this.world = world;

        this.coinScene = GD.Load<PackedScene>("res://src/objects/Coin/Coin.tscn");
    }


    public void SpawnCoin(Godot.Vector3 pos)
    {
        Coin coin = coinScene.Instantiate<Coin>();
        coin.Position = pos;

        this.coins[pos] = coin;
        this.world.AddChild(coin);
    }
    public void RemoveCoin(Godot.Vector3 pos)
    {
        this.world.RemoveChild(this.coins[pos]);
        this.coins[pos].QueueFree();
    }


    public void UpdateCoins()
    {
        if (this.coins.Count >= GameGlobals.WorldCoinsLimit)
        {
            return;
        }

        for (int i = 0; i<GameGlobals.WorldCoinsLimit - this.coins.Count; i++)
        {
            Godot.Vector3 pos = new Godot.Vector3(this.world.WorldPos.X,this.world.WorldPos.Y,this.world.WorldPos.Z);

        }
    }
    

}
using Godot;

public class ChunkCoinManager : CoinManager
{
	Chunk chunk;

	
	public ChunkCoinManager(Chunk chunk) : base(chunk)
	{
		this.chunk = chunk;
	}

	public override bool ValidatePos(Godot.Vector3 pos)
	{
		return this.chunk.CheckIfPosFits(pos);
	}

	public override void UpdateCoins()
	{
		if (this.coins.Count >= GameGlobals.ChunkCoinLimit)
		{
			return;
		}

		for (int i=0; i < GameGlobals.ChunkCoinLimit - this.coins.Count; i++)
		{
			Godot.Vector3 pos;

			do
			{
				int x = this.random.Next(
						-GameGlobals.ChunkWidth/2,
						GameGlobals.ChunkWidth/2
					);
				int z = this.random.Next(
						-GameGlobals.ChunkWidth/2,
						GameGlobals.ChunkWidth/2
					);
				
				
				pos = this.chunk.chunkPos + 
				new Godot.Vector3(
					x,
					GameGlobals.world.getBlockHeightAtPos(x,z),
					z
				);
				

			} while (this.coins.ContainsKey(pos));
			
			GD.Print("Tile Pos: ", pos);
			SpawnCoin(pos);
		}
		
		

	}
}

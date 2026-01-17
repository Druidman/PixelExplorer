using Godot;

public class ChunkCoinManager : CoinManager
{
	Chunk chunk;

	
	public ChunkCoinManager(Chunk chunk) : base(chunk)
	{
		this.chunk = chunk;
	}

	public override bool ValidatePos(Godot.Vector3 localPos)
	{
		return this.chunk.CheckIfValidLocalPosition(localPos);
	}

	public override void UpdateCoins()
	{
		if (this.coins.Count >= GameGlobals.ChunkCoinLimit)
		{
			return;
		}

		for (int i=0; i < GameGlobals.ChunkCoinLimit - this.coins.Count; i++)
		{
			Godot.Vector3I localPos;

			do
			{
				int x = this.random.Next(
						(-GameGlobals.ChunkWidth/2),
						(GameGlobals.ChunkWidth/2)
					)  + (int)this.chunk.chunkPos.X ;
				int z = this.random.Next(
						(-GameGlobals.ChunkWidth/2),
						(GameGlobals.ChunkWidth/2)
					) + this.chunk.chunkPos.Z ;

				
				int y = this.chunk.world.getBlockHeightAtPos(x,z) + 1;

				localPos = (Godot.Vector3I)this.chunk.ConvertToLocalPosition(new Godot.Vector3I(x,y,z));
				

			} while (this.coins.ContainsKey(localPos));
		
			SpawnCoin(localPos);
		}
		
		

	}
}

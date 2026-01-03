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
		return this.chunk.CheckIfLocalPosFits(pos);
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
				float x = this.random.Next(
						(-GameGlobals.ChunkWidth/2),
						(GameGlobals.ChunkWidth/2) - 1
					)  + (int)this.chunk.chunkPos.X ;
				float z = this.random.Next(
						(-GameGlobals.ChunkWidth/2),
						(GameGlobals.ChunkWidth/2) - 1 
					) + this.chunk.chunkPos.Z ;

				
				float y = GameGlobals.world.getBlockHeightAtPos(x,z) + 1;
				
				int platform = this.chunk.getPlatformGlobalY(y);
				int row = this.chunk.getRowGlobalZ(z);
				int col = this.chunk.getColGlobalX(x);
				
				pos = this.chunk.getLocalPositionOfTile(platform,row,col);

			} while (this.coins.ContainsKey(pos));
		
			SpawnCoin(pos);
		}
		
		

	}
}

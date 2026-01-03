
using Godot;



public partial class World : Node3D
{
	private WorldNoise noise = new WorldNoise();
	private Godot.Vector3 WorldPos = GameGlobals.StartWorldMiddle;
	// EnemyManager enemyManager;
	CoinManager coinManager = new CoinManager();

	
	private Godot.Vector3 GetChunkPositionFromGlobalPos(Godot.Vector3 pos)
	{
		return (Godot.Vector3I)(new Godot.Vector3(pos.X, 0, pos.Z) / (int)GameGlobals.ChunkWidth) * GameGlobals.ChunkWidth;
	}

	public bool CheckIfPosFitsInWorld(Godot.Vector3 pos)
	{
		if (
			pos.X < GameGlobals.MaxWorldTopLeft.X ||
			pos.X > GameGlobals.MaxWorldBottomRight.X ||
			
			pos.Z < GameGlobals.MaxWorldTopLeft.Z ||
			pos.Z > GameGlobals.MaxWorldBottomRight.Z
		)
		{
			return false;
		}		
		return true;
	}

	public Godot.Vector3 getWorldPos()
	{
		return this.WorldPos;
	}

	private float getNoiseValue(float x, float y)
	{
		return noise.GetValue(x,y);
	}

	public int getBlockHeightAtPos(float x, float z)
	{
		float y = getNoiseValue(x,z) * 15f;
		// now y is a float which we don't like for our world so we put it in 0-1-2-3..-50range for tiling
		return (int)y;
	}

    public override void _Process(double delta)
    {
        this.coinManager.UpdateCoins();
    }


	
}

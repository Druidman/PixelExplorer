
using Godot;



public partial class World : Node3D
{
	int ind = 0;
	WorldNoise noise = new WorldNoise();
	

	public Godot.Vector3 WorldPos = GameGlobals.StartWorldMiddle;

	
	Player player;
	EnemyManager enemyManager;
	CoinManager coinManager;
	
	public override void _EnterTree()
	{
		this.WorldPos = GameGlobals.StartWorldMiddle;
	}
	public override void _Ready()

	{
		ThreadGuard.Initialize();
		GameGlobals.Initialize();

		player = (Player)GetNode("../Player");
		enemyManager = new EnemyManager(player, this);		
	}
	
	public Godot.Vector3I getDistanceFromWorldMiddleInChunksCount(Godot.Vector3 pos)
	{
		return (Godot.Vector3I)(pos - this.WorldPos).Abs() / (int)GameGlobals.ChunkWidth;
	}
	
	public override void _Process(double delta)
	{
		this.enemyManager.UpdateEnemies();
		
	}
	
	private Godot.Vector3 GetChunkPos(Godot.Vector3 pos)
	{
		return (Godot.Vector3I)(new Godot.Vector3(pos.X, 0, pos.Z) / (int)GameGlobals.ChunkWidth) * GameGlobals.ChunkWidth;
	}

	private void updateWorldPos(Godot.Vector3 pos)
	{
		this.WorldPos = pos;
	}

	
}

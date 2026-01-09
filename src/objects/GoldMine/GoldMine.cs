using Godot;

public partial class GoldMine : StaticBody3D
{
	Player player = null;
	Godot.Vector3 pos;

	public override void _Ready()
	{
		this.GlobalPosition = pos;
	}
	public void Initialize(Player player, Godot.Vector3 position)
	{
		this.player = player;
		this.pos = position;
	}

	public void OnTimerCall()
	{
		if (this.player == null) return;
		
		
		this.player.AddCoins(10);
	}
}

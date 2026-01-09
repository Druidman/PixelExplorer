using Godot;

public partial class GoldMine : StaticBody3D
{
	Player player = null;


	public void AssignOwner(Player player)
	{
		this.player = player;
	}

	public void OnTimerCall()
	{
		if (this.player == null) return;
		
		
		this.player.AddCoins(10);
	}
}

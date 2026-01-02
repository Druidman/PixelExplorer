using Godot;

public partial class Bush : StaticBody3D{
	public void Remove()
	{
		this.GetParent().RemoveChild(this);
		QueueFree();
	}
	public void Destroy(Player player)
	{
		player.AddCoins(1);
		Remove();
	}
}

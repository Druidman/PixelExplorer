using Godot;

public partial class Ore : StaticBody3D
{
	Godot.Vector3 pos;
	public void Initialize(Godot.Vector3 pos)
	{
		this.pos = pos;
	}

	public override void _Ready()
	{
		this.GlobalPosition = pos;
	}
}

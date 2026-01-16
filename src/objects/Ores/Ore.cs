using Godot;

public partial class Ore : StaticBody3D
{
	public Godot.Vector3 globalPos;
	public void Initialize(Godot.Vector3 globalPos)
	{
		this.globalPos = globalPos;
	}

	public override void _Ready()
	{
		this.GlobalPosition = globalPos;
	}
}

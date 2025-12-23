using Godot;


[Tool]
public partial class World : Node3D
{
	public override void _Ready()
	{
		WorldNoise noise = new WorldNoise();
		
		Chunk chunk = new Chunk(new Godot.Vector3(0,0f,0), noise);
		MeshInstance3D mesh = chunk.InitMesh();
		AddChild(mesh);

	}
}

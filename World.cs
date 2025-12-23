using Godot;

public partial class World : Node3D
{
	public override void _Ready()
	{
		WorldNoise noise = new WorldNoise();
		
		Chunk chunk = new Chunk(new Godot.Vector3(-32f,0f,-32f), noise);
		MeshInstance3D mesh = chunk.InitMesh();
		AddChild(mesh);

		Chunk chunk2 = new Chunk(new Godot.Vector3(32f,0f,32f), noise);
		MeshInstance3D mesh2 = chunk2.InitMesh();
		AddChild(mesh2);

		Chunk chunk3 = new Chunk(new Godot.Vector3(-32f,0f,32f), noise);
		MeshInstance3D mesh3 = chunk3.InitMesh();
		AddChild(mesh3);

		Chunk chunk4 = new Chunk(new Godot.Vector3(32f,0f,-32f), noise);
		MeshInstance3D mesh4 = chunk4.InitMesh();
		AddChild(mesh4);
	}
}

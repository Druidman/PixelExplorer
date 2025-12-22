using Godot;

public partial class World : Node3D
{
	public override void _Ready()
	{
		FastNoiseLite noise = new FastNoiseLite();
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Value;
		
		Chunk chunk = new Chunk(new Godot.Vector3(0.0f,0.0f,0.0f), noise);
		MeshInstance3D mesh = chunk.InitMesh();
		AddChild(mesh);

		Chunk chunk2 = new Chunk(new Godot.Vector3(32.0f,0.0f,32.0f), noise);
		MeshInstance3D mesh2 = chunk2.InitMesh();
		AddChild(mesh2);
	}
}

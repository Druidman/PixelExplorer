

using System.Collections.Generic;

using Godot;

public partial class Chunk : MeshInstance3D
{
	List<Godot.Vector3> vertices = new List<Godot.Vector3>();
	public override void _Ready()
	{
		
		vertices.Add(new Vector3(0, 1, 0));
		vertices.Add(new Vector3(1, 0, 0));
		vertices.Add(new Vector3(0, 0, 1));
		
		
	}

	public override void _Process(double delta)
	{
		var newMesh = new Godot.ArrayMesh();
		

		var arrays = new Godot.Collections.Array();
		
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		
		
		newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		
		this.Mesh = newMesh;
	}
}

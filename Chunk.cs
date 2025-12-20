

using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Chunk : MeshInstance3D
{
	List<Godot.Vector3> vertices = new List<Godot.Vector3>();

	// List< GameObject > gameObjects;
	List< WorldTile > worldTiles = new List<WorldTile>();
	public override void _Ready()
	{
		
		worldTiles.Add(new WorldTile(new Godot.Vector3(0.0f,0.0f,0.0f), 1.0f));
		vertices.AddRange(worldTiles[0].GetVertices());
		setMesh();
		
	}
	private void setMesh()
	{
		var newMesh = new Godot.ArrayMesh();
		

		var arrays = new Godot.Collections.Array();
		
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		
		
		newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		
		this.Mesh = newMesh;
	}
	public override void _Process(double delta)
	{
		
	}
}

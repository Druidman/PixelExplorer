

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
[Tool]
public partial class Chunk : MeshInstance3D
{
	List<Godot.Vector3> vertices = new List<Godot.Vector3>();
	List<Godot.Vector3> normals = new List<Godot.Vector3>();

	// List< GameObject > gameObjects;
	List< WorldTile > worldTiles = new List<WorldTile>();
	FastNoiseLite noise = new Godot.FastNoiseLite();

	int width = 64;
	public override void _Ready()
	{
		int startXpos = -(width/2);
		int startZpos = -(width/2);

		for (int i = 0; i<this.width; i++)
		{
			for (int j = 0; j<this.width; j++)
			{
				float x = i + startXpos;
				float z = j + startZpos;
				float y = noise.GetNoise2D(x,z);
				// y is in -0.5 to 0.5
				y += 0.5f; // move it to 0-1

				y *= 10; // move it to 0-50
				// now y is a float which we don't like for our world so we put it in 0-1-2-3..-50range for tiling
				y = (int)y;
				worldTiles.Add(new WorldTile(new Godot.Vector3(x,y,z), 1.0f));
			}	
		}

		
		
		
	
		GenMesh();


		setMesh();
		
	}
	private void GenMesh()
	{
		for (int i = 0; i<this.worldTiles.Count(); i++){
			this.vertices.AddRange(this.worldTiles[i].GetVertices());
			this.normals.AddRange(this.worldTiles[i].GetNormals());
		}
	}
	private void setMesh()
	{
		var newMesh = new Godot.ArrayMesh();
		

		var arrays = new Godot.Collections.Array();
		
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();

		
		
		newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		
		this.Mesh = newMesh;
	}
	public override void _Process(double delta)
	{
		
	}
}

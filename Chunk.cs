

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
public partial class Chunk : MeshInstance3D
{

	// List< GameObject > gameObjects;
	List< WorldTile > worldTiles = new List<WorldTile>();
	FastNoiseLite noise = new Godot.FastNoiseLite();
 	private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
	private List<Godot.Vector3> Normals = new List<Godot.Vector3>();
	private List<Godot.Vector2> Uvs = new List<Godot.Vector2>();

	int width = 64;
	public override void _Ready()
	{
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Value;
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

		for (int i =0; i< this.worldTiles.Count(); i++)
		{
			this.Vertices.AddRange(worldTiles[i].GetVertices());
			this.Normals.AddRange(worldTiles[i].GetNormals());
			this.Uvs.AddRange(worldTiles[i].GetUvs());
		}

		var newMesh = new Godot.ArrayMesh();

		StandardMaterial3D mat = new StandardMaterial3D();

		ImageTexture texture = new ImageTexture();
		Image img = new Image();
		img.Load("res://images/texture.png");
		texture.SetImage(img);

		mat.AlbedoTexture = texture;

		this.MaterialOverride = mat; // IMPORTANT
		
		
		var arrays = new Godot.Collections.Array();
		
		arrays.Resize((int)Godot.Mesh.ArrayType.Max);
		arrays[(int)Godot.Mesh.ArrayType.Vertex] = this.Vertices.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.Normal] = this.Normals.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.TexUV] = this.Uvs.ToArray();

		
		
		newMesh.AddSurfaceFromArrays(Godot.Mesh.PrimitiveType.Triangles, arrays);
		
		
		
		this.Mesh = newMesh;
		this.CreateTrimeshCollision();

		GD.Print(GetChildCount());

		
	}
	public override void _Process(double delta)
	{
		
	}
}

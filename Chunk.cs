

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
[Tool]
public partial class Chunk : Node3D
{

	// List< GameObject > gameObjects;
	List< WorldTile > worldTiles = new List<WorldTile>();
	FastNoiseLite noise = new Godot.FastNoiseLite();

	int width = 256;
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

		for (int i =0; i< this.worldTiles.Count(); i++)
		{
			worldTiles[i].setUpMesh();
			MeshInstance3D mesh = worldTiles[i].getMeshInstance();
			mesh.CreateTrimeshCollision();
			
			AddChild(mesh);
		}

		GD.Print(GetChildCount());

		
	}
	public override void _Process(double delta)
	{
		
	}
}

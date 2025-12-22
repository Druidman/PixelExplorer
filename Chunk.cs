

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

// chunk Position is declared as bottom center pos !!!
public partial class Chunk
{
	static int Width = 32;
	static int Height = 64;

	Godot.Vector3 chunkPos;
	Godot.Vector3 chunkTopLeft; // -z, -x
	FastNoiseLite noise;


	List< List< List< WorldTile > > > tiles = new List<List<List<WorldTile>>>();
	private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
	private List<Godot.Vector3> Normals = new List<Godot.Vector3>();
	private List<Godot.Vector2> Uvs = new List<Godot.Vector2>();
	
	
	public Chunk(Godot.Vector3 chunkPosition, FastNoiseLite worldNoise)
	{
		this.chunkPos = chunkPosition;
		this.chunkTopLeft = chunkPos - new Godot.Vector3((Width/2), 0, (Width/2));
		this.noise = worldNoise;

	}
	public MeshInstance3D InitMesh()
	{
		this.CreateDefaultTilePlatforms();

		generateTiles();

		

		for (int i =0; i< this.tiles.Count(); i++)
		{
			for (int j =0; j< this.tiles[i].Count(); j++)
			{
				for (int k =0; k< this.tiles[i][j].Count(); k++)
				{
					if (tiles[i][j][k].blockType != BlockType.NONE)
					{
						this.Vertices.AddRange(tiles[i][j][k].GetVertices());
						this.Normals.AddRange(tiles[i][j][k].GetNormals());
						this.Uvs.AddRange(tiles[i][j][k].GetUvs());
					}
					
				}	
			}
		}

		MeshInstance3D mesh = new MeshInstance3D();

		var newMesh = new Godot.ArrayMesh();

		StandardMaterial3D mat = new StandardMaterial3D();

		ImageTexture texture = new ImageTexture();
		Image img = new Image();
		img.Load("res://images/customTexture.png");
		
		texture.SetImage(img);
		
		mat.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

		mat.AlbedoTexture = texture;

		mesh.MaterialOverride = mat; // IMPORTANT

		
		
		var arrays = new Godot.Collections.Array();
		
		arrays.Resize((int)Godot.Mesh.ArrayType.Max);
		arrays[(int)Godot.Mesh.ArrayType.Vertex] = this.Vertices.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.Normal] = this.Normals.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.TexUV] = this.Uvs.ToArray();

		
		
		newMesh.AddSurfaceFromArrays(Godot.Mesh.PrimitiveType.Triangles, arrays);
		
		
		
		
		mesh.Mesh = newMesh;
		mesh.CreateTrimeshCollision();

		// GD.Print(tiles[0][0][0].Position);
		// GD.Print(tiles[0][Width-1][Width-1].Position);
		// GD.Print(getGlobalPositionOfTile(0,0,0));
		// GD.Print(getGlobalPositionOfTile(0,Width-1,Width-1));
		

		return mesh;

		
	}

	public int getPlatformGlobalY(float y)
	{	
		if (y < 0 || (int)y >= tiles.Count()) return -1;

		return (int)y;
	}

	public int getRowGlobalZ(float z)
	{	

		float localPos = this.ConvertToLocalChunkPos(new Godot.Vector3(0.0f, 0.0f,z)).Z;

		return (int)localPos;
	}
	public int getColGlobalX(float x)
	{	

		float localPos = this.ConvertToLocalChunkPos(new Godot.Vector3(x, 0.0f,0.0f)).X;

		return (int)localPos;
	}
	public Godot.Vector3 getGlobalPositionOfTile(int platform, int row, int col)
	{
		return ConvertToGlobalPosition(new Godot.Vector3(col + 0.5f, platform + 0.5f, row + 0.5f ));
	}
	public Godot.Vector3 ConvertToLocalChunkPos(Godot.Vector3 globalPos)
	{
		return new Godot.Vector3(globalPos.X - this.chunkTopLeft.X, globalPos.Y, globalPos.Z - this.chunkTopLeft.Z);
	}
	public Godot.Vector3 ConvertToGlobalPosition(Godot.Vector3 localPos)
	{
		return new Godot.Vector3(localPos.X + this.chunkTopLeft.X, localPos.Y, localPos.Z + this.chunkTopLeft.Z);
	}

	private void generateTiles()
	{
		for (float x = this.chunkTopLeft.X + 0.5f; x < this.chunkTopLeft.X + Width; x += GameGlobals.TileWidth)
		{
			for (float z = this.chunkTopLeft.Z + 0.5f; z < this.chunkTopLeft.Z + Width; z += GameGlobals.TileWidth)
			{
				int y = generateTileHeight(x,z);


				int platform = getPlatformGlobalY(y);
				int row = getRowGlobalZ(z);
				int col = getColGlobalX(x);
				
				if (platform >= this.tiles.Count())
				{
					continue; // skip if height bigger than max height
				}
				GD.Print(platform, " ", row, " ", col);
				
				this.tiles[platform][row][col] = new WorldTile(getGlobalPositionOfTile(platform, row, col), BlockType.Grass);


			
			}	
		}
	}

	private int generateTileHeight(float x, float z)
	{
		float y = noise.GetNoise2D(x,z);
		// y is in -0.5 to 0.5
		y =  (y + 0.5f) * 10; // move it to 0-1, move it to 0-50

		// now y is a float which we don't like for our world so we put it in 0-1-2-3..-50range for tiling
		return (int)y;
	}
	private void CreateDefaultTilePlatforms()
	{
		for (float y=this.chunkTopLeft.Y + 0.5f; y<this.chunkTopLeft.Y + Height; y+=GameGlobals.TileWidth)
		{
			tiles.Add(new List<List<WorldTile>>());	
			int platform = getPlatformGlobalY(y);

			for (float z=this.chunkTopLeft.Z + 0.5f; z<this.chunkTopLeft.Z + Width; z+=GameGlobals.TileWidth)
			{
				tiles[platform].Add(new List<WorldTile>());
				int row = getRowGlobalZ(z);
				for (float x=this.chunkTopLeft.X + 0.5f; x<this.chunkTopLeft.X + Width; x+=GameGlobals.TileWidth)
				{
					tiles[platform][row].Add(new WorldTile(getGlobalPositionOfTile(platform,row,getColGlobalX(x)), BlockType.NONE));
				}
			}
		}
	}
}

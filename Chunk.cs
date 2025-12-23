

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

// chunk Position is declared as bottom center pos !!!
public class Chunk
{
	static int Width = 64;
	static int Height = 100;

	Godot.Vector3 chunkPos;
	Godot.Vector3 chunkTopLeft; // -z, -x
	WorldNoise noise;


	List< List< List< WorldTile > > > tiles = new List<List<List<WorldTile>>>();
	private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
	private List<Godot.Vector3> Normals = new List<Godot.Vector3>();
	private List<Godot.Vector2> Uvs = new List<Godot.Vector2>();
	
	
	public Chunk(Godot.Vector3 chunkPosition, WorldNoise worldNoise)
	{
		this.chunkPos = chunkPosition;
		this.chunkTopLeft = chunkPos - new Godot.Vector3((Width/2), 0, (Width/2));
		this.noise = worldNoise;

	}
	public MeshInstance3D InitMesh()
	{

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
		if (y < 0 || (int)y >= Height) return -1;

		return (int)(y / (float)GameGlobals.TileWidth);
	}

	public int getRowGlobalZ(float z)
	{	

		float localPos = this.ConvertToLocalChunkPos(new Godot.Vector3(0.0f, 0.0f,z)).Z;

		return (int)(localPos / (float)GameGlobals.TileWidth);
	}
	public int getColGlobalX(float x)
	{	

		float localPos = this.ConvertToLocalChunkPos(new Godot.Vector3(x, 0.0f,0.0f)).X;

		return (int)(localPos / (float)GameGlobals.TileWidth);
	}
	public Godot.Vector3 getGlobalPositionOfTile(int platform, int row, int col)
	{
		return ConvertToGlobalPosition(new Godot.Vector3((col * GameGlobals.TileWidth)  + (GameGlobals.TileWidth / 2), (platform * GameGlobals.TileWidth) + (GameGlobals.TileWidth / 2), (row * GameGlobals.TileWidth) + (GameGlobals.TileWidth / 2) ));
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
		int minY = 0;
		int maxY = 0;
		for (float x = this.chunkTopLeft.X + (GameGlobals.TileWidth / 2.0f); x <= this.chunkTopLeft.X + Width - (GameGlobals.TileWidth / 2.0f); x += GameGlobals.TileWidth)
		{
			for (float z = this.chunkTopLeft.Z + (GameGlobals.TileWidth / 2.0f); z <= this.chunkTopLeft.Z + Width - (GameGlobals.TileWidth / 2.0f); z += GameGlobals.TileWidth)
			{
				int y = generateTileHeight(x,z);
				if (y < minY)
				{
					minY = y;
				}
				if (y > maxY)
				{
					maxY = y;
				}


				int platform = getPlatformGlobalY(y);
				int row = getRowGlobalZ(z);
				int col = getColGlobalX(x);
				
				if (!CheckIfValidTileIndicies(platform, 0, 0))
				{
					platform = 0;
				}
				// GD.Print(platform, " ", row, " ", col);
				BlockType blockType = BlockType.Grass;
				
				UpdateTile(platform, row, col, new WorldTile(getGlobalPositionOfTile(platform, row, col), blockType));
				


			
			}	
		}
		GD.Print("MAX: " + maxY + " MIN: " + minY);
	}

	private bool UpdateTile(int platform, int row, int col, WorldTile tile)
	{	
		if (!CheckIfTileFits(platform, row, col))
		{
			if (!ResizeTilesToPlatform(platform)) return false;
			if (!ResizeTilesToRow(platform,row)) return false;
			if (!ResizeTilesToCol(platform,row,col)) return false;
		}

		// GD.Print("Plat: " + platform + " Row: " + row + " Col: " + col);
		// GD.Print(this.tiles[platform]);
		// GD.Print(this.tiles[platform][row]);
		// GD.Print(this.tiles[platform][row][col]);
		this.tiles[platform][row][col] = tile;
		return true;
	}

	private bool ResizeTilesToPlatform(int platform)
	{
		if (!CheckIfValidTileIndicies(platform, 0, 0)) return false;
		

		
		
		for (int i = this.tiles.Count(); i<platform + 1; i++)
		{
			this.tiles.Add(new List<List<WorldTile>>());
		}
		return true;
	}
	private bool ResizeTilesToRow(int platform, int row)
	{
		// GD.Print("Plat: " + platform + " Row: " + row + " FROM RESIZE ROW");
		if (!CheckIfValidTileIndicies(platform, row, 0)) return false;

		if (!CheckIfTilePlatformFits(platform)) return false;

		// GD.Print("Pass " + this.tiles[platform].Count());
		
		for (int i = this.tiles[platform].Count(); i<row + 1; i++)
		{
			// GD.Print("Adding on " + i);
			this.tiles[platform].Add(new List<WorldTile>());
		}
		return true;
	}
	private bool ResizeTilesToCol(int platform, int row, int col)
	{

		if (!CheckIfValidTileIndicies(platform, row, col)) return false;

		if (!CheckIfTileRowFits(platform, row)) return false;

		
		for (int i = this.tiles[platform][row].Count(); i<col + 1; i++)
		{
			this.tiles[platform][row].Add(new WorldTile(getGlobalPositionOfTile(platform, row, i),BlockType.NONE));
		}
		return true;
	}

	private bool CheckIfTileFits(int platform, int row, int col)
	{
		if (!CheckIfTileColFits(platform, row, col)) return false;

		return true;
	}

	private bool CheckIfTilePlatformFits(int platform)
	{
		if (platform < 0 || platform >= this.tiles.Count()) return false;
		return true;
	}
	private bool CheckIfTileRowFits(int platform, int row)
	{
		if (!CheckIfTilePlatformFits(platform)) return false;
		if (row < 0 || row >= this.tiles[platform].Count()) return false;
		return true;
	}
	private bool CheckIfTileColFits(int platform, int row, int col)
	{
		if (!CheckIfTileRowFits(platform, row)) return false;
		if (col < 0 || col >= this.tiles[platform][row].Count()) return false;
		return true;
	}

	private bool CheckIfValidTileIndicies(int platform, int row, int col)
	{

		if (platform < 0 || platform > Height / GameGlobals.TileWidth) return false;
		if (row < 0 || row > Width / GameGlobals.TileWidth) return false;
		if (col < 0 || col > Width / GameGlobals.TileWidth) return false;

		return true;
	}
	private int generateTileHeight(float x, float z)
	{
		float y = noise.GetValue(x,z) * 15f;
		
		

		// now y is a float which we don't like for our world so we put it in 0-1-2-3..-50range for tiling
		return (int)y;
	}
	
}

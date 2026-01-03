

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;


public enum ChunkCollisionState
{
	NONE,
	QUEUED,
	GENERATED
}
// chunk Position is declared as bottom center pos !!!
public partial class Chunk : Node3D
{


	static int Width = GameGlobals.ChunkWidth;
	static int Height = 100;

	public Godot.Vector3 chunkPos;
	public Godot.Vector3 chunkTopLeft; // -z, -x
	World world;

	private ChunkCoinManager chunkCoinManager;

	[Export]
	public MeshInstance3D mesh;

	List< List< List< WorldTile > > > tiles = new List<List<List<WorldTile>>>();
	private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
	private List<Godot.Vector3> Normals = new List<Godot.Vector3>();
	private List<Godot.Vector2> Uvs = new List<Godot.Vector2>();
	

	public bool meshReady = false;
	public bool addedToTree = false;
	public ChunkCollisionState chunkCollisionState = ChunkCollisionState.NONE;
	public void Initialize(Godot.Vector3 chunkPosition)
	{
		
		this.chunkPos = chunkPosition;
		this.chunkTopLeft = chunkPos - new Godot.Vector3((Width/2), 0, (Width/2));
		this.world = GameGlobals.world;
		this.chunkCoinManager = new ChunkCoinManager(this);
		this.chunkCoinManager.UpdateCoins(); // gen base ones
		

	}
	public override void _EnterTree()
	{
		this.GlobalPosition = this.chunkPos;
	}
	

	public void GenerateChunkMesh()
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

		
	}
	public void BuildChunkMesh(ImageTexture BlockTexture)
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadGuard.MainThreadId)
			throw new InvalidOperationException("Method must be called from main thread");

		var newMesh = new Godot.ArrayMesh();
		
		

		StandardMaterial3D mat = new StandardMaterial3D();

		
		
		mat.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

		mat.AlbedoTexture = BlockTexture;

		mesh.MaterialOverride = mat; // IMPORTANT

		
		
		var arrays = new Godot.Collections.Array();
		
		arrays.Resize((int)Godot.Mesh.ArrayType.Max);
		arrays[(int)Godot.Mesh.ArrayType.Vertex] = this.Vertices.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.Normal] = this.Normals.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.TexUV] = this.Uvs.ToArray();

		newMesh.AddSurfaceFromArrays(Godot.Mesh.PrimitiveType.Triangles, arrays);
	
		
		
		mesh.Mesh = newMesh;
		
		this.meshReady = true;

		
	}
	public void GenerateChunkCollision()
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadGuard.MainThreadId)
			throw new InvalidOperationException("Method must be called from main thread");

		this.chunkCollisionState = ChunkCollisionState.NONE;
		mesh.CreateTrimeshCollision();
		

		this.chunkCollisionState = ChunkCollisionState.GENERATED;
		
		
		
	}

	public int getPlatformGlobalY(float y)
	{	

		if (y < 0 || y >= Height) return -1;

		float topLeftBasedPos = y - this.chunkTopLeft.Y;
		

		return (int)MathF.Floor(topLeftBasedPos / (float)GameGlobals.TileWidth);
	}

	public int getRowGlobalZ(float z)
	{	

		float topLeftBasedPos = z - this.chunkTopLeft.Z;

		return (int)MathF.Floor(topLeftBasedPos / (float)GameGlobals.TileWidth);
	}
	public int getColGlobalX(float x)
	{	

		float topLeftBasedPos = x - this.chunkTopLeft.X;

		return (int)MathF.Floor(topLeftBasedPos / (float)GameGlobals.TileWidth);
	}
	public Godot.Vector3 getGlobalPositionOfTile(int platform, int row, int col)
	{
		return ConvertToGlobalPosition( getLocalPositionOfTile(platform, row, col) );
	}

	public Godot.Vector3 getLocalPositionOfTile(int platform, int row, int col)
	{
		return new Godot.Vector3(
			col + ((float)GameGlobals.TileWidth / 2f), 
			platform + ((float)GameGlobals.TileWidth / 2f), 
			row + ((float)GameGlobals.TileWidth / 2f)
		) - new Godot.Vector3(GameGlobals.ChunkWidth / 2, GameGlobals.ChunkWidth / 2, GameGlobals.ChunkWidth / 2);
	}
	public Godot.Vector3 ConvertToLocalChunkPos(Godot.Vector3 globalPos)
	{
		return globalPos - this.chunkPos;
	}
	public Godot.Vector3 ConvertToGlobalPosition(Godot.Vector3 localPos)
	{
		return localPos + this.chunkPos;
	}

	private void generateTiles()
	{
		int minY = 0;
		int maxY = 0;
		for (float x = this.chunkTopLeft.X + (GameGlobals.TileWidth / 2.0f); x <= this.chunkTopLeft.X + Width - (GameGlobals.TileWidth / 2.0f); x += GameGlobals.TileWidth)
		{
			for (float z = this.chunkTopLeft.Z + (GameGlobals.TileWidth / 2.0f); z <= this.chunkTopLeft.Z + Width - (GameGlobals.TileWidth / 2.0f); z += GameGlobals.TileWidth)
			{
				int y = this.world.getBlockHeightAtPos(x,z);
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
	
				BlockType blockType = BlockType.Grass;
				
				
				UpdateTile(platform, row, col, new WorldTile(getLocalPositionOfTile(platform, row, col), blockType));
				
			}	
		}

	}

	public bool CheckIfGlobalPosFits(Godot.Vector3 GlobalPos)
	{
		return CheckIfValidTileIndicies(
			getPlatformGlobalY(GlobalPos.Y),
			getRowGlobalZ(GlobalPos.Z),
			getColGlobalX(GlobalPos.X)
			
		);
		
	}
	public bool CheckIfLocalPosFits(Godot.Vector3 localPos)
	{
		return CheckIfGlobalPosFits(ConvertToGlobalPosition(localPos));
	
	}
	private bool UpdateTile(int platform, int row, int col, WorldTile tile)
	{	
		if (!CheckIfTileFits(platform, row, col))
		{
			if (!ResizeTilesToPlatform(platform)) return false;
			if (!ResizeTilesToRow(platform,row)) return false;
			if (!ResizeTilesToCol(platform,row,col)) return false;
		}


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
		
		if (!CheckIfValidTileIndicies(platform, row, 0)) return false;

		if (!CheckIfTilePlatformFits(platform)) return false;

		
		
		for (int i = this.tiles[platform].Count(); i<row + 1; i++)
		{
			
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
	
}

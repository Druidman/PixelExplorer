

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
	public World world;

	private ChunkCoinManager chunkCoinManager;
	

	[Export]
	public MeshInstance3D mesh;

	
	private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
	private List<Godot.Vector3> Normals = new List<Godot.Vector3>();
	private List<Godot.Vector2> Uvs = new List<Godot.Vector2>();
	public bool meshReady = false;
	public bool addedToTree = false;
	public bool disabled = false;
	public ChunkCollisionState chunkCollisionState = ChunkCollisionState.NONE;

		  
	public void Initialize(Godot.Vector3 chunkPosition, World world)
	{
		
		this.chunkPos = chunkPosition;
		this.chunkTopLeft = chunkPos - new Godot.Vector3((Width/2f), 0, (Width/2f));
		this.world = world;
		this.chunkCoinManager = new ChunkCoinManager(this);
		this.chunkCoinManager.UpdateCoins(); // gen base one
	}
	public override void _EnterTree()
	{
		this.GlobalPosition = this.chunkPos;
	}
	
	public void GenerateChunk()
	{
		this.generateTiles();
		this.GenerateChunkTileMesh();
	}

	public void ApplyChunkObjects()
	{
		List<Ore> ores = this.world.GetChunkOres(this.chunkPos);
		if (ores != null)
		{
			foreach (Ore ore in ores)
			{
				AddChild(ore);
			}
		}
		
	}
	public void CreateChunkCollision()
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadGuard.MainThreadId)
			throw new InvalidOperationException("Method must be called from main thread");

		this.chunkCollisionState = ChunkCollisionState.NONE;

		this.mesh.CreateTrimeshCollision();
		

		this.chunkCollisionState = ChunkCollisionState.GENERATED;
	}

	private void GenerateChunkTileMesh()
	{
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
	public void ApplyChunkTileMesh()
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadGuard.MainThreadId)
			throw new InvalidOperationException("Method must be called from main thread");

		var newMesh = new Godot.ArrayMesh();
		
		

		StandardMaterial3D mat = new StandardMaterial3D();

		
		
		mat.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

		mat.AlbedoTexture = GameGlobals.texture;

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

	private void generateTiles()
	{
		int minY = 0;
		int maxY = 0;
		for (float x = this.chunkTopLeft.X + (GameGlobals.TileWidth / 2f); x <= this.chunkTopLeft.X + Width; x += GameGlobals.TileWidth)
		{
			for (float z = this.chunkTopLeft.Z + (GameGlobals.TileWidth / 2f); z <= this.chunkTopLeft.Z + Width; z += GameGlobals.TileWidth)
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

	
	
}
	
	

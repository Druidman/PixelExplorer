

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;

// chunk Position is declared as bottom center pos !!!
public partial class Chunk : MeshInstance3D
{
	static int Width = GameGlobals.ChunkWidth;

	public Godot.Vector3I chunkPos;
	public Godot.Vector3I chunkTopLeft; // -z, -x
	public Godot.Vector3I chunkTopRight; // -z, +x
	public Godot.Vector3I chunkBottomRight; // +z, +x
	public Godot.Vector3I chunkBottomLeft; // +z, -x


	public World world;


	public CollisionShape3D collisionShape;

	
	private List<Godot.Vector3> Vertices = new List<Godot.Vector3>();
	private List<Godot.Vector3> Normals = new List<Godot.Vector3>();
	private List<Godot.Vector2> Uvs = new List<Godot.Vector2>();
	public bool isAddedToTree
	{
		get
		{
			return (GetParent() == null) ? false : true;
		}
	}
	public bool isChunkCollisionShapeGenerated = false;
	public bool isBlockMeshGenerated = false;
	public bool isBlockMeshApplied = false;


		  
	public void Initialize(Godot.Vector3I chunkPosition, World world)
	{
		
		this.chunkPos = chunkPosition;
		this.chunkTopLeft = chunkPos - new Godot.Vector3I(Width/2, 0, Width/2); // int division is intentional!
		this.chunkTopRight = chunkPos - new Godot.Vector3I(-Width/2, 0, Width/2); // int division is intentional!

		this.chunkBottomRight = chunkPos + new Godot.Vector3I(Width/2, 0, Width/2); // int division is intentional!
		this.chunkBottomLeft = chunkPos + new Godot.Vector3I(-Width/2, 0, Width/2); // int division is intentional!

		this.world = world;

	}
	public override void _EnterTree()
	{

		this.GlobalPosition = this.chunkPos;
	}

	public void ShowChunk()
	{
		this.world.ShowChunkObjects(this);
		this.Visible = true;
	}
	public void HideChunk()
	{
		this.world.HideChunkObjects(this);
		this.Visible = false;
	}
	
	public void GenerateChunk()
	{
		this.generateTiles();
		this.GenerateChunkTileMesh();
	}

	public void CreateChunkCollision()
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadGuard.MainThreadId)
			throw new InvalidOperationException("Method must be called from main thread");

		this.isChunkCollisionShapeGenerated = false;
		
		this.CreateTrimeshCollision();
		
		this.isChunkCollisionShapeGenerated = true;
	}

	private void GenerateChunkTileMesh()
	{
		int i =1;
		foreach (WorldTile tile in this.tiles.Values)
		{
	
			if (tile is Block block)
			{
				
				this.Vertices.AddRange(block.GetVertices());
		
				this.Normals.AddRange(block.GetNormals());
		
				this.Uvs.AddRange(block.GetUvs());
			}
			i++;
			
		}
		this.isBlockMeshGenerated = true;
		
	}
	public void ApplyChunkTileMesh()
	{
		if (Thread.CurrentThread.ManagedThreadId != ThreadGuard.MainThreadId)
			throw new InvalidOperationException("Method must be called from main thread");

		var newMesh = new Godot.ArrayMesh();
		
		

		StandardMaterial3D mat = new StandardMaterial3D();

		
		
		mat.TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest;

		mat.AlbedoTexture = GameGlobals.texture;

		this.MaterialOverride = mat; // IMPORTANT

		
		
		var arrays = new Godot.Collections.Array();
		
		arrays.Resize((int)Godot.Mesh.ArrayType.Max);
		
		arrays[(int)Godot.Mesh.ArrayType.Vertex] = this.Vertices.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.Normal] = this.Normals.ToArray();
		arrays[(int)Godot.Mesh.ArrayType.TexUV] = this.Uvs.ToArray();

		newMesh.AddSurfaceFromArrays(Godot.Mesh.PrimitiveType.Triangles, arrays);
		
		this.Mesh = newMesh;

		this.isBlockMeshApplied = true;
	}

	private void generateTiles()
	{
		int i = 1;
		for (int x = this.chunkTopLeft.X; x <= this.chunkTopRight.X; x += GameGlobals.TileWidth)
		{
			for (int z = this.chunkTopLeft.Z; z <= this.chunkBottomLeft.Z; z += GameGlobals.TileWidth)
			{
				int y = this.world.getBlockHeightAtPos(x,z);
			
				
				Godot.Vector3I globalTilePosition = new Godot.Vector3I(x,y,z);
				Godot.Vector3I localTilePosition = (Godot.Vector3I) ConvertToLocalPosition(globalTilePosition);

				if (!CheckIfValidTileGlobalPosition(globalTilePosition))
				{
					throw new Exception("Wrong position somehow " + globalTilePosition + ' ' + localTilePosition);
				}
	
				BlockType blockType = BlockType.Grass;
				
				
				UpdateTile(localTilePosition, new Block(localTilePosition, blockType));
				
				i++;
				
			}	
		}

	}

	
	
}
	
	

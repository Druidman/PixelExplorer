using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;

public enum PlayerMoveDir
{
	PositiveZ,
	PositiveX,
	NegativeZ,
	NegativeX
}

public partial class World : Node3D
{

	WorldNoise noise = new WorldNoise();

	List<List<Chunk>> chunks = new List<List<Chunk>>();
	float Xpos = 0f;
	bool exitApp = false;
	Thread chunkManagerThread;
	ImageTexture texture = new ImageTexture();

	CharacterBody3D player;
	Godot.Vector3 WorldPos = GameGlobals.StartWorldMiddle;

	bool chunksReady = false;
	Godot.Vector3 WorldTopLeftPos = GameGlobals.StartWorldMiddle - new Godot.Vector3(GameGlobals.WorldWidth / 2, 0, GameGlobals.WorldWidth / 2);
	public override void _Ready()

	{

		player = (CharacterBody3D)GetNode("../Player");
		
		Image img = new Image();
		img.Load("res://images/customTexture.png");
		
		texture.SetImage(img);

		
		this.chunkManagerThread = new Thread(chunkManager);
		this.chunkManagerThread.Start();
	}
	private void chunkManager()
	{
		int count = 0;

		for (int i=0; i<GameGlobals.WorldWidth / GameGlobals.ChunkWidth; i++)
		{
			this.chunks.Add(new List<Chunk>());
			for (int j=0; j<GameGlobals.WorldWidth / GameGlobals.ChunkWidth; j++)
			{
				Godot.Vector3 pos = new Godot.Vector3(GameGlobals.ChunkWidth / 2,0,GameGlobals.ChunkWidth / 2) + 
					this.WorldTopLeftPos + 
					new Godot.Vector3(j*GameGlobals.ChunkWidth, 0, i*GameGlobals.ChunkWidth);

				Chunk chunk = new Chunk(pos, this.noise, texture);
				this.chunks[i].Add(chunk);

				Thread t = this.startChunkGenThread(
					chunk

				);
				GD.Print("Count: " + count);
				count++;
				Thread.Sleep(100);
				GD.Print("ChunkPos: ", pos);
				
			}	
		}

		this.chunksReady = true;
	}

	public override void _Process(double delta)
	{
		if (!chunksReady)
		{
			return;
		}
		Chunk currentPlayerChunk = getChunkByPos(this.player.GlobalPosition);
		if (currentPlayerChunk == getChunkByPos(this.WorldPos))
		{
			return; // player didn't change chunks so nothing to do here
		}

		GD.Print("PLayer changed chunk");
		
		
		Godot.Vector2I currIndices = GetIndicesFromLocalWorldPos( ConvertToLocalWorldPos(currentPlayerChunk.chunkPos) );
		Godot.Vector2I prevIndices = GetIndicesFromLocalWorldPos( ConvertToLocalWorldPos(this.WorldPos) );
		
		GD.Print("WOrldPos: ", this.WorldPos);
		updateWorldPos(currentPlayerChunk.chunkPos);
		GD.Print("WOrldPos: ", this.WorldPos);
		// calc player move dir
		if (currIndices.X > prevIndices.X) // for bigger row so move +z
		{
			int chunksChanged = currIndices.X - prevIndices.X;
			for (int i=0; i < chunksChanged; i++)
			{
				for (int j = 0; j<this.chunks[0].Count; j++)
				{
					RemoveChild(this.chunks[0][j].mesh);
				}
				this.chunks.RemoveAt(0);
				List<Chunk> newRow = new List<Chunk>();
				for (float j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + ((GameGlobals.ChunkRowCount - 1) * GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);
					newRow.Add(chunk);
				}
				this.chunks.Add(newRow);
			}
		}
		if (currIndices.X < prevIndices.X) // for smaller row so move -z
		{
			int chunksChanged = currIndices.X - prevIndices.X;
			for (int i=0; i < chunksChanged; i++)
			{
				
			}
		}
		if (currIndices.Y > prevIndices.Y) // for bigger col so move +x
		{
			int chunksChanged = currIndices.Y - prevIndices.Y;
			for (int i=0; i < chunksChanged; i++)
			{
				
			}
		}
		if (currIndices.Y < prevIndices.Y) // for smaller row so move +z
		{
			int chunksChanged = currIndices.Y - prevIndices.Y;
			for (int i=0; i < chunksChanged; i++)
			{
				
			}
		}
		
		// Move all
		
		
		// queue new ones
		
	}
	private void updateWorldPos(Godot.Vector3 pos)
	{
		this.WorldPos = pos;
		this.WorldTopLeftPos = this.WorldPos - new Godot.Vector3(GameGlobals.WorldWidth / 2, 0, GameGlobals.WorldWidth / 2);

	}

	public Chunk getChunkByPos(Godot.Vector3 pos)
	{
		Godot.Vector3 localPos = this.ConvertToLocalWorldPos(pos);
		Godot.Vector2I indices = GetIndicesFromLocalWorldPos(localPos);

		return this.chunks.ElementAt(indices.X).ElementAt(indices.Y);
	}
	public Godot.Vector3 ConvertToLocalWorldPos(Godot.Vector3 pos)
	{
		return pos - this.WorldTopLeftPos;
	}

	public Godot.Vector2I GetIndicesFromLocalWorldPos(Godot.Vector3 localPos)
	{
		return new Godot.Vector2I((int)localPos.Z, (int)localPos.X) / (int)GameGlobals.ChunkWidth;
	}
	public override void _ExitTree()
	{
		this.exitApp = true;
		this.chunkManagerThread.Join();
	}
	private Thread startChunkGenThread(Chunk chunk)
	{
		
		Thread chunkThread = new Thread(()=>GenChunk(chunk));
		chunkThread.Start();
		return chunkThread;
	}
	private void GenChunk(Chunk chunk)
	{

		GD.Print("StartUp");
		chunk.InitMesh();
		GD.Print("Chunk inited");

		// AddChild(chunk.mesh);
		CallDeferred("add_child", chunk.mesh);
		GD.Print("Chunk mesh added");

		
		
		var reff = getChunkByPos(chunk.chunkTopLeft);
		reff = chunk;
		

		
	}
}

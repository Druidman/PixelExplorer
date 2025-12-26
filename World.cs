using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;


public partial class World : Node3D
{

	WorldNoise noise = new WorldNoise();

	List<List<Chunk>> chunks = new List<List<Chunk>>();
	List<Chunk> stagedChunks = new List<Chunk>();
	List<Thread> threads = new List<Thread>();
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
				this.chunks[i].Add(new ChunkPlaceHolder()); // to make map filled

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

	private void CommitChunks()
	{
		if (this.stagedChunks.Count <= 0)
		{
			return;
		}
	
	
		Godot.Vector2I indices = GetIndicesFromLocalWorldPos(ConvertToLocalWorldPos(this.stagedChunks.First().chunkPos));
		if (
			indices.X < 0 || indices.X >= GameGlobals.ChunkRowCount ||
			indices.Y < 0 || indices.Y >= GameGlobals.ChunkRowCount
		)
		{
			return;
		}


		this.chunks[indices.X][indices.Y] = this.stagedChunks.First();
		AddChild(this.stagedChunks.First().mesh);
		this.chunks[indices.X][indices.Y].added = true;

		
		this.stagedChunks.RemoveAt(0);

		
	}


	public Godot.Vector3 GetGlobalPosFromIndices(Godot.Vector2I indices)
	{
		return new Godot.Vector3(
			this.WorldTopLeftPos.X + (indices.Y * GameGlobals.ChunkWidth) + ( GameGlobals.ChunkWidth / 2), 
			this.WorldTopLeftPos.Y, 
			this.WorldTopLeftPos.Z + (indices.X * GameGlobals.ChunkWidth) + ( GameGlobals.ChunkWidth / 2)
		);
	}
	public override void _Process(double delta)
	{
		CommitChunks();
		if (!chunksReady)
		{
			return;
		}

		
		
		
		Godot.Vector2I currIndices = GetIndicesFromLocalWorldPos( ConvertToLocalWorldPos(this.player.GlobalPosition) );
		Godot.Vector2I prevIndices = GetIndicesFromLocalWorldPos( ConvertToLocalWorldPos(this.WorldPos) );

		if (currIndices == prevIndices)
		{
			return; //player stayed in same chunk
		}

		GD.Print("PLayer changed chunk");

		GD.Print("WOrldPos: ", this.WorldPos);
		updateWorldPos(GetGlobalPosFromIndices(currIndices));
		GD.Print("WOrldPos: ", this.WorldPos);
		// calc player move dir
		if (currIndices.X > prevIndices.X) // for bigger row so move +z
		{
			int chunksChanged = currIndices.X - prevIndices.X;
			for (int i=0; i < chunksChanged; i++)
			{
				for (int j = 0; j<this.chunks[0].Count; j++)
				{
					if (this.chunks[0][j].added)
					{
						RemoveChild(this.chunks[0][j].mesh);	
					}
					
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
					newRow.Add(new ChunkPlaceHolder());
				}
				this.chunks.Add(newRow);
			}
		}
		if (currIndices.X < prevIndices.X) // for smaller row so move -z
		{
			int chunksChanged = prevIndices.X - currIndices.X;
			for (int i=0; i < chunksChanged; i++)
			{
				for (int j = 0; j<this.chunks.Last().Count; j++)
				{
					if (this.chunks.Last()[j].added)
					{
						RemoveChild(this.chunks.Last()[j].mesh);	
					}
				}
				this.chunks.RemoveAt(this.chunks.Count - 1);
				List<Chunk> newRow = new List<Chunk>();
				for (float j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);
					newRow.Add(new ChunkPlaceHolder());
				}
				this.chunks.Insert(0, newRow);
			}
		}
		if (currIndices.Y > prevIndices.Y) // for bigger col so move +x
		{
			int chunksChanged = currIndices.Y - prevIndices.Y;
			for (int i=0; i < chunksChanged; i++)
			{
				for (int j = 0; j<GameGlobals.ChunkRowCount; j++)
				{
					if (this.chunks[j].First().added)
					{
						
						RemoveChild(this.chunks[j].First().mesh);
					}
					
					this.chunks[j].RemoveAt(0);
				}
				
			
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(this.WorldTopLeftPos.X + GameGlobals.WorldWidth - (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);
			
					
					this.chunks[j].Add(new ChunkPlaceHolder());
				}
			}
		}
		if (currIndices.Y < prevIndices.Y) // for smaller row so move +z
		{
			int chunksChanged = prevIndices.Y - currIndices.Y;
			for (int i=0; i < chunksChanged; i++)
			{
				for (int j = 0; j<GameGlobals.ChunkRowCount; j++)
				{
					if (this.chunks[j].Last().added)
					{
						RemoveChild(this.chunks[j].Last().mesh);
					}
					
					this.chunks[j].RemoveAt(this.chunks[j].Count - 1);
				}
				
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(this.WorldTopLeftPos.X + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);
					
					
					this.chunks[j].Insert(0, new ChunkPlaceHolder());
				}
			}
		}
		
	}
	private void AddToThreadPool(Thread t)
	{
		this.threads.Add(t);
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
		chunkThread.UnsafeStart();
		
		return chunkThread;
	}
	private void GenChunk(Chunk chunk)
	{

		GD.Print("StartUp");
		chunk.InitMesh();
		GD.Print("Chunk inited");


		this.stagedChunks.Add(chunk);


		

		
	}
}

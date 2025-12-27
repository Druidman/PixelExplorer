using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;


enum ChunksSequence
{
	First,
	Last
}

public partial class World : Node3D
{

	WorldNoise noise = new WorldNoise();

	LinkedList<LinkedList<Chunk>> chunks = new LinkedList<LinkedList<Chunk>>();
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
			
			this.chunks.AddLast(new LinkedList<Chunk>());
			for (int j=0; j<GameGlobals.WorldWidth / GameGlobals.ChunkWidth; j++)
			{
				Godot.Vector3 pos = new Godot.Vector3(GameGlobals.ChunkWidth / 2,0,GameGlobals.ChunkWidth / 2) + 
					this.WorldTopLeftPos + 
					new Godot.Vector3(j*GameGlobals.ChunkWidth, 0, i*GameGlobals.ChunkWidth);

				Chunk chunk = new Chunk(pos, this.noise, texture);
				this.chunks.ElementAt(i).AddLast(new ChunkPlaceHolder()); // to make map filled

				Thread t = this.startChunkGenThread(
					chunk

				);

				GD.Print("Count: " + count);
				count++;
				// Thread.Sleep(100);
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

		if (!this.stagedChunks.First().isMeshReady())
		{
			return ;
		}


		this.stagedChunks.First().addedToTree = true;
		CallDeferred(Node3D.MethodName.AddChild, this.stagedChunks.First().mesh);
		var row = this.chunks.ElementAt(indices.X);

		if (indices.Y == 0) row.AddFirst(this.stagedChunks.First());
		else if (indices.Y == row.Count) row.AddLast(this.stagedChunks.First());
		else
		{
			GD.Print(indices.Y);
			LinkedListNode<Chunk> node = row.First;
			for (int i =0; i< indices.Y; i++)
			{
				node = node.Next;
			}
			row.AddBefore(node, this.stagedChunks.First());
			row.Remove(node);
			
		}

		
		
		this.stagedChunks.RemoveAt(0);

		
	}

	private void CleanUpChunk(Chunk chunk)
	{
		if (chunk is ChunkPlaceHolder)
		{
			return;
		}
		if (chunk.addedToTree)
		{
			CallDeferred(Node3D.MethodName.RemoveChild, chunk.mesh);
		}
	}
	private void RemoveRow(ChunksSequence rowSeq)
	{
		if (this.chunks.Count == 0) return ;

		LinkedList<Chunk> row;
		if (rowSeq == ChunksSequence.First) row = this.chunks.First.Value;
		else if (rowSeq == ChunksSequence.Last) row = this.chunks.Last.Value;
		else return;

		foreach (Chunk chunk in row)
		{
			CleanUpChunk(chunk);
			
		}
		this.chunks.Remove(row);
	
		if (rowSeq == ChunksSequence.First) this.chunks.AddLast(new LinkedListNode<LinkedList<Chunk>>(new LinkedList<Chunk>()));
		else if (rowSeq == ChunksSequence.Last) this.chunks.AddFirst(new LinkedListNode<LinkedList<Chunk>>(new LinkedList<Chunk>()));
		else return;

	}
	private void RemoveCol(ChunksSequence colSeq)
	{

		foreach (LinkedList<Chunk> row in this.chunks)
		{
			if (row.Count == 0) continue;
				
			LinkedListNode<Chunk> node = colSeq == ChunksSequence.First ? row.First : row.Last;
			CleanUpChunk(node.Value);
			row.Remove(node);
			
			if (colSeq == ChunksSequence.First) row.AddLast(new LinkedListNode<Chunk>(new ChunkPlaceHolder()));
			else if (colSeq == ChunksSequence.Last) row.AddFirst(new LinkedListNode<Chunk>(new ChunkPlaceHolder()));
			else return;

		}

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
		int chunksChanged = 0;

		bool movedPlusRow = currIndices.X > prevIndices.X;
		bool movedNegativeRow = currIndices.X < prevIndices.X;
		bool movedPlusCol = currIndices.Y > prevIndices.Y;
		bool movedNegativeCol = currIndices.Y < prevIndices.Y;

		if (movedPlusRow){ chunksChanged = currIndices.X - prevIndices.X; }
		else if (movedNegativeRow){ chunksChanged = prevIndices.X - currIndices.X; }
		else if (movedPlusCol){ chunksChanged = currIndices.Y - prevIndices.Y; }
		else if (movedNegativeCol){ chunksChanged = prevIndices.Y - currIndices.Y; }

		// calc player move dir
		for (int i=0; i < chunksChanged; i++)
		{
			if (movedPlusRow) // for bigger row so move +z
			{
				RemoveRow(ChunksSequence.First);
			
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(
							this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), 
							this.WorldPos.Y, 
							this.WorldTopLeftPos.Z + ((GameGlobals.ChunkRowCount - 1) * GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);
				}
				
				
			}
			if (movedNegativeRow) // for smaller row so move -z
			{
				
				RemoveRow(ChunksSequence.Last);
				
				for (float j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);
				}
				
				
			}
			if (movedPlusCol) // for bigger col so move +x
			{
				
				RemoveCol(ChunksSequence.First);
				
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(this.WorldTopLeftPos.X + GameGlobals.WorldWidth - (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);

				}
				
			}
			if (movedNegativeCol) // for smaller row so move +z
			{
				
				RemoveCol(ChunksSequence.Last);
				
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					Chunk chunk = new Chunk(
						new Godot.Vector3(this.WorldTopLeftPos.X + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)), 
						this.noise, 
						this.texture
					);
					Thread t = startChunkGenThread(chunk);
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
		chunkThread.Start();
		
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

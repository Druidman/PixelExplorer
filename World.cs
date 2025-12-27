using System;
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
	int c = 0;
	private int threadId = 0;

	List<List<Chunk>> chunks = new List<List<Chunk>>();
	List<Chunk> stagedChunks = new List<Chunk>();

	Dictionary<int, Thread> threads = new Dictionary<int, Thread>();
	List<int> threadsToJoin = new List<int>();

	float Xpos = 0f;
	bool exitApp = false;
	ImageTexture texture = new ImageTexture();

	CharacterBody3D player;
	Godot.Vector3 WorldPos = GameGlobals.StartWorldMiddle;

	bool chunksReady = false;
	Godot.Vector3 WorldTopLeftPos = GameGlobals.StartWorldMiddle - new Godot.Vector3(GameGlobals.WorldWidth / 2, 0, GameGlobals.WorldWidth / 2);

	private int getThreadId()
	{
		this.threadId++;
		return this.threadId;
	}
	public void ThreadWrapper(Action threadFunc, int id)
	{
		threadFunc();
		this.threadsToJoin.Add(id);

	}
	public static List<Chunk> genPlaceholderChunkRow()
	{
		List<Chunk> row = new List<Chunk>(GameGlobals.ChunkRowCount);
		for (int i=0; i<GameGlobals.ChunkRowCount; i++) row.Add(null);
		return row;
	}
	public override void _Ready()

	{

		player = (CharacterBody3D)GetNode("../Player");
		
		Image img = new Image();
		img.Load("res://images/customTexture.png");
		
		texture.SetImage(img);

		GenWorldBase();
		
	}
	private void StartThread(Action action)
	{
		int id = getThreadId();
		Thread t = new Thread(()=>this.ThreadWrapper(action, id));
		AddToThreads(id, t);
		t.Start();
		
	}
	private void GenWorldBase()
	{

		for (int i=0; i<GameGlobals.WorldWidth / GameGlobals.ChunkWidth; i++)
		{
			
			this.chunks.Add(genPlaceholderChunkRow());
			for (int j=0; j<GameGlobals.WorldWidth / GameGlobals.ChunkWidth; j++)
			{
				Godot.Vector3 pos = new Godot.Vector3(GameGlobals.ChunkWidth / 2,0,GameGlobals.ChunkWidth / 2) + 
					this.WorldTopLeftPos + 
					new Godot.Vector3(j*GameGlobals.ChunkWidth, 0, i*GameGlobals.ChunkWidth);
			
				Chunk chunk = new Chunk(pos, this.noise, texture);

				
				this.startChunkGenThread(chunk);
				

	
				
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
			
			CleanUpChunk(this.stagedChunks.First());
			this.stagedChunks.RemoveAt(0);
			return;
		}

		if (!this.stagedChunks.First().isMeshReady())
		{
			
			CleanUpChunk(this.stagedChunks.First());
			this.stagedChunks.RemoveAt(0);
			return ;
		}

		if (this.chunks[indices.X][indices.Y] is null)
		{
		
			CleanUpChunk(this.stagedChunks.First());
			this.stagedChunks.RemoveAt(0);
			return ;
		}


		this.stagedChunks.First().addedToTree = true;
		AddChild(this.stagedChunks.First().mesh);
		
		this.chunks[indices.X][indices.Y] = this.stagedChunks.First();

		this.stagedChunks.RemoveAt(0);

		
	}

	private void CleanUpChunk(Chunk chunk)
	{
		if (chunk is null)
		{
			return;
		}


		if (chunk.addedToTree && chunk.mesh.GetParent() != null)
		{
			RemoveChild(chunk.mesh);
		}
		
		chunk.mesh.QueueFree();
		chunk.mesh = null;
	}
	private void RemoveRow(ChunksSequence rowSeq)
	{
		if (this.chunks.Count == 0) return ;

		int rowInd;
		if (rowSeq == ChunksSequence.First) rowInd = 0;
		else if (rowSeq == ChunksSequence.Last) rowInd = this.chunks.Count - 1;
		else return;

		foreach (Chunk chunk in this.chunks[rowInd])
		{
			CleanUpChunk(chunk);
			
		}
		this.chunks.RemoveAt(rowInd);
	
		if (rowSeq == ChunksSequence.First) this.chunks.Add(genPlaceholderChunkRow());
		else if (rowSeq == ChunksSequence.Last) this.chunks.Insert(0,genPlaceholderChunkRow());
		else return;

	}
	private void RemoveCol(ChunksSequence colSeq)
	{

		foreach (List<Chunk> row in this.chunks)
		{
			if (row.Count == 0) continue;
				
			int ind = colSeq == ChunksSequence.First ? 0 : row.Count - 1;
			CleanUpChunk(row[ind]);
			row.RemoveAt(ind);
			
			if (colSeq == ChunksSequence.First) row.Add(null);
			else if (colSeq == ChunksSequence.Last) row.Insert(0,null);
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

		GD.Print(this.chunks.Count, " ", this.chunks[0].Count);
		for (int i =0; i< this.threadsToJoin.Count; i++)
		{
			this.threads[this.threadsToJoin[i]].Join();
			this.threads.Remove(this.threadsToJoin[i]);

			this.threadsToJoin.RemoveAt(i);
			i--;
		}

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

		


		updateWorldPos(GetGlobalPosFromIndices(currIndices));

		int chunksChanged = 0;

		bool movedPlusRow = currIndices.X > prevIndices.X;
		bool movedNegativeRow = currIndices.X < prevIndices.X;
		bool movedPlusCol = currIndices.Y > prevIndices.Y;
		bool movedNegativeCol = currIndices.Y < prevIndices.Y;

		if (movedPlusRow){ chunksChanged = currIndices.X - prevIndices.X; }
		else if (movedNegativeRow){ chunksChanged = prevIndices.X - currIndices.X; }
		else if (movedPlusCol){ chunksChanged = currIndices.Y - prevIndices.Y; }
		else if (movedNegativeCol){ chunksChanged = prevIndices.Y - currIndices.Y; }


		Dictionary<Godot.Vector3, Chunk> chunkToQueue = new Dictionary<Godot.Vector3, Chunk>();
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
					chunkToQueue[chunk.chunkPos] = chunk;
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
					chunkToQueue[chunk.chunkPos] = chunk;
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
					chunkToQueue[chunk.chunkPos] = chunk;

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
					chunkToQueue[chunk.chunkPos] = chunk;
				}
				
			}
		}
		if (chunkToQueue.Keys.Count > 0)
		{
			StartChunksGenerator(chunkToQueue);	
		}
		

		
		
	}
	private void AddToThreads(int id, Thread t)
	{
		this.threads[id] = t;
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
		foreach (int key in this.threads.Keys)
		{
			this.threads[key].Join();
		}
	}
	private void startChunkGenThread(Chunk chunk)
	{
		StartThread(()=>GenChunk(chunk));
	
	}

	private void StartChunksGenerator(Dictionary<Godot.Vector3, Chunk> chunksToGen)
	{	

		StartThread(()=>this.chunksGenerator(chunksToGen));

	}
	private void chunksGenerator(Dictionary<Godot.Vector3, Chunk> chunksToGen)
	{
		foreach (Godot.Vector3 key in chunksToGen.Keys){
			startChunkGenThread(chunksToGen[key]);
		}
	}
	private void GenChunk(Chunk chunk)
	{

	
		// chunk.InitMesh();
		


		this.stagedChunks.Add(chunk);
		
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using Godot;


enum ChunksSequence
{
	First,
	Last
}


public class ThreadWorkingData {
	public Chunk chunk = null;
	public bool ready;
	public ThreadWorkingData(Chunk chunk) {
		this.chunk = chunk;
		this.ready = false;
	}
	
}
public partial class World : Node3D
{

	WorldNoise noise = new WorldNoise();
	private readonly object _dataLock = new();

	List<List<Chunk>> chunks = new List<List<Chunk>>();

	private int threadId = 0;
	bool exitApp = false;


	LinkedList<ThreadWorkingData> threadsWorkingData = new LinkedList<ThreadWorkingData>();
	List<Thread> threads = new List<Thread>();

	
	ImageTexture texture = new ImageTexture();

	
	Godot.Vector3 WorldPos = GameGlobals.StartWorldMiddle;
	Godot.Vector3 WorldTopLeftPos = GameGlobals.StartWorldMiddle - new Godot.Vector3(GameGlobals.WorldWidth / 2, 0, GameGlobals.WorldWidth / 2);

	
	CharacterBody3D player;
	private int getThreadId()
	{
		this.threadId++;
		return this.threadId;
	}
	private static List<Chunk> genPlaceholderChunkRow()
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
		Thread t = new Thread(()=>action());
		this.threads.Add(t);
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
			

				
				this.startChunkGenThread(pos);
				

	
				
			}	
		}
	}

	private bool UpdateChunkGenThread(ThreadWorkingData data)
	{
		Godot.Vector2I indices = GetIndicesFromLocalWorldPos(ConvertToLocalWorldPos(data.chunk.chunkPos));
		if (
			indices.X < 0 || indices.X >= GameGlobals.ChunkRowCount ||
			indices.Y < 0 || indices.Y >= GameGlobals.ChunkRowCount
		) return false;

		
		
		if (!data.chunk.isMeshReady())
		{
			data.chunk.BuildChunkMesh(this.texture);
		}

		if (!data.chunk.isMeshReady()) return false;
	


		if (this.chunks[indices.X][indices.Y] is not null) return false;


		data.chunk.addedToTree = true;
		AddChild(data.chunk.mesh);
		
		this.chunks[indices.X][indices.Y] = data.chunk;

		return true;
	}

	private void UpdateChunkGenThreads()
	{
		lock (_dataLock)
		{


			LinkedListNode<ThreadWorkingData> node = this.threadsWorkingData.First;
			while (node != null)
			{
				var next = node.Next;
				if (node.Value.ready)
				{
					GD.Print("Regular");

					if (!UpdateChunkGenThread(node.Value))
					{
						CleanUpChunk(node.Value.chunk);
					}

					this.threadsWorkingData.Remove(node);
				}

				node = next;
			}
				
		}

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
		
		if (chunk.mesh != null) chunk.mesh.QueueFree();
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
	
	
	private void JoinFinishedThreads()
	{
		for (int i =0; i< this.threads.Count; i++)
		{
			if (!this.threads[i].IsAlive)
			{
				this.threads[i].Join();
				this.threads.RemoveAt(i);
				i--;
			}
		}
	}
	private void JoinAllThreads()
	{
		for (int i =0; i< this.threads.Count; i++)
		{
			this.threads[i].Join();	
		}
		this.threads.Clear();
	}

	private void UpdateChunks()
	{
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


		List<Godot.Vector3> chunkToQueue = new List<Godot.Vector3>();
		// calc player move dir
		for (int i=0; i < chunksChanged; i++)
		{
			if (movedPlusRow) // for bigger row so move +z
			{
				RemoveRow(ChunksSequence.First);
			
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					
					chunkToQueue.Add(new Godot.Vector3(
							this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), 
							this.WorldPos.Y, 
							this.WorldTopLeftPos.Z + ((GameGlobals.ChunkRowCount - 1) * GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2)));
				}
				
				
			}
			if (movedNegativeRow) // for smaller row so move -z
			{
				
				RemoveRow(ChunksSequence.Last);
				
				for (float j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					
					chunkToQueue.Add(new Godot.Vector3(this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2)));
				}
				
				
			}
			if (movedPlusCol) // for bigger col so move +x
			{
				
				RemoveCol(ChunksSequence.First);
				
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
				
					chunkToQueue.Add(new Godot.Vector3(this.WorldTopLeftPos.X + GameGlobals.WorldWidth - (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)));

				}
				
			}
			if (movedNegativeCol) // for smaller row so move +z
			{
				
				RemoveCol(ChunksSequence.Last);
				
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					
					chunkToQueue.Add(new Godot.Vector3(this.WorldTopLeftPos.X + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)));
				}
				
			}
		}
		if (chunkToQueue.Count > 0)
		{
			chunksGenerator(chunkToQueue);
		}
	}
	public override void _Process(double delta)
	{

		JoinFinishedThreads();
		

		UpdateChunkGenThreads();

		UpdateChunks();
		
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
		JoinAllThreads();
	}
	private void startChunkGenThread(Godot.Vector3 pos)
	{	
		ThreadWorkingData data = new ThreadWorkingData(null);
		lock (_dataLock)
		{
			this.threadsWorkingData.AddLast(data);
			// GenChunk(this.threadsWorkingData.Last(), pos);
			StartThread(()=>GenChunk(data, pos));
		}
	}

	private void chunksGenerator(List<Godot.Vector3> chunksToGen)
	{
		foreach (Godot.Vector3 pos in chunksToGen){
			startChunkGenThread(pos);
		}
	}
	private void GenChunk(ThreadWorkingData data, Godot.Vector3 pos)
	{
		Chunk chunk = new Chunk(pos, this.noise);
		chunk.GenerateChunkMesh();
		lock (this._dataLock)
		{
			data.chunk = chunk;
			data.ready = true;
			
		}
		
		
	}
}

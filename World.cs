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
	public List<Chunk> chunks = new List<Chunk>();
	public List<bool> chunksDone = new List<bool>();
	public List<bool> chunksInserted = new List<bool>();
	public bool ready;
	public ThreadWorkingData() {
		this.ready = false;
	}
	
	
}
public partial class World : Node3D
{
	int ind = 0;
	WorldNoise noise = new WorldNoise();
	private readonly object _dataLock = new();
	private readonly object _chunkLock = new();

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
			lock (_chunkLock)
			{
				this.chunks.Add(genPlaceholderChunkRow());
			}
			
			for (int j=0; j<GameGlobals.WorldWidth / GameGlobals.ChunkWidth; j++)
			{
				Godot.Vector3 pos = new Godot.Vector3(GameGlobals.ChunkWidth / 2,0,GameGlobals.ChunkWidth / 2) + 
					this.WorldTopLeftPos + 
					new Godot.Vector3(j*GameGlobals.ChunkWidth, 0, i*GameGlobals.ChunkWidth);
			

				lock (_chunkLock)
				{
					this.startChunkGenThread([pos]);
				}
				

	
				
			}	
		}
	}

	private bool UpdateChunkGenThread(ThreadWorkingData data)
	{
		
		for (int i = 0; i < data.chunks.Count; i++)
		{
			if (data.chunksInserted[i] || !data.chunksDone[i])
			{
				continue;
			}

			Chunk chunk = data.chunks[i];

			if (chunk == null)
			{
				continue;
			}


			Godot.Vector2I indices = GetIndicesFromLocalWorldPos(ConvertToLocalWorldPos(chunk.chunkPos));
			if (
				indices.X < 0 || indices.X >= GameGlobals.ChunkRowCount ||
				indices.Y < 0 || indices.Y >= GameGlobals.ChunkRowCount
			) continue;


			GD.Print("Add?", this.ind);
			ind++;
			lock (_chunkLock)
			{
				CleanUpChunk(this.chunks[indices.X][indices.Y]);
			}

			chunk.addedToTree = true;
			CallDeferred(Node3D.MethodName.AddChild, chunk.mesh);
			// AddChild(chunk.mesh);
			lock (_chunkLock)
			{
				this.chunks[indices.X][indices.Y] = chunk;
			}
			
			lock (_dataLock)
			{
				data.chunksInserted[i] = true;	
			}

			
		 	
		
			// break;
			
		}
	
		bool isThreadReady = true;
		for (int i = 0; i < data.chunks.Count; i++)
		{
			Chunk chunk = data.chunks[i];
			if (chunk == null)
			{
				isThreadReady = false;
				break;
			}
			Godot.Vector2I indices = GetIndicesFromLocalWorldPos(ConvertToLocalWorldPos(chunk.chunkPos));
			if (
				!data.chunksInserted[i]
				// indices.X >= 0 && indices.X < GameGlobals.ChunkRowCount &&
				// indices.Y >= 0 && indices.Y < GameGlobals.ChunkRowCount
			)
			{
				isThreadReady = false;
				break;
			}
		}
		if (isThreadReady)
		{
			return true;
		}
		

		return false;
	}

	private void UpdateChunkGenThreads()
	{
		

		
			LinkedListNode<ThreadWorkingData> node = this.threadsWorkingData.First;
			// GD.Print(this.threadsWorkingData.Count);
			while (node != null)
			{
				var next = node.Next;
				
				
				
				if (UpdateChunkGenThread(node.Value))
				{
					lock (_dataLock)
					{
						this.threadsWorkingData.Remove(node);
						
					}				

				}


				node = next;
				
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
			// RemoveChild(chunk.mesh);
			CallDeferred(Node3D.MethodName.RemoveChild, chunk.mesh);
		}

		// if (chunk.mesh != null) chunk.mesh.QueueFree();
		if (chunk.mesh != null) chunk.mesh.CallDeferred(MeshInstance3D.MethodName.QueueFree);
		chunk.mesh = null;
	}
	private void RemoveRow(ChunksSequence rowSeq)
	{
		lock (_chunkLock)
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

	}
	private void RemoveCol(ChunksSequence colSeq)
	{
		lock (_chunkLock)
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

		

		bool movedPlusRow = currIndices.X > prevIndices.X;
		bool movedNegativeRow = currIndices.X < prevIndices.X;
		bool movedPlusCol = currIndices.Y > prevIndices.Y;
		bool movedNegativeCol = currIndices.Y < prevIndices.Y;


		int chunksChangedRow = 0;
		int chunksChangedCol = 0;

		if (movedPlusRow){ chunksChangedRow = currIndices.X - prevIndices.X; }
		else if (movedNegativeRow){ chunksChangedRow = prevIndices.X - currIndices.X; }

		if (movedPlusCol){ chunksChangedCol = currIndices.Y - prevIndices.Y; }
		else if (movedNegativeCol){ chunksChangedCol = prevIndices.Y - currIndices.Y; }


		List<List<Godot.Vector3>> chunksToQueue = new List<List<Godot.Vector3>>();
		// calc player move dir
		for (int i=0; i < chunksChangedRow; i++)
		{
			if (movedPlusRow) // for bigger row so move +z
			{
				RemoveRow(ChunksSequence.First);

				
				List<Godot.Vector3> row = new List<Vector3>();

				
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					
					row.Add(new Godot.Vector3(
							this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), 
							this.WorldPos.Y, 
							this.WorldTopLeftPos.Z + ((GameGlobals.ChunkRowCount - 1) * GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2)));
				}
				chunksToQueue.Add(row);
				
				
			}
			if (movedNegativeRow) // for smaller row so move -z
			{
				
				RemoveRow(ChunksSequence.Last);

				List<Godot.Vector3> row = new List<Vector3>();
				
				for (float j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					
					row.Add(new Godot.Vector3(this.WorldTopLeftPos.X + (j*GameGlobals.ChunkWidth) + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2)));
				}
				chunksToQueue.Add(row);
				
				
			}
		}
		for (int i=0; i < chunksChangedCol; i++)
		{
			if (movedPlusCol) // for bigger col so move +x
			{
				
				RemoveCol(ChunksSequence.First);
				List<Godot.Vector3> col = new List<Vector3>();
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
				
					col.Add(new Godot.Vector3(this.WorldTopLeftPos.X + GameGlobals.WorldWidth - (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)));

				}
				chunksToQueue.Add(col);
				
			}
			if (movedNegativeCol) // for smaller row so move +z
			{
				
				RemoveCol(ChunksSequence.Last);
				List<Godot.Vector3> col = new List<Vector3>();
				for (int j=0; j < GameGlobals.ChunkRowCount; j++)
				{
					
					col.Add(new Godot.Vector3(this.WorldTopLeftPos.X + (GameGlobals.ChunkWidth / 2), this.WorldPos.Y, this.WorldTopLeftPos.Z + (GameGlobals.ChunkWidth / 2) + (j*GameGlobals.ChunkWidth)));
				}
				chunksToQueue.Add(col);
				
			}
		}
		
		if (chunksToQueue.Count > 0)
		{
			chunksGenerator(chunksToQueue);
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
		lock (_chunkLock)
		{
			return this.chunks.ElementAt(indices.X).ElementAt(indices.Y);
		}
	}
	public Godot.Vector3 ConvertToLocalWorldPos(Godot.Vector3 pos)
	{
		return pos - this.WorldTopLeftPos;
	}

	public Godot.Vector2I GetIndicesFromLocalWorldPos(Godot.Vector3 localPos)
	{
		return new Godot.Vector2I((int)(localPos.Z / GameGlobals.ChunkWidth), (int)(localPos.X / GameGlobals.ChunkWidth)) ;
	}
	public override void _ExitTree()
	{
		this.exitApp = true;
		JoinAllThreads();
	}
	private void startChunkGenThread( List<Godot.Vector3> positions)
	{	
		ThreadWorkingData data = new ThreadWorkingData();
		lock (_dataLock)
		{
			this.threadsWorkingData.AddLast(data);

			StartThread(()=>GenChunk(data, positions));
		}
	}

	private void chunksGenerator(List<List<Godot.Vector3>> chunksToGen)
	{
		GD.Print(chunksToGen);
		foreach (List<Godot.Vector3> chunksBatch in chunksToGen){
			startChunkGenThread(chunksBatch);
		}
	}
	private void GenChunk(ThreadWorkingData data, List<Godot.Vector3> positions)
	{
		for (int x = 0; x<positions.Count; x++)
		{
			lock (this._dataLock)
			{
				data.chunks.Add(null);
				data.chunksDone.Add(false);
				data.chunksInserted.Add(false);
			}
			
		}

		int i = 0;
		foreach (Godot.Vector3 pos in positions)
		{
			Chunk chunk = new Chunk(pos, this.noise);
			chunk.GenerateChunkMesh();
			chunk.BuildChunkMesh(this.texture);
			lock (this._dataLock)
			{
				data.chunks[i] = chunk;
				data.chunksDone[i] = true;

	
			}
			i++;
			
		}
		lock (this._dataLock)
		{
			data.ready = true;
		}
		
		
		
	}
}

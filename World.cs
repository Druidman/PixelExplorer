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
	public bool chunkDone = false;
	public bool ready = false;
	
	
}
public partial class World : Node3D
{
	int ind = 0;
	WorldNoise noise = new WorldNoise();
	private readonly object _dataLock = new();

	Dictionary<Godot.Vector3, Chunk> chunks = new Dictionary<Godot.Vector3, Chunk>();

	private int threadId = 0;
	bool exitApp = false;


	LinkedList<ThreadWorkingData> threadsWorkingData = new LinkedList<ThreadWorkingData>();
	List<Thread> threads = new List<Thread>();

	
	ImageTexture texture = new ImageTexture();

	
	Godot.Vector3 WorldPos = GameGlobals.StartWorldMiddle;

	int worldChunkRadius = GameGlobals.chunkRadius;
	float maxChunkDist = (GameGlobals.chunkRadius) * GameGlobals.ChunkWidth;
	CharacterBody3D player;
	private int getThreadId()
	{
		this.threadId++;
		return this.threadId;
	}
	public override void _Ready()

	{

		player = (CharacterBody3D)GetNode("../Player");
		
		Image img = new Image();
		img.Load("res://images/customTexture.png");
		
		texture.SetImage(img);
		
	}
	private void StartThread(Action action)
	{
		int id = getThreadId();
		Thread t = new Thread(()=>action());
		this.threads.Add(t);
		t.Start();
		
	}

	
	private bool UpdateChunkGenThread(ThreadWorkingData data)
	{
		
		if (!data.ready || !data.chunkDone || data.chunk == null)
		{
			return false;
		}
		
		if (this.chunks.GetValueOrDefault(data.chunk.chunkPos) != null)
		{
			
			CleanUpChunk(this.chunks.GetValueOrDefault(data.chunk.chunkPos));	
			
			
		}

		data.chunk.addedToTree = true;
		CallDeferred(Node3D.MethodName.AddChild, data.chunk.mesh);
		this.chunks[data.chunk.chunkPos] = data.chunk;

		return true;
	}
	private void UpdateChunkGenThreads()
	{
			
		LinkedListNode<ThreadWorkingData> node = this.threadsWorkingData.First;
		
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


	public Godot.Vector3I getDistanceFromWorldMiddleInChunksCount(Godot.Vector3 pos)
	{
		
		return (Godot.Vector3I)(pos - this.WorldPos).Abs() / (int)GameGlobals.ChunkWidth;
		
		
	}

	private void UpdateChunks()
	{

		Godot.Vector2 newWorldPos = (
			new Godot.Vector2(this.player.Position.X, this.player.Position.Z) / GameGlobals.ChunkWidth).Floor() * GameGlobals.ChunkWidth; 
		

		if (this.chunks.GetValueOrDefault(new Godot.Vector3(newWorldPos.X, this.WorldPos.Y, newWorldPos.Y)) != null)
		{
			if (this.chunks.GetValueOrDefault(new Godot.Vector3(newWorldPos.X, this.WorldPos.Y, newWorldPos.Y)) == this.chunks.GetValueOrDefault(this.WorldPos))
			{
				return;
			}
		}

		
		updateWorldPos(new Godot.Vector3(newWorldPos.X, this.WorldPos.Y, newWorldPos.Y));

		
		for (
			int x = (int)this.WorldPos.X - ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth); 
			x <= (int)this.WorldPos.X + ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth);
			x+=GameGlobals.ChunkWidth
		)
		{
		
			// for (
			// 	int z = (int)this.WorldPos.Z + (int)((getDistanceFromWorldMiddleInChunksCount(new Godot.Vector3(x, this.WorldPos.Y, this.WorldPos.Z)).X - (this.worldChunkRadius - 1)) * GameGlobals.ChunkWidth); 			
			// 	z <= (int)this.WorldPos.Z - (int)((getDistanceFromWorldMiddleInChunksCount(new Godot.Vector3(x, this.WorldPos.Y, this.WorldPos.Z)).X - (this.worldChunkRadius - 1))  * GameGlobals.ChunkWidth);
			// 	z+=GameGlobals.ChunkWidth
			// )
			for (
				int z = (int)this.WorldPos.Z - ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth); 
				z <= (int)this.WorldPos.Z + ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth);
				z+=GameGlobals.ChunkWidth
			)
			{
				
				
				Godot.Vector3 pos = new Godot.Vector3(x,this.WorldPos.Y,z);
				if (this.chunks.GetValueOrDefault(pos) == null)
				{
					
					RequestChunkGenAt(pos);
					this.chunks[pos] = new Chunk(pos, this.noise); // as placeholder

				}



				
			
			}
			
		}

		// now removing old ones
		foreach (Godot.Vector3 key in this.chunks.Keys)
		{
			Chunk chunk = this.chunks[key];
			if (!chunk.addedToTree)
			{
				
				continue;
			}


			if (!CheckIfPosFitsInWorld(chunk.chunkPos))
			{
				CleanUpChunk(chunk);
				this.chunks.Remove(key);
				// this.chunks[key] = null;
			}
		}
	}

	private bool CheckIfPosFitsInWorld(Godot.Vector3 pos)
	{
		Godot.Vector3 distance  = getDistanceFromWorldMiddleInChunksCount(pos);
		if (
			distance.X > this.worldChunkRadius - 1 ||
			distance.Z > this.worldChunkRadius - 1
		)
		{
				
			return false;
		}
	
		
		return true;

	}

	private void RequestChunkGenAt(Godot.Vector3 pos)
	{
		startChunkGenThread(pos);
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
	}

	public override void _ExitTree()
	{
		this.exitApp = true;
		JoinAllThreads();
	}
	private void startChunkGenThread(Godot.Vector3 position)
	{	
		ThreadWorkingData data = new ThreadWorkingData();
		lock (_dataLock)
		{
			this.threadsWorkingData.AddLast(data);
			
		}
		StartThread(()=>GenChunk(data, position));
		
	}
	private void GenChunk(ThreadWorkingData data, Godot.Vector3 position)
	{
		
		Chunk chunk = new Chunk(position, this.noise);
		chunk.GenerateChunkMesh();
		chunk.BuildChunkMesh(this.texture);
		lock (this._dataLock)
		{
			data.chunk = chunk;
			data.chunkDone = true;
			data.ready = true;

		}

		
	}
}

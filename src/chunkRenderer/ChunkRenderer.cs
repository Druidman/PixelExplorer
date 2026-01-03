using System;
using System.Collections.Generic;
using System.Threading;
using Godot;


public class ThreadWorkingData {
	public Chunk chunk = null;
	public bool chunkDone = false;
	public bool ready = false;
	
	
}

public partial class ChunkRenderer : Node3D
{
	Godot.Vector3 origin;

    [Export]
	Player player;

	Dictionary<Godot.Vector3, Chunk> chunks = new Dictionary<Godot.Vector3, Chunk>();
	LinkedList<ThreadWorkingData> threadsWorkingData = new LinkedList<ThreadWorkingData>();
	
	int worldChunkRadius = GameGlobals.chunkRadius;
	float maxChunkDist = (GameGlobals.chunkRadius) * GameGlobals.ChunkWidth;

	

	private readonly object _dataLock = new();
	
	public override void _Ready()
	{
		this.origin = player.Position;
	}

	private void StartThread(Action action)
    {
        Thread t = new Thread(() => action());
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
		data.chunk.BuildChunkMesh(GameGlobals.texture);
		

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
					break;
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

			CallDeferred(Node3D.MethodName.RemoveChild, chunk.mesh);
		}

		if (chunk.mesh != null) chunk.mesh.CallDeferred(MeshInstance3D.MethodName.QueueFree);
		chunk.mesh = null;
	}
	private void UpdateChunks()
	{

		Godot.Vector2 newWorldPos = (
			new Godot.Vector2(this.player.Position.X, this.player.Position.Z) / GameGlobals.ChunkWidth
		).Floor() * GameGlobals.ChunkWidth; 
		

		if (this.chunks.GetValueOrDefault(new Godot.Vector3(newWorldPos.X, this.origin.Y, newWorldPos.Y)) != null)
		{
			if (
				this.chunks.GetValueOrDefault(new Godot.Vector3(newWorldPos.X, this.origin.Y, newWorldPos.Y)) 
				== 
				this.chunks.GetValueOrDefault(this.origin)
			)
			{
				return;
			}
		}

		
		origin = new Godot.Vector3(newWorldPos.X, this.origin.Y, newWorldPos.Y);

		
		for (
			int x = (int)this.origin.X - ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth); 
			x <= (int)this.origin.X + ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth);
			x+=GameGlobals.ChunkWidth
		)
		{
			for (
				int z = (int)this.origin.Z - ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth); 
				z <= (int)this.origin.Z + ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth);
				z+=GameGlobals.ChunkWidth
			)
			{
				
				
				Godot.Vector3 pos = new Godot.Vector3(x,this.origin.Y,z);
				if (this.chunks.GetValueOrDefault(pos) == null)
				{
					
					RequestChunkGenAt(pos);
					this.chunks[pos] = new Chunk(pos); // as placeholder

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
		
			}
		}

	}
	public void GenChunkCollisions(){
		List<Godot.Vector3> requiredCollisions = [
			this.origin,	
			this.origin + new Godot.Vector3(GameGlobals.ChunkWidth,0,0),
			this.origin + new Godot.Vector3(-GameGlobals.ChunkWidth,0,0),
			this.origin + new Godot.Vector3(0,0,GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3(0,0,-GameGlobals.ChunkWidth),

			this.origin + new Godot.Vector3(GameGlobals.ChunkWidth,0,GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3(GameGlobals.ChunkWidth,0,-GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3(-GameGlobals.ChunkWidth,0,-GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3(-GameGlobals.ChunkWidth,0,GameGlobals.ChunkWidth),
		];
		foreach (Godot.Vector3 pos in requiredCollisions)
		{
			Chunk cChunk = this.chunks.GetValueOrDefault(pos);
			if (cChunk == null)
			{
				return;
			}
			if (!cChunk.meshReady)
			{
				return;
			}
			if (cChunk.chunkCollisionState != ChunkCollisionState.GENERATED)
			{
				cChunk.GenerateChunkCollision();
			}
		}
	}
	public Godot.Vector3I getDistanceFromOriginInChunksCount(Godot.Vector3 pos)
	{
		return (Godot.Vector3I)(pos - this.origin).Abs() / (int)GameGlobals.ChunkWidth;
	}

	private bool CheckIfPosFitsInWorld(Godot.Vector3 pos)
	{
		Godot.Vector3 distance = getDistanceFromOriginInChunksCount(pos);
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
		
		Chunk chunk = new Chunk(position);
		chunk.GenerateChunkMesh();
		
		lock (this._dataLock)
		{
			data.chunk = chunk;
			data.chunkDone = true;
			data.ready = true;

		}

		
	}


    public override void _Process(double delta)
    {
        UpdateChunks();
        UpdateChunkGenThreads();
        GenChunkCollisions();
    }
}

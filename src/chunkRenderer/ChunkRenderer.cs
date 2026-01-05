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
	private Godot.Vector3 origin;

	[Export]
	Player player;

	Dictionary<Godot.Vector3, Chunk> chunks = new Dictionary<Godot.Vector3, Chunk>();

	LinkedList<Chunk> pendingRemove = new LinkedList<Chunk>();
	LinkedList<Chunk> pendingAdd = new LinkedList<Chunk>();
	
	int worldChunkRadius = GameGlobals.chunkRadius;
	float maxChunkDist = (GameGlobals.chunkRadius) * GameGlobals.ChunkWidth;

	bool AllowThreadBlockInChunkGen = false;

	private readonly object _dataLock = new();
	
	
	public override void _Ready()
	{
		this.origin = new Godot.Vector3(player.Position.X, 0, player.Position.Z);
		
	}

	public Godot.Vector3 GetOrigin(){ return this.origin; }

	private void StartThread(Action action)
	{
		Thread t = new Thread(() => action());
		t.Start();
	}

	
	private bool CommitChunk(Chunk chunk)
	{
		if (chunk == null)
		{
			return false;
		}
		
		if (this.chunks.GetValueOrDefault(chunk.chunkPos) != null)
		{
			CleanUpChunk(this.chunks.GetValueOrDefault(chunk.chunkPos));	

		}
		chunk.BuildChunkMesh();
		chunk.ApplyChunkCollision();
		

		chunk.addedToTree = true;
		
		CallDeferred(Node3D.MethodName.AddChild, chunk);

		this.chunks[chunk.chunkPos] = chunk;

		return true;
	}
	private void CommitChunks()
	{
		Chunk chunk;
		lock (_dataLock)
		{
			LinkedListNode<Chunk> node = this.pendingAdd.First;	
			if (node == null)
			{
				return;
			}
			chunk = node.Value;
		}
		
		CommitChunk(chunk);
		
		lock (_dataLock)
		{
			this.pendingAdd.RemoveFirst();
		}
		
		
	}
	private void CleanUpChunk(Chunk chunk)
	{
		if (chunk is null)
		{
			return;
		}
		if (chunk == GameGlobals.placeholderChunk){
			return;
		}
		if (!chunk.addedToTree)
		{
			return;
		}


		chunk.Visible = false;
		chunk.ProcessMode = ProcessModeEnum.Disabled;
		chunk.disabled = true;

		// if (chunk.GetParent() != null)
		// {
		// 	RemoveChild(chunk);
		
		// }

		
		// chunk.QueueFree();
		

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

		GenNewChunks();
		


		CleanUpOldChunks();

		

	}
	private void GenNewChunks()
	{
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
				if (
					GameGlobals.game.world.CheckIfPosFitsInWorld(pos) &&
					this.chunks.GetValueOrDefault(pos) == null 
					// if is null then chunk in given pos is not scheduled and non existent
				)
				{
					RequestChunkGenAt(pos);
					this.chunks[pos] = GameGlobals.placeholderChunk; // as placeholder, so we won't schedule another gen
				}
				else if (
					this.chunks.GetValueOrDefault(pos) != GameGlobals.placeholderChunk && 
					GameGlobals.game.world.CheckIfPosFitsInWorld(pos) &&
					this.chunks.GetValueOrDefault(pos) != null 
				)
				{
					this.chunks[pos].disabled = false;
					this.chunks[pos].Visible = true;
					this.chunks[pos].ProcessMode = ProcessModeEnum.Inherit;
				}
			
			}
			
		}
	}
	private void CleanUpOldChunks()
	{
		foreach (Godot.Vector3 key in this.chunks.Keys)
		{
			Chunk chunk = this.chunks[key];

			if (chunk.addedToTree && !CheckIfPosFitsInRenderDistance(chunk.chunkPos))
			{
				pendingRemove.AddLast(chunk);
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
				cChunk.ApplyChunkCollision();
			}
		}
	}
	public Godot.Vector3I getDistanceFromOriginInChunksCount(Godot.Vector3 pos)
	{
		return (Godot.Vector3I)(pos - this.origin).Abs() / (int)GameGlobals.ChunkWidth;
	}

	private bool CheckIfPosFitsInRenderDistance(Godot.Vector3 pos)
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
		
		Chunk chunk = GameGlobals.chunkScene.Instantiate<Chunk>();
		StartThread(()=>GenChunk(chunk, position));
		
	}
	private void GenChunk(Chunk chunk, Godot.Vector3 position)
	{

		chunk.Initialize(position);
		
		chunk.GenerateChunkMesh();
		
		lock (_dataLock)
		{
			this.pendingAdd.AddLast(chunk);
		}
	}


	public override void _Process(double delta)
	{
		UpdateChunks();
		
		GenChunkCollisions();

		if (this.pendingRemove.Count > 0) RemoveChunks();

		int addCount;
		lock (_dataLock)
		{
			addCount = this.pendingAdd.Count;
		}
		if (addCount > 0) CommitChunks();
		
		

		
	}

	public void RemoveChunks()
	{
		var node = this.pendingRemove.First;
		while (node != null)
		{
			var next = node.Next;

			Chunk chunk = node.Value;

			if (chunk != null)
			{
				CleanUpChunk(chunk);
				this.pendingRemove.Remove(node);
				

				// if (this.chunks[chunk.chunkPos] == chunk)
				// {
				// 	this.chunks.Remove(chunk.chunkPos);
				// }
			
				// break;
			}

			node = next;
		
			

		}
		
	}
}

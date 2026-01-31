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
	public Godot.Vector3I origin {get; private set;}

	[Export]
	Player player;


	[Export]
	World world;
	LinkedList<Chunk> pendingAdd = new LinkedList<Chunk>();


	private Dictionary<Godot.Vector3I, Chunk> queuedChunks = new Dictionary<Godot.Vector3I, Chunk>();
	
	int worldChunkRadius = GameGlobals.chunkRadius;

	private readonly object _dataLock = new();
	
	public override void _Ready()
	{
		this.origin = new Godot.Vector3I((int)player.Position.X, 0, (int)player.Position.Z);
		
	}

	private void StartThread(Action action)
	{
		Thread t = new Thread(() => action());
		t.Start();
	}

	
	private bool CommitChunk(Chunk chunk)
	{
		if (chunk == null) return false;
		if (!this.queuedChunks.ContainsKey(chunk.chunkPos))
		 throw new Exception("Attemptiong to commit unqueued chunk");
		if (chunk.isAddedToTree) return false;

		
		chunk.ApplyChunkTileMesh();
		
		chunk.CreateChunkCollision();
		
		
		CallDeferred(Node3D.MethodName.AddChild, chunk);

		this.queuedChunks.Remove(chunk.chunkPos);

		
		
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
		chunk.QueueFree();		
	}

	private void HideChunk(Chunk chunk)
	{
		if (chunk is null) return;
		if (!chunk.isAddedToTree) return;

		chunk.Visible = false;
		chunk.ProcessMode = ProcessModeEnum.Disabled;
		chunk.PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;

		
	}

	private void ShowChunk(Chunk chunk)
	{
		if (chunk is null) return;
		if (!chunk.isAddedToTree) return;

		chunk.Visible = true;
		chunk.ProcessMode = ProcessModeEnum.Inherit;
		chunk.PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Inherit;

		
	}
	
	private void UpdateChunks()
	{

		if (this.world.GetChunkPositionFromGlobalPos(this.player.GlobalPosition) != this.world.GetChunkPositionFromGlobalPos(this.origin))
		{
			origin = this.world.GetChunkPositionFromGlobalPos(this.player.GlobalPosition);
		}

		GenNewChunks();
		


		HideChunks();
		ShowChunks();

		

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
				Godot.Vector3I pos = new Godot.Vector3I(x,this.origin.Y,z);

				if (!this.world.CheckIfValidGlobalPosition(pos))
				{
					continue;
				}

				Chunk chunk = this.world.GetChunkAtExactPos(pos);

				if (
					chunk == null
				)
				{
					Chunk nChunk = this.world.CreateChunkAtPosition(pos);
					if (nChunk == null) throw new Exception("Something wrong with chunk positioning");

					if (!this.world.UpdateChunkAtPosition(pos, nChunk)) throw new Exception("Something wrong with chunk update");

					RequestChunkGen(nChunk);
				}
				
				else if (
					!chunk.isAddedToTree &&
					!this.queuedChunks.ContainsKey(pos)
				) 
				// we found chunk that should be queued for add because it is in render distance
				// essentialy just chunk that probably was created not by chunkRenderer
				{
					lock (_dataLock)
					{
						RequestChunkGen(chunk);
						GD.Print("add");
					}
				}
			
			}
			
		}
	}
	private void HideChunks()
	{
		foreach (Godot.Vector3I key in this.world.GetAvailableChunkPositions())
		{
			Chunk chunk = this.world.GetChunkAtExactPos(key);

			if (chunk.isAddedToTree && !CheckIfPosFitsInRenderDistance(chunk.chunkPos))
			{
				HideChunk(chunk);
			}
		}
	}

	private void ShowChunks()
	{
		foreach (Godot.Vector3I key in this.world.GetAvailableChunkPositions())
		{
			Chunk chunk = this.world.GetChunkAtExactPos(key);

			if (chunk.isAddedToTree && CheckIfPosFitsInRenderDistance(chunk.chunkPos))
			{
				ShowChunk(chunk);
			}
		}
	}
	public void GenChunkCollisions(){
		List<Godot.Vector3I> requiredCollisions = [
			this.origin,	
			this.origin + new Godot.Vector3I(GameGlobals.ChunkWidth,0,0),
			this.origin + new Godot.Vector3I(-GameGlobals.ChunkWidth,0,0),
			this.origin + new Godot.Vector3I(0,0,GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3I(0,0,-GameGlobals.ChunkWidth),

			this.origin + new Godot.Vector3I(GameGlobals.ChunkWidth,0,GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3I(GameGlobals.ChunkWidth,0,-GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3I(-GameGlobals.ChunkWidth,0,-GameGlobals.ChunkWidth),
			this.origin + new Godot.Vector3I(-GameGlobals.ChunkWidth,0,GameGlobals.ChunkWidth),
		];
		
		
		foreach (Godot.Vector3I pos in requiredCollisions)
		{
			Chunk cChunk = this.world.GetChunkAtExactPos(pos);

			if (cChunk == null) continue;
			if (
				!cChunk.isAddedToTree ||
				!cChunk.isBlockMeshGenerated ||
				cChunk.isChunkCollisionShapeGenerated
			) continue;
			

			cChunk.CreateChunkCollision();
			
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
	private void RequestChunkGen(Chunk chunk)
	{
		if (this.queuedChunks.ContainsKey(chunk.chunkPos)) 
			throw new Exception("Possible copy of chunk to generate queued!");
		
		this.queuedChunks[chunk.chunkPos] = chunk;
		startChunkGenThread(chunk);
	}
	

	private void startChunkGenThread(Chunk chunk)
	{	
		
		StartThread(()=>GenChunk(chunk));
	}
	private void GenChunk(Chunk chunk)
	{
		chunk.GenerateChunk();
		
		lock (_dataLock)
		{
			this.pendingAdd.AddLast(chunk);
		}
	}


	public override void _Process(double delta)
	{
		UpdateChunks();
		
		GenChunkCollisions();

		int addCount;
		lock (_dataLock)
		{
			addCount = this.pendingAdd.Count;
		}
		if (addCount > 0) CommitChunks();
		
		

		
	}
}

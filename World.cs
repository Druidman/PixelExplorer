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

	int worldChunkRadius = 3;
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
		
		
	}

	

	private void UpdateChunkGenThreads()
	{
			
		

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
		int i =0;
		for (
			int x = (int)this.WorldPos.X - ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth); 
			x <= (int)this.WorldPos.X + ((this.worldChunkRadius - 1) * GameGlobals.ChunkWidth);
			x+=GameGlobals.ChunkWidth
		)
		{
		
			for (
				int z = (int)this.WorldPos.Z + (int)((getDistanceFromWorldMiddleInChunksCount(new Godot.Vector3(x, this.WorldPos.Y, this.WorldPos.Z)).X - (this.worldChunkRadius - 1)) * GameGlobals.ChunkWidth); 			
				z <= (int)this.WorldPos.Z - (int)((getDistanceFromWorldMiddleInChunksCount(new Godot.Vector3(x, this.WorldPos.Y, this.WorldPos.Z)).X - (this.worldChunkRadius - 1))  * GameGlobals.ChunkWidth);
				z+=GameGlobals.ChunkWidth
			)
			{
				
				GD.Print(i, ": ", x ,  " " , z, " ");
				
				
				i++;
			}
			
		}
	}
	public override void _Process(double delta)
	{

		// JoinFinishedThreads();
		

		// UpdateChunkGenThreads();

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

			StartThread(()=>GenChunk(data, position));
		}
	}

	private void chunksGenerator(Dictionary<Godot.Vector3, bool> chunksToGen)
	{
		
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

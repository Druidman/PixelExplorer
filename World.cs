using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;



public partial class World : Node3D
{

	WorldNoise noise = new WorldNoise();

	List<List<Chunk>> chunks = new List<List<Chunk>>();
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
				Godot.Vector3 pos = new Godot.Vector3(GameGlobals.ChunkWidth,0,GameGlobals.ChunkWidth) + 
					this.WorldTopLeftPos + 
					new Godot.Vector3(j*GameGlobals.ChunkWidth, 0, i*GameGlobals.ChunkWidth);

				Chunk chunk = new Chunk(pos, this.noise, texture);
				this.chunks[i].Add(chunk);

				Thread t = this.startChunkGenThread(
					chunk

				);
				GD.Print("Count: " + count);
				count++;
				Thread.Sleep(100);
				
			}	
		}

		this.chunksReady = true;
	}

	public override void _Process(double delta)
	{
		if (!chunksReady)
		{
			return;
		}
		Chunk currentPlayerChunk = getChunkByPos(this.player.GlobalPosition);
		if (currentPlayerChunk == getChunkByPos(this.WorldPos))
		{
			return; // player didn't change chunks so nothing to do here
		}

		GD.Print("PLayer changed chunk");
		
		updateWorldPos(currentPlayerChunk.chunkPos);

		//Add chunk transport and new gen queue
	}
	private void updateWorldPos(Godot.Vector3 pos)
	{
		this.WorldPos = pos;
		this.WorldTopLeftPos = this.WorldPos - new Godot.Vector3(GameGlobals.WorldWidth / 2, 0, GameGlobals.WorldWidth / 2);

	}

	public Chunk getChunkByPos(Godot.Vector3 pos)
	{
		Godot.Vector3 localPos = this.ConvertToLocalWorldPos(pos);
		int row = (int)localPos.Z / (int)GameGlobals.ChunkWidth;
		int col = (int)localPos.X / (int)GameGlobals.ChunkWidth;
		GD.Print(localPos);
		GD.Print(row, " ", col);

		return this.chunks.ElementAt(row).ElementAt(col);
	}
	public Godot.Vector3 ConvertToLocalWorldPos(Godot.Vector3 pos)
	{
		return pos - this.WorldTopLeftPos;
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

		// AddChild(chunk.mesh);
		CallDeferred("add_child", chunk.mesh);
		GD.Print("Chunk mesh added");

		
		
		var reff = getChunkByPos(chunk.chunkTopLeft);
		reff = chunk;
		

		
	}
}

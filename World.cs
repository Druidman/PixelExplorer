using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;



public partial class World : Node3D
{

	WorldNoise noise = new WorldNoise();
	List<Chunk> chunks = new List<Chunk>();
	List<Chunk> stagedChunks = new List<Chunk>();
	float Xpos = 0f;
	bool exitApp = false;
	Thread chunkManagerThread;
	ImageTexture texture = new ImageTexture();
	public override void _Ready()
	{
		
		Image img = new Image();
		img.Load("res://images/customTexture.png");
		
		texture.SetImage(img);

		
		this.chunkManagerThread = new Thread(chunkManager);
		this.chunkManagerThread.Start();
		Chunk chunk = new Chunk(new Godot.Vector3(0f,0f,0f), noise, texture);
		chunk.InitMesh();
		CallDeferred("add_child", chunk.mesh);
		chunks.Add(chunk);

		

	}
	private void chunkManager()
	{
		while (!exitApp){
			
		
			Thread.Sleep(3000);
		
			Thread t = this.startChunkGenThread(new Godot.Vector3(Xpos,0f,0f));
			Xpos += 64;
		}
			
		
	}
	public void smth()
	{
		
	}
	public override void _Process(double delta)
	{

		
	}
	public override void _ExitTree()
	{
		this.exitApp = true;
		this.chunkManagerThread.Join();
	}
	private Thread startChunkGenThread(Godot.Vector3 pos)
	{
		Chunk chunk = new Chunk(pos, this.noise, texture);
		
		Thread chunkThread = new Thread(()=>GenChunk(chunk));
		chunkThread.Start();
		return chunkThread;
	}
	private void GenChunk(Chunk chunk)
	{
		
		chunk.InitMesh();
		CallDeferred("add_child", chunk.mesh);
		chunks.Add(chunk);
	}
}

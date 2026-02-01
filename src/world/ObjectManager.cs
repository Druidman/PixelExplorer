using System.Collections.Generic;
using Godot;

public abstract partial class ObjectManager<Obj> : Node3D where Obj : Node3D
{
	protected Dictionary<Godot.Vector3I, Obj> objects = new Dictionary<Godot.Vector3I, Obj>();
    protected Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Obj>> objectsByChunk = new Dictionary<Godot.Vector3I, Dictionary<Godot.Vector3I, Obj>>();

    abstract public int ObjectsLimit {get; protected set;}


    [Export]
    PackedScene objectScene;
	[Export]
	protected World world;

	public Dictionary<Godot.Vector3I, Obj> GetObjectsAtChunkPos(Godot.Vector3I chunkGlobalPosition)
	{
		return this.objectsByChunk.GetValueOrDefault(chunkGlobalPosition);
	}
    public Obj GetObjectAtPos(Godot.Vector3I objectGlobalPos)
	{
		return this.objects.GetValueOrDefault(objectGlobalPos);
	}

	public Godot.Vector3I GetLocalPosition(Godot.Vector3I globalPos)
	{
		return (Godot.Vector3I)(globalPos - this.GlobalPosition);
	}
	public Godot.Vector3I GetGlobalPosition(Godot.Vector3I localPos)
	{
		return (Godot.Vector3I)(localPos + this.GlobalPosition);
	}
	

    abstract protected void InitializeObject(Obj obj);
	protected bool CreateObject(Godot.Vector3I globalObjectPos)
	{

		if (!this.world.CheckIfValidGlobalPosition(globalObjectPos))
		{
			return false;
		}
        
		Obj obj = this.objectScene.Instantiate<Obj>();
        this.InitializeObject(obj);
		obj.Position = this.GetLocalPosition(globalObjectPos);
		obj.Visible = false;

		Godot.Vector3I chunkPos = this.world.GetChunkPositionFromGlobalPos(globalObjectPos);

		if (!this.objectsByChunk.ContainsKey(chunkPos))
		{
			this.objectsByChunk[chunkPos] = new Dictionary<Godot.Vector3I, Obj>();
		}

		this.objectsByChunk[chunkPos][globalObjectPos] = obj;
        this.objects[globalObjectPos] = obj;

		AddChild(obj);	

		return true;
		
	}

	protected bool RemoveObject(Obj obj)
	{

		Godot.Vector3I globalPos = GetGlobalPosition((Godot.Vector3I)obj.Position);

		if (!this.world.CheckIfValidGlobalPosition(globalPos))
		{
			return false;
		}

		this.GetObjectAtPos(globalPos)?.QueueFree(); // free node

		this.objectsByChunk.GetValueOrDefault(this.world.GetChunkPositionFromGlobalPos(globalPos)).Remove(globalPos); // actual remove
		this.objects.Remove(globalPos); // actual remove

		return true;
	}
	abstract public void GenerateObjects();

    protected void GenerateObjectsRandomlyOnWorldBlocksSurface()
    {
		GD.Print(this.objects.Count);
		if (this.objects.Count >= this.ObjectsLimit)
		{
			return;
		}

		int amountToGen = this.ObjectsLimit - this.objects.Count;

		for (int i=0; i < amountToGen; i++)
		{
			Godot.Vector3I globalPos;
			do
			{
				globalPos = this.world.GetRandomBlockPosInWorld();
				globalPos.Y += 1;
				
			} while ( this.objects.ContainsKey(globalPos) );
			
		
			if (!CreateObject(globalPos)) throw new System.Exception("Something is wrong in creating object in object manager!");
			
		}

		GD.Print(this.objects.Count);
    }

	public bool ShowChunkObjects(Godot.Vector3I chunkGlobalPosition)
	{	

		Dictionary<Godot.Vector3I, Obj> objectsToShow = this.objectsByChunk.GetValueOrDefault(chunkGlobalPosition);
		if (objectsToShow == null) return false;
		
		foreach (Godot.Vector3I orePos in objectsToShow.Keys)
		{
			objectsToShow[orePos].Visible = true;
		}
		return true;
	}

	public bool HideChunkObjects(Godot.Vector3I chunkGlobalPosition)
	{	

		Dictionary<Godot.Vector3I, Obj> objectsToHide = this.objectsByChunk.GetValueOrDefault(chunkGlobalPosition);
		if (objectsToHide == null) return false;
		
		foreach (Godot.Vector3I orePos in objectsToHide.Keys)
		{
			objectsToHide[orePos].Visible = false;
		}
		return true;
	}
}

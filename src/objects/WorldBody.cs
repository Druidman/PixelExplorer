using Godot;
using System.Collections.Generic;

public abstract partial class WorldBody : StaticBody3D, IWorldObject
{


	public abstract List<Godot.Vector3> BaseTiles {get;}
	public Godot.Vector3 GlobalPos {get; set;}
	public virtual Godot.Vector3 PositionOffset
	{
		get
		{
			return new Godot.Vector3(0,0,0);
		}
	}
	
	protected World world;

	public List<Godot.Vector3> GetTiles()
	{
		return ((IWorldObject)this).Tiles;
	}
	public override void _EnterTree()
	{
		this.GlobalPosition = this.GlobalPos;
		OnEnterSceneTree();
		List<Godot.Vector3> tilesToOccupy = this.GetTiles();
		foreach (Godot.Vector3 globalPos in tilesToOccupy)
		{
			
			Chunk chunk = this.world.GetChunkAtPos(globalPos);
			if (chunk == null)
			{
				continue;
			}

			chunk.UpdateTile(
				chunk.ConvertToLocalPosition(this.world.GetTilePosition(globalPos)), 
				new WorldTile(WorldTileState.Occupied, this.world.GetTilePosition(globalPos))
			);
			GD.Print("TIle occupied!");

			
		}
	}
	public override void _ExitTree()
	{
		this.GlobalPosition = this.GlobalPos;
		OnExitSceneTree();
		List<Godot.Vector3> tilesOccupied = this.GetTiles();
		foreach (Godot.Vector3 globalPos in tilesOccupied)
		{
			
			Chunk chunk = this.world.GetChunkAtPos(globalPos);
			if (chunk == null)
			{
				continue;
			}

			chunk.UpdateTile(
				chunk.ConvertToLocalPosition(this.world.GetTilePosition(globalPos)),
				new WorldTile(WorldTileState.Free, this.world.GetTilePosition(globalPos))
			);

			
		}
	}

	protected virtual void OnEnterSceneTree(){}
	protected virtual void OnExitSceneTree(){}
	protected void Initialize(World world, Godot.Vector3 pos)
	{
		((IWorldObject)this).Initialize(pos);
		this.world = world;
		
	}

   
}

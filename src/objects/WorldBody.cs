using Godot;
using System.Collections.Generic;

public abstract partial class WorldBody<T> : StaticBody3D, IWorldObject<T> where T : IWorldObjectDimensions<T> 
{

	protected virtual WorldTileType tileType => WorldTileType.WorldBodyTile;		
	
	protected World world;

	public List<Godot.Vector3I> GetTiles()
	{
		return ((IWorldObject<T>)this).Tiles;
	}
	public override void _EnterTree()
	{
		OnEnterSceneTree();
		List<Godot.Vector3I> tilesToOccupy = this.GetTiles();
		foreach (Godot.Vector3I globalPos in tilesToOccupy)
		{
			
			Chunk chunk = this.world.GetChunkAtPos(globalPos);
			if (chunk == null)
			{
				continue;
			}

			chunk.UpdateTile(
				chunk.ConvertToLocalPosition(this.world.GetTilePosition(globalPos)), 
				new WorldTile(WorldTileState.Occupied, this.world.GetTilePosition(globalPos), tileType)
			);
			

			
		}
	}
	public override void _ExitTree()
	{
		OnExitSceneTree();
		List<Godot.Vector3I> tilesOccupied = this.GetTiles();
		foreach (Godot.Vector3I globalPos in tilesOccupied)
		{
			
			Chunk chunk = this.world.GetChunkAtPos(globalPos);
			if (chunk == null)
			{
				continue;
			}

			chunk.UpdateTile(
				chunk.ConvertToLocalPosition(this.world.GetTilePosition(globalPos)),
				new WorldTile(WorldTileState.Free, this.world.GetTilePosition(globalPos), tileType)
			);

			
		}
		QueueFree();
		
	}

	protected virtual void OnEnterSceneTree(){}
	protected virtual void OnExitSceneTree(){}
	protected void Initialize(World world)
	{
		this.world = world;
	}

   
}

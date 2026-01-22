using Godot;
using System.Collections.Generic;

public abstract partial class Building : WorldBody
{
    public abstract override List<Godot.Vector3> BaseTiles {get;}
	public abstract override Godot.Vector3 PositionOffset {get;}


    protected override WorldTileType tileType => WorldTileType.BuildingTile;

    protected float healthPoints = 20;
    protected Player player;


    public void Initialize(Player player, Godot.Vector3 pos, World world)
    {
        this.Initialize(world,pos);
        this.player = player;
    }
}
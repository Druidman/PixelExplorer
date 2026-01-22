using Godot;
using System.Collections.Generic;

public abstract partial class Building : WorldBody
{
    public abstract override List<Godot.Vector3> BaseTiles {get;}
	public abstract override Godot.Vector3 PositionOffset {get;}


    protected override WorldTileType tileType => WorldTileType.BuildingTile;

    private float healthPoints = 20;
    protected Player player;

    public void TakeHealth(float delta)
    {
        healthPoints -= delta;
        if (healthPoints <= 0)
        {
            this.OnDestroy();
            GetParent()?.RemoveChild(this);
        }
    }

    protected abstract void OnDestroy();


    public void Initialize(Player player, Godot.Vector3 pos, World world)
    {
        this.Initialize(world,pos);
        this.player = player;
    }
}
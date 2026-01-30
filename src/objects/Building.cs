using Godot;
using System.Collections.Generic;

public interface IBuilding
{
    Vector3 GlobalPosition {get; set;}
    public abstract float healthPoints {
        get;
        set;
    }

    void TakeHealth(float delta);

}
    

public abstract partial class Building<T> : WorldBody<T>, IBuilding where T : IWorldObjectDimensions<T>
{
    protected override WorldTileType tileType => WorldTileType.BuildingTile;
    public float healthPoints {
        get;
        set;
    } = 20;
    
    protected Player player;

    public void TakeHealth(float delta)
    {
        healthPoints -= delta;
        if (healthPoints <= 0)
        {
            this.Destroy();
        }
    }
    
    protected void Initialize(Player player, World world)
    {
        this.Initialize(world);
        this.player = player;
    }

    protected virtual void OnDestroy(){}
    protected void Destroy()
    {
        this.OnDestroy();
        GetParent()?.RemoveChild(this);
    }

    
}
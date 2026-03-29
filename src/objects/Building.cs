using Godot;
using System.Collections.Generic;

#nullable enable

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
    
    protected Player? player = null;

    public void TakeHealth(float delta)
    {
        healthPoints -= delta;
        if (healthPoints <= 0)
        {
            this.Destroy();
        }
    }
    
    protected void Initialize(Player? player, World? world)
    {
        if (world == null) throw new System.Exception("null world parameter not accepted! [Building(Initialize)]");

        this.Initialize(world);
        this.player = player;
    }

    protected virtual void OnDestroy(){}
    protected void Destroy()
    {
        this.OnDestroy();
        GetParent()?.RemoveChild(this);
    }

    protected virtual void OnBuildingEnterTree(){

    }
    protected virtual void OnBuildingExitTree(){
        
    }

    protected override void OnEnterSceneTree(){
        GD.Print("PLAY place");
        if (GameGlobals.buildingPlacedSound!=null) GameGlobals.buildingPlacedSound.Play();
        OnBuildingEnterTree();
    }
    protected override void OnExitSceneTree(){
        GD.Print("PLAY destroy");
        if (GameGlobals.buildingDestroyedSound!=null) GameGlobals.buildingDestroyedSound.Play();
        OnBuildingExitTree();
    }

    
}
using Godot;
using System.Collections.Generic;

// allegedly nbot working

public interface IWorldObjectDimensions<T> where T : IWorldObjectDimensions<T>
{
    public abstract static int TilesX {get;}
    public abstract static int TilesY {get;}
    public abstract static int TilesZ {get;}

    public static virtual List<Godot.Vector3I> BaseTiles {get;} = GenBaseTiles();
    
    public static List<Godot.Vector3I> GenBaseTiles()
	{
        if (T.TilesX < 1 || T.TilesY < 1 || T.TilesZ < 0)
        {
            return null;
        }

		List<Godot.Vector3I> tilePositions = new List<Vector3I>(T.TilesX * T.TilesY * T.TilesZ);	

        Godot.Vector3I origin = new Godot.Vector3I(0,0,0);

		Godot.Vector3I shapeTopLeft = origin - new Godot.Vector3I(GameGlobals.TileWidth * (T.TilesX - 1), 0, GameGlobals.TileWidth * (T.TilesZ - 1));
		Godot.Vector3I shapeBottomRight = origin + new Godot.Vector3I(GameGlobals.TileWidth * (T.TilesX - 1), 0, GameGlobals.TileWidth * (T.TilesZ - 1));

		for (int x = shapeTopLeft.X; x <= shapeBottomRight.X; x += GameGlobals.TileWidth)
		{
			for (int y = shapeTopLeft.Y; y <= shapeBottomRight.Y; y += GameGlobals.TileWidth)
			{
				for (int z = shapeTopLeft.Z; z <= shapeBottomRight.Z; z += GameGlobals.TileWidth)
				{
					tilePositions.Add(new Godot.Vector3I(x,y,z));
				}
			}

		}
		return tilePositions;
	}
    
}

public interface IWorldObject<T> where T : IWorldObjectDimensions<T>
{
    public Godot.Vector3 GlobalPosition {get; set;}
    public Godot.Vector3 GlobalPos {get; set;}

    public List<Godot.Vector3I> Tiles {
        get
        {
            List<Godot.Vector3I> tiles = new List<Godot.Vector3I>(T.BaseTiles);
            for (int i =0; i< tiles.Count; i++)
            {
                tiles[i] += (Godot.Vector3I)this.GlobalPosition;
            }
            return tiles;
        }
    }

    public void Initialize(Godot.Vector3 globalPosition)
	{
		this.GlobalPos = globalPosition;
	}
    
}

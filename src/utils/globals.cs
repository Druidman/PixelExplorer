
using System.Collections.Generic;
using Godot;

class GameGlobals
{
    public static int ChunkWidth = 8;
    public static Godot.Vector3 StartWorldMiddle = new Godot.Vector3(0,0,0);
    public static int chunkRadius = 15;

    public static Godot.Vector3 PlayerStartPos = new Godot.Vector3(0,200,0);
    public static int TileWidth = 1;
    public static Godot.Vector2 TextureAtlasSize = new Godot.Vector2(48,64);
    public static int YAxisCells = 4;
    public static int XAxisCells = 3;

    public static float XAxisMove = (float)TextureAtlasSize.X / (float)XAxisCells / (float)TextureAtlasSize.X;
    public static float YAxisMove = (float)TextureAtlasSize.Y / (float)YAxisCells / (float)TextureAtlasSize.Y;

    public static List<Godot.Vector2> baseBlockUvSector = [
        new Godot.Vector2(0, 0),
        new Godot.Vector2(XAxisMove, 0),
        new Godot.Vector2(XAxisMove,YAxisMove),
        new Godot.Vector2(0,YAxisMove)
    ];
}


using System.Collections.Generic;

class GameGlobals
{
    public static Godot.Vector2 TextureAtlasSize = new Godot.Vector2(64,160);
    public static int YAxisCells = 10;
    public static int XAxisCells = 4;

    public static float XAxisMove = (float)TextureAtlasSize.X / (float)XAxisCells / (float)TextureAtlasSize.X;
    public static float YAxisMove = (float)TextureAtlasSize.Y / (float)YAxisCells / (float)TextureAtlasSize.Y;

    public static List<Godot.Vector2> baseBlockUvSector = [
        new Godot.Vector2(0, 0),
        new Godot.Vector2(XAxisMove, 0),
        new Godot.Vector2(XAxisMove,YAxisMove),
        new Godot.Vector2(0,YAxisMove)
    ];
}


using System.Collections.Generic;

class GameGlobals
{
    public static Godot.Vector2 TextureAtlasSize = new Godot.Vector2(64,160);

    public static List<Godot.Vector2> baseBlockUvSector = [
        new Godot.Vector2(0.0f,0.0f),
        new Godot.Vector2(0.0f, 16.0f / TextureAtlasSize.Y),
        new Godot.Vector2(16.0f / TextureAtlasSize.X, 0.0f),
        new Godot.Vector2(16.0f / TextureAtlasSize.X, 16.0f / TextureAtlasSize.Y)
    ];
}

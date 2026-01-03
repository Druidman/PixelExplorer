
using System.Collections.Generic;
using Godot;

class GameGlobals
{

    public static int WorldCoinsLimit = 100;
    public static bool DebugMode = false;
    public static int ChunkWidth = 8;
    public static Godot.Vector3 StartWorldMiddle = new Godot.Vector3(0,0,0);
    public static int chunkRadius = 20;


    public static int WorldWidth = 500;
    public static Godot.Vector3 MaxWorldTopLeft = new Godot.Vector3(-WorldWidth / 2,0,-WorldWidth / 2);
    public static Godot.Vector3 MaxWorldBottomRight = new Godot.Vector3(WorldWidth / 2,0,WorldWidth / 2);

    public static Godot.Vector3 PlayerStartPos = new Godot.Vector3(0,200,0);
    public static float GravitySpeed = 20.0f;
    public static float PlayerJumpForce = 10f;

    
    public static float PlayerSpeed = 10.0f;
    public static float PlayerDecelerationSpeed = GameGlobals.PlayerSpeed * 0.1f;

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

    public static ImageTexture texture = new ImageTexture();
    public static World world = null;
    public static Game game = null;




    public static void Initialize(Game gameInstance)
    {
        Image img = new Image();
		img.Load("res://images/TextureWithoutEdges.png");
		
		texture.SetImage(img);
        game = gameInstance;
        world = game.world;
 
    }
}


using System.Collections.Generic;
using Godot;

class GameGlobals
{

    public static int ChunkCoinLimit = 1;
    public static bool DebugMode = false;
    public static int ChunkWidth = 17;
    public static Godot.Vector3I StartWorldMiddle = new Godot.Vector3I(0,0,0);
    public static int chunkRadius = 10;


    public static int WorldWidth = 500;
    public static Godot.Vector3I MaxWorldTopLeft = new Godot.Vector3I(-WorldWidth / 2,0,-WorldWidth / 2);
    public static Godot.Vector3I MaxWorldBottomRight = new Godot.Vector3I(WorldWidth / 2,0,WorldWidth / 2);

    public static Godot.Vector3 PlayerStartPos = new Godot.Vector3(0,200,0);
    public static float GravitySpeed = 30.0f;
    public static float PlayerJumpForce = 10f;

    
    public static float PlayerSpeed = 15.0f;
    public static float PlayerDecelerationSpeed = GameGlobals.PlayerSpeed * 0.1f;
    public static int GoldMineCost = 10;

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

    public static PackedScene coinScene = null;
    public static PackedScene GoldOreScene = null;
    public static PackedScene GoldMineScene = null;
    public static PackedScene SoldierHomeScene = null;
    public static PackedScene SoldierHomePlacerScene = null;
    public static PackedScene chunkScene = null;
    public static PackedScene soldierScene = null;
    public static Chunk placeholderChunk = null;

    public static int SoldierCost = 5;


    public static void Initialize()
    {
        Image img = new Image();
		img.Load("res://images/TextureWithoutEdges.png");
		
		texture.SetImage(img);

        coinScene = GD.Load<PackedScene>("res://src/objects/Coin/Coin.tscn");
        chunkScene = GD.Load<PackedScene>("res://src/world/chunk/chunk.tscn");
        soldierScene = GD.Load<PackedScene>("res://src/entities/Soldier/soldier.tscn");
        GoldOreScene = GD.Load<PackedScene>("res://src/objects/Ores/Gold/GoldOre.tscn");
        GoldMineScene = GD.Load<PackedScene>("res://src/objects/GoldMine/gold_mine.tscn");
        SoldierHomeScene = GD.Load<PackedScene>("res://src/objects/SoldierHome/soldier_home.tscn");
        SoldierHomePlacerScene = GD.Load<PackedScene>("res://src/objects/SoldierHome/soldier_home_placer.tscn");
        placeholderChunk = chunkScene.Instantiate<Chunk>();
 
    }
}

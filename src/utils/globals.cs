
using System.Collections.Generic;
using Godot;

class GameGlobals
{

    public static int CoinLimit = 1000;
    public static bool DebugMode = false;
    public static int ChunkWidth = 17;
    public static Godot.Vector3I StartWorldMiddle = new Godot.Vector3I(0,0,0);
    public static int chunkRadius = 10;

    public static int PlayerStartCoins = 10;
    

    public static Godot.Vector3 PlayerStartPos = new Godot.Vector3(0,200,0);
    public static float GravitySpeed = 30.0f;
    public static float PlayerJumpForce = 10f;

    
    public static float PlayerSpeed = 15.0f;
    public static float PlayerDecelerationSpeed = GameGlobals.PlayerSpeed * 0.1f;
    public static int GoldMineCost = 10;
    public static int housePrice = 10;
    public static int archerTurretPrice = 10;
    public static int magicTurretPrice = 10;

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

    public static ImageTexture texture = makeTexture();
    public static PackedScene coinScene = GD.Load<PackedScene>("res://src/objects/Coin/Coin.tscn");
    public static PackedScene GoldOreScene = GD.Load<PackedScene>("res://src/objects/Ores/Gold/GoldOre.tscn");
    public static PackedScene GoldMineScene = GD.Load<PackedScene>("res://src/objects/GoldMine/gold_mine.tscn");
    public static PackedScene SoldierHomeScene = GD.Load<PackedScene>("res://src/objects/SoldierHome/soldier_home.tscn");
    public static PackedScene chunkScene = GD.Load<PackedScene>("res://src/world/chunk/chunk.tscn");
    public static PackedScene soldierScene = GD.Load<PackedScene>("res://src/entities/Soldier/soldier.tscn");
    public static PackedScene ArcherTurretScene = GD.Load<PackedScene>("res://src/objects/Turrets/ArcherTurret/ArcherTurret.tscn");
    public static PackedScene MagicTurretScene = GD.Load<PackedScene>("res://src/objects/Turrets/MagicTurret/MagicTurret.tscn");
    public static Chunk placeholderChunk = chunkScene.Instantiate<Chunk>();

    public static int SoldierCost = 5;

    public static SfxPlayer coinCollectedSound = null;
    public static SfxPlayer buildingPlacedSound = null;
    public static SfxPlayer buildingDestroyedSound = null;
    public static SfxPlayer punchSound = null;
    public static SfxPlayer spawnSound = null;
    public static SfxPlayer dieSound = null;

    private static ImageTexture makeTexture()
    {
        ImageTexture texture = new ImageTexture();
        Image img = new Image();
		img.Load("res://images/TextureWithoutEdges.png");
		
		texture.SetImage(img);
        return texture;
    }

}

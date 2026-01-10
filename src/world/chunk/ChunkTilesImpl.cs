
using System.Collections.Generic;

public partial class Chunk
{
    List< List< List< WorldTile > > > tiles = new List<List<List<WorldTile>>>();
    private bool UpdateTile(int platform, int row, int col, WorldTile tile)
	{	
		if (!CheckIfTileFits(platform, row, col))
		{
			if (!ResizeTilesToPlatform(platform)) return false;
			if (!ResizeTilesToRow(platform,row)) return false;
			if (!ResizeTilesToCol(platform,row,col)) return false;
		}


		this.tiles[platform][row][col] = tile;
		return true;
	}
    private bool ResizeTilesToPlatform(int platform)
	{
		if (!CheckIfValidTileIndicies(platform, 0, 0)) return false;
		

		
		
		for (int i = this.tiles.Count; i<platform + 1; i++)
		{
			this.tiles.Add(new List<List<WorldTile>>());
		}
		return true;
	}
	private bool ResizeTilesToRow(int platform, int row)
	{
		
		if (!CheckIfValidTileIndicies(platform, row, 0)) return false;

		if (!CheckIfTilePlatformFits(platform)) return false;

		
		
		for (int i = this.tiles[platform].Count; i<row + 1; i++)
		{
			
			this.tiles[platform].Add(new List<WorldTile>());
		}
		return true;
	}
	private bool ResizeTilesToCol(int platform, int row, int col)
	{

		if (!CheckIfValidTileIndicies(platform, row, col)) return false;

		if (!CheckIfTileRowFits(platform, row)) return false;

		
		for (int i = this.tiles[platform][row].Count; i<col + 1; i++)
		{
			this.tiles[platform][row].Add(new WorldTile(getGlobalPositionOfTile(platform, row, i),BlockType.NONE));
		}
		return true;
	}


	public WorldTile CheckIfTileExists(int platform, int row, int col)
	{
		if (!CheckIfTileFits(platform ,row, col))
		{
			return null;
		}
		WorldTile tile = this.tiles[platform][row][col];
		if (tile.blockType == BlockType.NONE)
		{
			return null;
		}
		return tile;
	}

	public bool CheckIfTileFits(int platform, int row, int col)
	{
		if (!CheckIfTileColFits(platform, row, col)) return false;

		return true;
	}

	private bool CheckIfTilePlatformFits(int platform)
	{
		if (platform < 0 || platform >= this.tiles.Count) return false;
		return true;
	}
	private bool CheckIfTileRowFits(int platform, int row)
	{
		if (!CheckIfTilePlatformFits(platform)) return false;
		if (row < 0 || row >= this.tiles[platform].Count) return false;
		return true;
	}
	private bool CheckIfTileColFits(int platform, int row, int col)
	{
		if (!CheckIfTileRowFits(platform, row)) return false;
		if (col < 0 || col >= this.tiles[platform][row].Count) return false;
		return true;
	}
    private bool CheckIfValidTileIndicies(int platform, int row, int col)
	{

		if (platform < 0 || platform > Height / GameGlobals.TileWidth) return false;
		if (row < 0 || row > Width / GameGlobals.TileWidth) return false;
		if (col < 0 || col > Width / GameGlobals.TileWidth) return false;

		return true;
	}
	

}
using NeaProject.Classes;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace NeaProject.Engine;

public class Renderer
{
    const int tileWidth = 32;
    const int tileHeight = 32;

    private uint pink = MakePixel(0xff, 0xbb, 0xbb, 0xff);
    private uint black = MakePixel(0x00, 0x00, 0x00, 0xFF);
    private uint white = MakePixel(0xFF, 0xFF, 0xFF, 0xFF);
    private readonly Map _map;
    private readonly Player _player;
    private readonly Dictionary<char, Sprite> _sprites;
    private readonly int _width;
    private readonly int _height;
    private readonly uint[,] _buffer;
    private readonly int _xTileCount;
    private readonly int _yTileCount;
    private double t;
    
    public Renderer(Map map, Player player, Dictionary<char, Sprite> sprites, int width, int height)
    {
        _map = map;
        _player = player;
        _sprites = sprites;
        _width = width;
        _height = height;
        _buffer = new uint[height, width];
        _xTileCount = width/tileWidth;
        _yTileCount = height/tileHeight;
    }
    
    public uint[,] UpdateFrameBuffer()
    {
        DrawMap();
        DrawPlayer();

        return _buffer;
    }

    private void DrawPlayer()
    {
        DrawPlainTile(_player.YPos, _player.XPos, pink);
    }

    private void DrawMap()
    {
        for (int tileRow = 0; tileRow < _yTileCount; tileRow++)
        {
            for (int tileCol = 0; tileCol < _xTileCount; tileCol++)
            {
                // determine colour of tile
                uint tileColour = GetBaseMap(tileRow, tileCol);

                DrawPlainTile(tileRow, tileCol, tileColour);
            }
        }
    }

    private void DrawPlainTile(int tileRow, int tileCol, uint tileColour)
    {
        // calculating offset
        int yOffset = tileRow * tileHeight;
        int xOffset = tileCol * tileWidth;

        // draw tile
        for (int pixelRow = 0; pixelRow < tileHeight; pixelRow++)
        {
            for (int pixelCol = 0; pixelCol < tileWidth; pixelCol++)
            {
                _buffer[pixelRow + yOffset, pixelCol + xOffset] = tileColour;
            }
        }
    }

    private uint GetCheckerboard(int tileRow, int tileCol)
    {
        uint tileColour;
        if (tileRow % 2 == tileCol % 2)
        {
            tileColour = black;
        }
        else
        {
            tileColour = white;
        }

        return tileColour;
    }

    private uint GetBaseMap(int tileRow, int tileCol)
    {
        char tileChar = _map.GetTileChar(tileRow, tileCol);
        Sprite sprite = _sprites[tileChar];
        Color charColour = sprite.GetColourAt(0, 0);
        uint tileColour = MakePixel(charColour.R, charColour.G, charColour.B, charColour.A);
        return tileColour;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
        (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
}
using NeaProject.Classes;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace NeaProject.Engine;

public class Renderer
{
    const int tileWidth = 32;
    const int tileHeight = 32;

    private uint pink = MakePixel(0xff, 0xbb, 0xbb, 0xff);
    private uint black = MakePixel(0x00, 0x00, 0x00, 0xff);
    private uint white = MakePixel(0xff, 0xff, 0xff, 0xff);
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
        _xTileCount = width / tileWidth;
        _yTileCount = height / tileHeight;
    }

    public uint[,] UpdateFrameBuffer()
    {
        DrawMap();
        DrawPlayer();

        return _buffer;
    }

    private void DrawPlayer()
    {
        DrawSprite(_player.YPos, _player.XPos, _sprites['p']);
    }

    private void DrawMap()
    {
        for (int tileRow = 0; tileRow < _yTileCount; tileRow++)
        {
            for (int tileCol = 0; tileCol < _xTileCount; tileCol++)
            {
                // determine colour of tile
                char mapChar = _map.GetTileChar(tileRow, tileCol);
                Sprite sprite = _sprites[mapChar];
                DrawSprite(tileRow, tileCol, sprite);
            }
        }
    }

    private void DrawSprite(int tileRow, int tileCol, Sprite sprite)
    {
        // calculating offset
        int yOffset = tileRow * tileHeight;
        int xOffset = tileCol * tileWidth;

        // draw tile
        for (int pixelRow = 0; pixelRow < tileHeight; pixelRow++)
        {
            for (int pixelCol = 0; pixelCol < tileWidth; pixelCol++)
            {
                uint pixelColour = sprite.GetColourAt(pixelCol, pixelRow);
                if (pixelColour != 0x00ffffff) // if not transparent, then draw pixel (16777215)
                {
                    _buffer[pixelRow + yOffset, pixelCol + xOffset] = pixelColour;
                }
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
        uint tileColour = sprite.GetColourAt(0, 0);
        return tileColour;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
        (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
}
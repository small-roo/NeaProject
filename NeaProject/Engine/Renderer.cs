using NeaProject.Classes;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace NeaProject.Engine;

public class Renderer
{
    const int tileWidth = Sprite.tileSize;
    const int tileHeight = Sprite.tileSize;

    private readonly Map _map;
    private readonly Player _player;
    private readonly Dictionary<char, Sprite?> _sprites;
    private readonly uint[,] _buffer;
    private readonly int _xTileCount;
    private readonly int _yTileCount;

    public Renderer(Map map, Player player, Dictionary<char, Sprite?> sprites, int width, int height)
    {
        _map = map;
        _player = player;
        _sprites = sprites;
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
        DrawSprite(_player.YPos, _player.XPos, _sprites['p'], _player.FrameIndex);
    }

    private void DrawMap()
    {
        for (int tileRow = 0; tileRow < _yTileCount; tileRow++)
        {
            for (int tileCol = 0; tileCol < _xTileCount; tileCol++)
            {
                // determine sprite of tile
                char mapChar = _map.GetTileChar(tileRow, tileCol);
                Sprite? sprite = _sprites[mapChar];
                DrawSprite(tileRow, tileCol, sprite, 0);
                mapChar = _map.GetOverlayTileChar(tileRow, tileCol);
                sprite = _sprites[mapChar];
                DrawSprite(tileRow, tileCol, sprite, 0);
            }
        }
    }

    private void DrawSprite(int tileRow, int tileCol, Sprite? sprite, int frameIndex)
    {
        if (sprite == null)
        {
            return;
        }
        // calculating offset
        int yOffset = tileRow * tileHeight;
        int xOffset = tileCol * tileWidth;

        // draw tile
        for (int pixelRow = 0; pixelRow < tileHeight; pixelRow++)
        {
            for (int pixelCol = 0; pixelCol < tileWidth; pixelCol++)
            {
                uint pixelColour = sprite.GetColourAt(pixelCol, pixelRow, frameIndex);
                if (pixelColour >> 24 != 0x00) // if not transparent, then draw pixel (checks the alpha channel)
                {
                    _buffer[pixelRow + yOffset, pixelCol + xOffset] = pixelColour;
                }
            }
        }
    }
}
using NeaProject.Classes;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace NeaProject.Engine;

public class Renderer
{
    const int tileWidth = Sprite.tileSize;
    const int tileHeight = Sprite.tileSize;

    public int DrawingStartTileX = 1;
    public int DrawingStartTileY = 1;
    public int ViewportHeight = 320;
    public int ViewportWidth = 640;
    public int ViewportTileX = 20;
    public int ViewportTileY = 10;
    public int ViewportEdgeBuffer = 3;

    private readonly Map _map;
    private readonly Player _player;
    private readonly Dictionary<char, Sprite?> _sprites;
    private readonly List<Npc> _npcs;
    private readonly Stopwatch _stopwatch;
    private readonly uint[,] _buffer;

    public Renderer(Map map, Player player, List<Npc> npcs, Dictionary<char, Sprite?> sprites)
    {
        _map = map;
        _player = player;
        _npcs = npcs;
        _sprites = sprites;
        _stopwatch = Stopwatch.StartNew();
        _buffer = new uint[ViewportHeight, ViewportWidth];
    }

    public uint[,] UpdateFrameBuffer()
    {
        bool isAnimationFrame = false;
        if (_stopwatch.ElapsedMilliseconds > 100)
        {
            _stopwatch.Restart();
            isAnimationFrame = true;
        }

        DrawMap();
        DrawNpcs(isAnimationFrame);
        DrawPlayer(isAnimationFrame);
        
        return _buffer;
    }

    public void MoveCamera()
    {
        if (_player.XPos - DrawingStartTileX < ViewportEdgeBuffer)
        {
            DrawingStartTileX--;
        }
        else if (_player.XPos - DrawingStartTileX >= ViewportTileX - ViewportEdgeBuffer)
        { 
            DrawingStartTileX++;
        }

        if (_player.YPos - DrawingStartTileY < ViewportEdgeBuffer)
        {
            DrawingStartTileY--;
        }
        else if (_player.YPos - DrawingStartTileY >= ViewportTileY - ViewportEdgeBuffer)
        {
            DrawingStartTileY++;
        }
    }

    private void DrawPlayer(bool isAnimationFrame)
    {
        if (isAnimationFrame)
        {
            _player.Animate();
        }
        DrawSprite(_player.YPos - DrawingStartTileY, _player.XPos - DrawingStartTileX, _sprites['p'], _player.FrameIndex);
    }

    private void DrawNpcs(bool isAnimationFrame)
    { 
        foreach (var npc in _npcs.Where(npc =>
            npc.YPos >= DrawingStartTileY &&
            npc.YPos < DrawingStartTileY + ViewportTileY &&
            npc.XPos >= DrawingStartTileX &&
            npc.XPos < DrawingStartTileX + ViewportTileX))
        {
            if (isAnimationFrame)
            {
                npc.Animate();
            }
            DrawSprite(npc.YPos - DrawingStartTileY, npc.XPos - DrawingStartTileX, _sprites[npc.SpriteRef], npc.FrameIndex);
        }
    }

    private void DrawMap()
    {
        for (int drawingTileY = 0; drawingTileY < ViewportTileY; drawingTileY++)
        {
            for (int drawingTileX = 0; drawingTileX < ViewportTileX; drawingTileX++)
            {
                // determine sprite of tile
                char mapChar = _map.GetTileChar(DrawingStartTileY + drawingTileY, DrawingStartTileX + drawingTileX);
                Sprite? sprite = _sprites[mapChar];
                DrawSprite(drawingTileY, drawingTileX, sprite, 0);
                mapChar = _map.GetOverlayTileChar(DrawingStartTileY + drawingTileY, DrawingStartTileX + drawingTileX);
                sprite = _sprites[mapChar];
                switch (mapChar)
                {
                    case 'B':
                    case '.':
                        {
                            break;
                        }
                    default:
                        { 
                            DrawSprite(drawingTileY, drawingTileX, sprite, 0);
                            break; 
                        }
                }
                
            }
        }
    }

    private void DrawSprite(int drawingTileY, int drawingTileX, Sprite? sprite, int frameIndex)
    {
        if (sprite == null)
        {
            return;
        }
        // calculating offset
        int yOffset = drawingTileY * tileHeight;
        int xOffset = drawingTileX * tileWidth;

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
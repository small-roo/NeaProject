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

    public Renderer(Game game, Dictionary<char, Sprite?> sprites)
    {
        _map = game.Map;
        _player = game.Player;
        _npcs = game.Npcs;
        _sprites = sprites;
        _stopwatch = Stopwatch.StartNew();
        _buffer = new uint[ViewportHeight, ViewportWidth];
    }

    public uint[,] UpdateFrameBuffer(Game game)
    {
        bool isAnimationFrame = false;
        if (_stopwatch.ElapsedMilliseconds > 100)
        {
            _stopwatch.Restart();
            isAnimationFrame = true;
        }

        DrawMap(game.Camera);
        DrawNpcs(isAnimationFrame, game);
        DrawPlayer(isAnimationFrame, game);

        return _buffer;
    }

    public void MoveCamera(Camera camera)
    {
        if (_player.XPos - camera.DrawingStartTileX < ViewportEdgeBuffer)
        {
            camera.DrawingStartTileX--;
        }
        else if (_player.XPos - camera.DrawingStartTileX >= ViewportTileX - ViewportEdgeBuffer)
        {
            camera.DrawingStartTileX++;
        }

        if (_player.YPos - camera.DrawingStartTileY < ViewportEdgeBuffer)
        {
            camera.DrawingStartTileY--;
        }
        else if (_player.YPos - camera.DrawingStartTileY >= ViewportTileY - ViewportEdgeBuffer)
        {
            camera.DrawingStartTileY++;
        }
    }

    private void DrawPlayer(bool isAnimationFrame, Game game)
    {
        if (isAnimationFrame)
        {
            _player.Animate(game);
        }
        DrawSprite(_player.YPos - game.Camera.DrawingStartTileY, _player.XPos - game.Camera.DrawingStartTileX, _sprites['p'], _player.FrameIndex);
    }

    private void DrawNpcs(bool isAnimationFrame, Game game)
    {
        // move any onscreen npcs (which might move them offscreen :])
        foreach (var npc in OnScreenNpcs(game))
        {
            if (isAnimationFrame)
            {
                npc.Animate(game);
            }
        }
        // draw them if they are still onscreen
        foreach (var npc in OnScreenNpcs(game))
        {
            DrawSprite(npc.YPos - game.Camera.DrawingStartTileY, npc.XPos - game.Camera.DrawingStartTileX, _sprites[npc.SpriteRef], npc.FrameIndex);
        }
    }

    private IEnumerable<Npc> OnScreenNpcs(Game game)
    {
        return _npcs.Where(npc =>
                npc.YPos >= game.Camera.DrawingStartTileY &&
                npc.YPos < game.Camera.DrawingStartTileY + ViewportTileY &&
                npc.XPos >= game.Camera.DrawingStartTileX &&
                npc.XPos < game.Camera.DrawingStartTileX + ViewportTileX);
    }

    private void DrawMap(Camera camera)
    {
        for (int drawingTileY = 0; drawingTileY < ViewportTileY; drawingTileY++)
        {
            for (int drawingTileX = 0; drawingTileX < ViewportTileX; drawingTileX++)
            {
                // determine sprite of tile
                char mapChar = _map.GetTileChar(camera.DrawingStartTileX + drawingTileX, camera.DrawingStartTileY + drawingTileY);
                Sprite? sprite = _sprites[mapChar];
                DrawSprite(drawingTileY, drawingTileX, sprite, 0);
                mapChar = _map.GetOverlayTileChar(camera.DrawingStartTileX + drawingTileX, camera.DrawingStartTileY + drawingTileY);
                sprite = _sprites[mapChar];
                switch (mapChar)
                {
                    case 'B':
                    case 'F':
                    case 'S':
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
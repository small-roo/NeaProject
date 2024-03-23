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

    public const int MaxViewportHeight = 360;
    public const int MaxViewportWidth = 720;

    public int ViewportEdgeBuffer = 3;
    public int ViewportHeight = 400;
    public int ViewportWidth = 640;
    private int ViewportTileY = 12;
    private int ViewportTileX = 20;

    private readonly Map _map;
    private readonly Player _player;
    private readonly Dictionary<char, Sprite?> _sprites;
    private readonly List<Npc> _npcs;
    private uint[,] _buffer = new uint[320, 640];

    public Renderer(Game game, Dictionary<char, Sprite?> sprites)
    {
        _map = game.Map;
        _player = game.Player;
        _npcs = game.Npcs;
        _sprites = sprites;
        
    }

    public uint[,] UpdateFrameBuffer(Game game, int viewportWidth, int viewportHeight)
    {
        //if doesn't match, recalculate size, viewport and recentre
        if (viewportHeight != ViewportHeight || viewportWidth != ViewportWidth) 
        {
            _buffer = new uint[viewportHeight, viewportWidth]; //new buffer array as the canvas is a different size
            ViewportHeight = viewportHeight;
            ViewportWidth = viewportWidth;
            ViewportTileY = viewportHeight / 32;
            ViewportTileX = viewportWidth / 32;
            game.ScreenTileHeight = ViewportTileY;
            game.ScreenTileWidth = ViewportTileX;

            //set how far from edge of screen it moves when the player takes a step depending on screen size
            if (ViewportTileX > 15) 
            {
                ViewportEdgeBuffer = 3;
            }
            else if (ViewportTileX > 11)
            {
                ViewportEdgeBuffer = 2;
            }
            else
            {
                ViewportEdgeBuffer = 1;
            }
            _map.VisibleHeight = ViewportTileY;
            _map.VisibleWidth = ViewportTileX;

            //recentre map to avoid trying to draw off the edge of the screen
            game.Camera.DrawingStartTileX = game.Player.XPos - game.Map.VisibleWidth / 2;
            game.Camera.DrawingStartTileY = game.Player.YPos - game.Map.VisibleHeight / 2;
        }

        //using a stopwatch prevents fps from affecting how fast the characters move
        bool isAnimationFrame = false;
        if (game.GameStopwatch.ElapsedMilliseconds > 100)
        {
            game.GameStopwatch.Restart();
            isAnimationFrame = true;
        }

        DrawMap(game.Camera);
        DrawNpcs(isAnimationFrame, game);
        DrawPlayer(isAnimationFrame, game);

        return _buffer;
    }

    private void DrawPlayer(bool isAnimationFrame, Game game)
    {
        if (isAnimationFrame) //would be used for idle animations/set animations for story purposes
        {
            _player.Animate(game);
        }
        char mapChar = _map.GetTileChar(_player.XPos, _player.YPos);
        Sprite? tileSprite = _sprites[mapChar];
        DrawSprite(_player.YPos - game.Camera.DrawingStartTileY, _player.XPos - game.Camera.DrawingStartTileX, _sprites['p'], tileSprite, _player.FrameIndex);
    }

    private void DrawNpcs(bool isAnimationFrame, Game game)
    {
        // move any onscreen npcs (which might move them offscreen)
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
            char mapChar = _map.GetTileChar(npc.XPos, npc.YPos);
            Sprite? tileSprite = _sprites[mapChar];
            DrawSprite(npc.YPos - game.Camera.DrawingStartTileY, npc.XPos - game.Camera.DrawingStartTileX, _sprites[npc.SpriteRef], tileSprite, npc.FrameIndex);
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
                //draw base tile
                DrawSprite(drawingTileY, drawingTileX, sprite, null, 0);
                // determine sprite of overlay tile
                mapChar = _map.GetOverlayTileChar(camera.DrawingStartTileX + drawingTileX, camera.DrawingStartTileY + drawingTileY);
                Sprite? overlaySprite = _sprites[mapChar];
                //draw overlay tile if it isn't a character
                switch (mapChar)
                {
                    case 'B':
                    case 'F':
                    case 'I':
                    case 'S':
                    case 'p':
                    case '.':
                        {
                            break;
                        }
                    default:
                        { 
                            DrawSprite(drawingTileY, drawingTileX, overlaySprite, sprite, 0);
                            break; 
                        }
                }
            }
        }
    }
    
    private void DrawSprite(int drawingTileY, int drawingTileX, Sprite? sprite, Sprite? baseSprite, int frameIndex)
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
                uint pixelAlpha = pixelColour >> 24;

                if (pixelAlpha == 0xff) // if opaque, draw w/o calculating 
                {
                    _buffer[pixelRow + yOffset, pixelCol + xOffset] = pixelColour;
                }
                else if (pixelAlpha != 0x00) // if transparent, don't draw pixel (checks the alpha channel)
                {
                    //seperate into RGB
                    uint red = pixelColour % 256;
                    uint green = (pixelColour >> 8) % 256;
                    uint blue = (pixelColour >> 16) % 256;
                    uint baseRed;
                    uint baseGreen;
                    uint baseBlue;
                    if (baseSprite != null) //seperate colour underneath into RGB
                    {
                        uint basePixelColour = baseSprite.GetColourAt(pixelCol, pixelRow, 0); // assume that the base sprite only has 1 frame.
                        baseRed = basePixelColour % 256;
                        baseGreen = (basePixelColour >> 8) % 256;
                        baseBlue = (basePixelColour >> 16) % 256;
                    }
                    else //if there is no pixel below, the base colour is black
                    {
                        baseRed = 0x00;
                        baseGreen = 0x00;
                        baseBlue = 0x00;
                    }
                    red = OpacityCalc(red, baseRed, pixelAlpha);
                    green = OpacityCalc(green, baseGreen, pixelAlpha);
                    blue = OpacityCalc(blue, baseBlue, pixelAlpha);
                    pixelColour = (uint)((0xff << 24) | (blue << 16) | (green << 8) | red); //recombine
                    _buffer[pixelRow + yOffset, pixelCol + xOffset] = pixelColour;
                }
            }
        }
    }

    //calculate what colour to draw a pixel from the existing colour, the new colour and the alpha channel of the new colour
    public static uint OpacityCalc(uint colour, uint baseColour, uint alpha)
    {
        colour = (colour * alpha + (0xff - alpha) * baseColour) / 0xff;
        return colour;
    }

    public void MoveCamera(Game game)
    {
        if (_player.XPos - game.Camera.DrawingStartTileX < ViewportEdgeBuffer) //moves left
        {
            game.Camera.DrawingStartTileX--;
        }
        else if (_player.XPos - game.Camera.DrawingStartTileX >= game.ScreenTileWidth - ViewportEdgeBuffer) //moves right
        {
            game.Camera.DrawingStartTileX++;
        }
        if (_player.YPos - game.Camera.DrawingStartTileY < ViewportEdgeBuffer) //moves up
        {
            game.Camera.DrawingStartTileY--;
        }
        else if (_player.YPos - game.Camera.DrawingStartTileY >= game.ScreenTileHeight - ViewportEdgeBuffer) //moves down
        {
            game.Camera.DrawingStartTileY++;
        }
    }
}
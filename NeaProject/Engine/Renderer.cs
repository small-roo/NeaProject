using NeaProject.Classes;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace NeaProject.Engine;

public class Renderer
{
    const int tileWidth = 16;
    const int tileHeight = 16;


    private uint black = MakePixel(0x00, 0x00, 0x00, 0xFF);
    private uint white = MakePixel(0xFF, 0xFF, 0xFF, 0xFF);
    private readonly Map _map;
    private readonly int _width;
    private readonly int _height;
    private readonly uint[,] _buffer;
    private readonly int _xTileCount;
    private readonly int _yTileCount;
    private double t;
    
    public Renderer(Map map, int width, int height)
    {
        _map = map;
        _width = width;
        _height = height;
        _buffer = new uint[height, width];
        _xTileCount = width/tileWidth;
        _yTileCount = height/tileHeight;
    }
    
    public uint[,] UpdateFrameBuffer()
    {
        //for (int row = 0; row < _height; row++)
        //    for (int col = 0; col < _width; col++)
        //    {
        //        byte red = (byte)(Math.Sin((double)col/_width*2*Math.PI)*127+128);
        //        byte green = (byte)(Math.Sin(t)*127+128);
        //        byte blue = (byte)(Math.Sin((double)row / _height * 2 * Math.PI+t) * 127 + 128);
        //        _buffer[row, col] = MakePixel(red, green, blue, 0xFF);
        //    }
        //t+=0.1;

        for (int tileRow = 0; tileRow < _yTileCount; tileRow++)
        {
            for (int tileCol = 0; tileCol < _xTileCount; tileCol++)
            {
                // determine colour of tile
                uint tileColour = GetBaseMap(tileRow, tileCol);

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
        }

        return _buffer;
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
        Color charColour = _map.GetCharColour(tileChar);
        uint tileColour = MakePixel(charColour.R, charColour.G, charColour.B, charColour.A);
        return tileColour;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
        (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
}
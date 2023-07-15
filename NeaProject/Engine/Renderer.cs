using System.Runtime.CompilerServices;

namespace NeaProject.Engine;

public class Renderer
{
    private readonly int _width;
    private readonly int _height;
    private readonly uint[,] _buffer;
    private double t;
    
    public Renderer(int width, int height)
    {
        _width = width;
        _height = height;
        _buffer = new uint[height, width];
    }
    
    public uint[,] UpdateFrameBuffer()
    {
        for (int row = 0; row < _height; row++)
            for (int col = 0; col < _width; col++)
            {
                byte red = (byte)(Math.Sin((double)col/_width*2*Math.PI)*127+128);
                byte green = (byte)(Math.Sin(t)*127+128);
                byte blue = (byte)(Math.Sin((double)row / _height * 2 * Math.PI+t) * 127 + 128);
                _buffer[row, col] = MakePixel(red, green, blue, 0xFF);
            }
        t+=0.1;
        return _buffer;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
        (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
}
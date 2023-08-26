using System.Drawing;

namespace NeaProject.Classes
{
    public class Sprite
    {
        private uint _colour;
        public string Name;
        public Sprite(Uri uriSprite, string name)
        {
            _colour = MakePixel(Color.White);
            Name = name;
        }

        public Sprite(string textSprite, string name) 
        {
            _colour = MakePixel(Color.Black);
            Name = name;
        }

        public Sprite(Color colour, string name)
        {
            _colour = MakePixel(colour);
            Name = name;
        }

        public uint GetColourAt(int x, int y) 
        {
            return _colour;
        }
        private static uint MakePixel(Color colour)
        {
            byte red = colour.R;
            byte green = colour.G;
            byte blue = colour.B;
            byte alpha = colour.A;
            return (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
        }
    }
}

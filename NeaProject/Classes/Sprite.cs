using System.Drawing;

namespace NeaProject.Classes
{
    public class Sprite
    {
        private uint[,] _colour = new uint[32,32];
        public string Name;
        public Sprite(Uri uriSprite, string name)
        {
            uint tileColour = MakePixel(Color.White);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    _colour[j, i] = tileColour;
                }
            }
            Name = name;
        }

        public Sprite(string textSprite, string name) 
        {
            _colour = new uint[32, 32];
            string[] spriteRows = textSprite.Split('\n').Skip(1).ToArray();
            int xIndex = 0;
            int yIndex = 0;
            foreach (string row in spriteRows)
            {
                string trimmedRow = row.Trim();
                foreach (char pixelChar in trimmedRow)
                {
                    _colour[xIndex, yIndex] = pixelChar switch
                    {
                        '.' => MakePixel(Color.Transparent),
                        'p' => MakePixel(Color.Pink),
                        'w' => MakePixel(Color.White),
                        'y' => MakePixel(Color.Yellow),
                        'b' => MakePixel(Color.CornflowerBlue),
                        'r' => MakePixel(Color.Red),
                        'k' => MakePixel(Color.Black),
                        'g' => MakePixel(Color.Green),
                        _ => MakePixel(Color.Magenta),
                    };
                    xIndex++;
                }
                xIndex = 0;
                yIndex++;
            }
            Name = name;
        }

        public Sprite(Color colour, string name)
        {
            uint tileColour = MakePixel(colour);
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    _colour[j, i] = tileColour;
                }
            }
            Name = name;
        }

        public uint GetColourAt(int x, int y) 
        {
            return _colour[x,y];
        }
        private static uint MakePixel(Color colour)
        {
            byte red = colour.R;
            byte green = colour.G;
            byte blue = colour.B;
            byte alpha = colour.A;
            var result = (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
            return result;
        }
    }
}

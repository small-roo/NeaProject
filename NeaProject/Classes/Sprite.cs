using SkiaSharp;
using System.Drawing;

namespace NeaProject.Classes
{
    public class Sprite
    {
        private readonly uint[,] _colour = new uint[32, 32];
        public string Name;
        public const int tileSize = 32;

        public Sprite(SKBitmap bitmap, string name) 
        {
            int yOffset;
            int xOffset;
            switch (name)
            {
                case "Grass":
                    {
                        xOffset = 0;
                        yOffset = 0;
                        break;
                    }
                case "Sand":
                    {
                        xOffset = 14 * tileSize;
                        yOffset = 4 * tileSize;
                        break;
                    }
                case "Rock":
                    {
                        xOffset = 28 * tileSize;
                        yOffset = 15 * tileSize;
                        break;
                    }
                case "Water":
                    {
                        xOffset = 4 * tileSize;
                        yOffset = 4 * tileSize;
                        break;
                    }
                default:
                    {
                        xOffset = 4 * tileSize;
                        yOffset = 13 * tileSize;
                        break;
                    }
            
            }

            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    uint tileColour = MakePixel(bitmap.GetPixel(x + xOffset, y + yOffset));
                    _colour[x, y] = tileColour;
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
                        'b' => MakePixel(Color.DarkSlateBlue),
                        'd' => MakePixel(Color.Maroon),
                        'e' => MakePixel(Color.Indigo),
                        'g' => MakePixel(Color.Green),
                        'h' => MakePixel(Color.PeachPuff),
                        'k' => MakePixel(Color.Black),
                        'm' => MakePixel(Color.HotPink),
                        'p' => MakePixel(Color.Pink),
                        'r' => MakePixel(Color.Crimson),
                        's' => MakePixel(Color.PapayaWhip),
                        'u' => MakePixel(Color.DarkOrchid),
                        'w' => MakePixel(Color.White),
                        'y' => MakePixel(Color.Yellow),
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

        private static uint MakePixel(SKColor colour)
        {
            byte red = colour.Red;
            byte green = colour.Green;
            byte blue = colour.Blue;
            byte alpha = colour.Alpha;
            var result = (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
            return result;
        }
    }
}

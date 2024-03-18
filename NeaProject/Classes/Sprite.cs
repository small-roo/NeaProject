using SkiaSharp;
using System.Drawing;

namespace NeaProject.Classes
{
    public class Sprite
    {
        private readonly uint[,,] _colour;
        public string Name;
        public const int tileSize = 32;

        public Sprite(SKBitmap bitmap, string name, int numOfFrames)
        {
            GetTileOffset(name, out int yOffset, out int xOffset);
            _colour = new uint[32, 32, numOfFrames]; //creates a 32x32 array for the colours used in the tile

            for (int frameIndex = 0; frameIndex < numOfFrames; frameIndex++) //loops for if there are multiple sprites for the same object
            {
                for (int y = 0; y < 32; y++) //pixel loop
                {
                    for (int x = 0; x < 32; x++)
                    {
                        uint tileColour = MakePixel(bitmap.GetPixel(x + xOffset, y + yOffset));
                        _colour[x, y, frameIndex] = tileColour;
                    }
                }
                xOffset += tileSize; //sprites with more than one frame follow each other left-to-right in the spritesheet
            }
            Name = name;
        }

        private static void GetTileOffset(string name, out int yOffset, out int xOffset) //the point in the png to start reading in the sprite
        {
            switch (name)
            {
                //teleports
                case "Spawn":
                    {
                        xOffset = 11;
                        yOffset = 2;
                        break;
                    }
                case "Teleport1":
                case "Teleport2":
                case "Teleport3":
                case "Teleport4":
                    {
                        xOffset = 1;
                        yOffset = 8;
                        break;
                    }

                //cacti
                case "Cactus": // left in in case of errors for now
                    {
                        xOffset = 39;
                        yOffset = 1;
                        break;
                    }
                case "Cactus1":
                    {
                        xOffset = 39;
                        yOffset = 2;
                        break;
                    }
                case "Cactus2":
                    {
                        xOffset = 40;
                        yOffset = 2;
                        break;
                    }
                case "Cactus3":
                    {
                        xOffset = 39;
                        yOffset = 3;
                        break;
                    }
                case "Cactus4":
                    {
                        xOffset = 40;
                        yOffset = 3;
                        break;
                    }

                //grass
                case "Grass":
                    {
                        xOffset = 0;
                        yOffset = 0;
                        break;
                    }
                case "Grass1":
                    {
                        xOffset = 0;
                        yOffset = 1;
                        break;
                    }
                case "Grass2":
                    {
                        xOffset = 1;
                        yOffset = 1;
                        break;
                    }
                case "Grass3":
                    {
                        xOffset = 0;
                        yOffset = 2;
                        break;
                    }
                case "Grass4":
                    {
                        xOffset = 1;
                        yOffset = 2;
                        break;
                    }

                //weapon
                case "SwingUp":
                    {
                        xOffset = 8;
                        yOffset = 3;
                        break;
                    }
                case "SwingRight":
                    {
                        xOffset = 8;
                        yOffset = 4;
                        break;
                    }
                case "SwingDown":
                    {
                        xOffset = 8;
                        yOffset = 5;
                        break;
                    }
                case "SwingLeft":
                    {
                        xOffset = 8;
                        yOffset = 6;
                        break;
                    }

                //other
                case "Sword":
                    {
                        xOffset = 2;
                        yOffset = 8;
                        break;
                    }
                case "Space":
                    {
                        xOffset = 46;
                        yOffset = 11;
                        break;
                    }
                case "MarsRock":
                    {
                        xOffset = 42;
                        yOffset = 25;
                        break;
                    }
                case "Player":
                    {
                        xOffset = 0;
                        yOffset = 16;
                        break;
                    }
                case "Rock":
                    {
                        xOffset = 28;
                        yOffset = 15;
                        break;
                    }
                case "Sand":
                    {
                        xOffset = 33;
                        yOffset = 25;
                        break;
                    }
                case "Tree":
                    {
                        xOffset = 8;
                        yOffset = 2;
                        break;
                    }
                case "Water":
                    {
                        xOffset = 4;
                        yOffset = 4;
                        break;
                    }
                //npcs
                case "Bird":
                    {
                        xOffset = 0;
                        yOffset = 18;
                        break;
                    }
                case "FinalBoss":
                    {
                        xOffset = 0;
                        yOffset = 17;
                        break;
                    }
                case "Fish":
                    {
                        xOffset = 5;
                        yOffset = 18;
                        break;
                    }
                case "Snake":
                    {
                        xOffset = 3;
                        yOffset = 18;
                        break;
                    }
                default:
                    {
                        xOffset = 4;
                        yOffset = 13;
                        break;
                    }

            }

            //and then multiply by the size of the tile to get the position in terms of pixels
            xOffset *= tileSize;
            yOffset *= tileSize;
        }

        public Sprite(string textSprite, string name) //for if the sprite is read in from a text file, like the map
        {
            _colour = new uint[32, 32, 1];
            string[] spriteRows = textSprite.Split('\n').Skip(1).ToArray();
            int xIndex = 0;
            int yIndex = 0;
            foreach (string row in spriteRows)
            {
                string trimmedRow = row.Trim();
                foreach (char pixelChar in trimmedRow)
                {
                    _colour[xIndex, yIndex, 0] = pixelChar switch
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

        public Sprite(Color colour, string name) //for if the sprite is a specific colour
        {
            _colour = new uint[32, 32, 1];
            uint tileColour = MakePixel(colour);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    _colour[x, y, 0] = tileColour;
                }
            }
            Name = name;
        }

        public uint GetColourAt(int x, int y, int frameIndex)
        {
            return _colour[x, y, frameIndex];
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

using System.Drawing;

namespace NeaProject.Classes
{
    public class Map
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public char[][] CharMap { get; set; } = Array.Empty<char[]>();
        public char[][] OverlayCharMap { get; set; } = Array.Empty<char[]>();

        /// <summary>
        /// Empty constructor for JSON deserialisation
        /// </summary>
        public Map() { }

        public Map(string stringMap)
        {
            string[] mapRows = stringMap.Split('\n');
            Width = mapRows[0].Length / 2; //don't need to subtract 1 because will always be even, and using integer division
            Height = mapRows.Length;
            CharMap = CreateJaggedArray(Height, Width);
            OverlayCharMap = CreateJaggedArray(Height, Width); 
            int rowIndex = 0;
            foreach (string row in mapRows) 
            {
                string trimmedRow = row.Trim();
                int tileCount = trimmedRow.Length / 2;
                for (int colIndex = 0; colIndex < tileCount; colIndex++)
                {
                    CharMap[rowIndex][colIndex] = trimmedRow[colIndex * 2];
                    OverlayCharMap[rowIndex][colIndex] = trimmedRow[colIndex * 2 + 1];
                }
                rowIndex++;
            }
            CharMap = RandomiseTiles(CharMap);
            OverlayCharMap = RandomiseTiles(OverlayCharMap);
        }

        public static char[][] CreateJaggedArray(int height, int width) 
        {
            var temp = new char[height][];
            for (int i = 0; i < height; i++)
            {
                temp[i] = new char[width];
            }
            return temp;
        }

        public char GetTileChar(int xTile, int yTile)
        {
            return CharMap[yTile][xTile];
        }
        public void SetTileChar(int xTile, int yTile, char newOverlay)
        {
            CharMap[yTile][xTile] = newOverlay;
        }

        public char GetOverlayTileChar(int xTile, int yTile)
        {
            return OverlayCharMap[yTile][xTile];
        }

        public void SetOverlayTileChar(int xTile, int yTile, char newOverlay)
        {
            OverlayCharMap[yTile][xTile] = newOverlay;
        }

        private static char[][] RandomiseTiles(char[][] currentMap)
        {
            Random random = new();
            int yTile = 0;
            foreach (char[] tileRow in currentMap) // can be reused for all randomisations
            {
                int xTile = 0;
                foreach (char tileChar in tileRow)
                {
                    switch (tileChar)
                    {
                        case 'c': //cactus
                            {
                                int cactusNumber = random.Next(0, 4);
                                switch (cactusNumber)
                                {
                                    case 0:
                                        {
                                            currentMap[yTile][xTile] = 'ç';
                                            break;
                                        }
                                    case 1:
                                        {
                                            currentMap[yTile][xTile] = 'ć';
                                            break;
                                        }
                                    case 2:
                                        {
                                            currentMap[yTile][xTile] = 'ĉ';
                                            break;
                                        }
                                    case 3:
                                        {
                                            currentMap[yTile][xTile] = 'č';
                                            break;
                                        }
                                    default:
                                        {
                                            break;
                                        }
                                }
                                break;
                            }
                        case 'g': //grass
                            {
                                int grassNumber = random.Next(0, 5);
                                switch (grassNumber)
                                {
                                    case 1:
                                        {
                                            currentMap[yTile][xTile] = 'ĝ';
                                            break;
                                        }
                                    case 2:
                                        {
                                            currentMap[yTile][xTile] = 'ğ';
                                            break;
                                        }
                                    case 3:
                                        {
                                            currentMap[yTile][xTile] = 'ġ';
                                            break;
                                        }
                                    case 4:
                                        {
                                            currentMap[yTile][xTile] = 'ģ';
                                            break;
                                        }
                                    case 0:
                                    default:
                                        {
                                            break;
                                        }
                                }
                                break;
                            }
                        default: 
                            {
                                break; 
                            }
                    }
                    xTile++;
                }
                yTile++;
            }
            return currentMap;
        }
    }
}

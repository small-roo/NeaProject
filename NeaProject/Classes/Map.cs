using System.Drawing;

namespace NeaProject.Classes
{
    public class Map
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int VisibleHeight { get; set; }
        public int VisibleWidth { get; set; }
        public char[][] CharMap { get; set; } = Array.Empty<char[]>();
        public char[][] OverlayCharMap { get; set; } = Array.Empty<char[]>();

        /// <summary>
        /// Empty constructor for JSON deserialisation
        /// </summary>
        public Map() { }

        public Map(string stringMap)
        {
            string[] mapRows = stringMap.Split('\n');
            //despite extra whitespace character, there are an even number of wanted tiles, so integer division works (don't need to subtract 1)
            Width = mapRows[0].Length / 2; //divide by 2 because there are two chars per tile
            Height = mapRows.Length;

            //have to use jagged arrays for json deserialisation
            CharMap = CreateJaggedArray(Height, Width);
            OverlayCharMap = CreateJaggedArray(Height, Width); 
            int rowIndex = 0;
            foreach (string row in mapRows) 
            {
                string trimmedRow = row.Trim(); //remove trailing whitespace
                int tileCount = trimmedRow.Length / 2; //again, divide by 2 because there are two chars per tile
                for (int colIndex = 0; colIndex < tileCount; colIndex++)
                {
                    //setting up both tilemaps
                    CharMap[rowIndex][colIndex] = trimmedRow[colIndex * 2];
                    OverlayCharMap[rowIndex][colIndex] = trimmedRow[colIndex * 2 + 1];
                }
                rowIndex++;
            }

            //set tiles like grass and cacti to be one of their random variants
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

        //these public get and set methods are mostly to reduce confusion over which way around x and y are
        public char GetTileChar(int xTile, int yTile)
        {
            return CharMap[yTile][xTile];
        }
        //unused or not, seems wrong not to have it
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

        //can be reused for all randomisations
        private static char[][] RandomiseTiles(char[][] currentMap)
        {
            Random random = new();
            int yTile = 0;
            foreach (char[] tileRow in currentMap) 
            {
                int xTile = 0;
                foreach (char tileChar in tileRow)
                {
                    //using unneccesary switches here, again because in a more developed version of the game there may be more random tile textures
                    switch (tileChar)
                    {
                        //cactus
                        case 'c':
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
                        //grass
                        case 'g':
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

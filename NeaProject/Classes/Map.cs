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

        public char GetTileChar(int yTile, int xTile)
        {
            return CharMap[yTile][xTile];
        }

        public char GetOverlayTileChar(int yTile, int xTile)
        {
            return OverlayCharMap[yTile][xTile];
        }

        public void SetOverlayTileChar(int xTile, int yTile, char newOverlay)
        {
            OverlayCharMap[yTile][xTile] = newOverlay;
        }
    }
}

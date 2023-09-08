using System.Drawing;

namespace NeaProject.Classes
{
    public class Map
    {
        public int Height = 10;
        public int Width = 20;
        private readonly char[,] charMap;
        private readonly char[,] overlayCharMap;

        public Map(string stringMap) 
        {
            charMap = new char[Height, Width];
            overlayCharMap = new char[Height, Width];
            string[] mapRows = stringMap.Split('\n');
            int rowIndex = 0;
            foreach (string row in mapRows) 
            {
                string trimmedRow = row.Trim();
                int tileCount = trimmedRow.Length / 2;
                for (int colIndex = 0; colIndex < tileCount; colIndex++)
                {
                    charMap[rowIndex, colIndex] = trimmedRow[colIndex * 2];
                    overlayCharMap[rowIndex, colIndex] = trimmedRow[colIndex * 2 + 1];
                }
                rowIndex++;
            }
        }

        public char GetTileChar(int yTile, int xTile)
        {
            return charMap[yTile, xTile];
        }

        public char GetOverlayTileChar(int yTile, int xTile)
        {
            return overlayCharMap[yTile, xTile];
        }

        public void SetOverlayTileChar(int xTile, int yTile, char newOverlay)
        {
            overlayCharMap[yTile, xTile] = newOverlay;
        }
    }
}

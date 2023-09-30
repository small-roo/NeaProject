using System.Drawing;

namespace NeaProject.Classes
{
    public class Map
    {
        public int Height;
        public int Width;
        private readonly char[,] charMap;
        private readonly char[,] overlayCharMap;

        public Map(string stringMap) 
        {
            string[] mapRows = stringMap.Split('\n');
            Width = mapRows[0].Length/2; //don't need to subtract 1 because will always be even, and using integer division
            Height = mapRows.Length;
            charMap = new char[Height, Width];
            overlayCharMap = new char[Height, Width]; 
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

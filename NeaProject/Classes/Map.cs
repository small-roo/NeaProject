using System.Drawing;

namespace NeaProject.Classes
{
    public class Map
    {
        public int Height = 10;
        public int Width = 20;
        private char[,] charMap;
        public Map(string stringMap) 
        {
            charMap = new char[Height, Width];
            string[] mapRows = stringMap.Split('\n');
            int rowIndex = 0;
            foreach (string row in mapRows) 
            {
                string trimmedRow = row.Trim();
                //foreach (char tile in trimmedRow)
                //{
                //    charMap[rowIndex, colIndex] = tile;
                //    colIndex++;
                //}
                int tileCount = trimmedRow.Length / 2;
                for (int colIndex = 0; colIndex < tileCount; colIndex++)
                {
                    charMap[rowIndex, colIndex] = trimmedRow[colIndex * 2];
                }
                rowIndex++;
            }
        }

        public char GetTileChar(int yTile, int xTile)
        {
            return charMap[yTile, xTile];
        }
    }
}

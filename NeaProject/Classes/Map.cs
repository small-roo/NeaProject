using System.Drawing;

namespace NeaProject.Classes
{
    public class Map
    {
        public int Height = 10;
        public int Width = 20;

        private string stringMap = """
            gggggggggggggggggggg
            gggggggggggrgggggggg
            ggrggggggggggggggggg
            ggggggggggggggggrggg
            gggggggrgggggggggggg
            ggggggggggggggggssss
            rgggggggggggssssswww
            ggggggssssssswwwwwww
            ssssssssswwwwwwwwwww
            sswwwwwwwwwwwwwwwwww
            """;
        private char[,] charMap;
        public Map() 
        {
            charMap = new char[Height, Width];
            string[] mapRows = stringMap.Split('\n');
            int rowIndex = 0;
            int colIndex = 0;
            foreach (string row in mapRows) 
            {
                string trimmedRow = row.Trim();
                foreach (char tile in trimmedRow)
                {
                    charMap[rowIndex, colIndex] = tile;
                    colIndex++;
                }
                colIndex = 0;
                rowIndex++;
            }
        }

        public char GetTileChar(int yTile, int xTile)
        {
            return charMap[yTile, xTile];
        }
    }
}

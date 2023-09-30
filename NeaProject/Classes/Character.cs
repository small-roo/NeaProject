namespace NeaProject.Classes
{
    public abstract class Character
    {
        public int CurrentHp { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int FrameIndex { get; set; }
        public required string Name { get; set; }
        public List<char> AllowedTiles { get; set; } = new List<char>();
        public List<string> Inventory { get; set; } = new List<string>();

        public void Move(Map _map, int moveX, int moveY)
        {
            if (XPos + moveX <= -1 || XPos + moveX >= _map.Width || YPos + moveY <= -1 || YPos + moveY >= _map.Height)
            {
                return;
            }
            //character is moving within the map

            char nextTile = _map.GetTileChar(YPos + moveY, XPos + moveX);
            char nextOverlayTile = _map.GetOverlayTileChar(YPos + moveY, XPos + moveX);
            if (!AllowedTiles.Contains(nextTile))
            {
                return;
            }
            //character is moving to a tile it is allowed to move to

            if (nextOverlayTile == '.')
            {
                XPos += moveX;
                YPos += moveY;
            }
            else if (nextOverlayTile == 'd')
            {
                XPos += moveX;
                YPos += moveY;
                Inventory.Add("Flower Bundle");
                _map.SetOverlayTileChar(XPos, YPos, '.');
            }
            else if (nextOverlayTile == 'f' && Inventory.Count(i => i == "Flower Bundle") >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Inventory.Remove("Flower Bundle");
                }
                Inventory.Add("Heartfelt Gift");
            }
            else
            {
                CurrentHp--;
            }
        }

        public bool IsDead()
        {
            return CurrentHp <= 0;
        }
    }
}

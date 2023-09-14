namespace NeaProject.Classes
{
    public abstract class Character
    {
        public int CurrentHp { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        protected readonly Map _map;
        public List<string> Inventory = new();
        private List<char> _allowedTiles = new();

        protected Character(Map map, List<char> allowedTiles)
        {
            _map = map;
            _allowedTiles = allowedTiles;
        }
        public void Move(int moveX, int moveY)
        {
            if (XPos + moveX <= -1 || XPos + moveX >= _map.Width || YPos + moveY <= -1 || YPos + moveY >= _map.Height)
            {
                return;
            }
            //character is moving within the map

            char nextTile = _map.GetTileChar(YPos + moveY, XPos + moveX);
            char nextOverlayTile = _map.GetOverlayTileChar(YPos + moveY, XPos + moveX);
            if (!_allowedTiles.Contains(nextTile))
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

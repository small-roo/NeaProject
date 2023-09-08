namespace NeaProject.Classes
{
    public class Player
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        private readonly Map _map;
        public List<string> Inventory = new();
        public int CurrentHp { get; set; } = 100;

        public Player(Map map)
        {
            XPos = map.Width / 2;
            YPos = map.Height / 2;
            _map = map;
        }

        public void Move(int moveX, int moveY)
        {
            if (XPos + moveX > -1 && XPos + moveX < _map.Width && YPos + moveY > -1 && YPos + moveY < _map.Height)
            {
                if (_map.GetOverlayTileChar(YPos + moveY, XPos + moveX) == '.')
                {
                    XPos += moveX;
                    YPos += moveY;
                }
                else if (_map.GetOverlayTileChar(YPos + moveY, XPos + moveX) == 'd')
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
        }
        public bool HasWon()
        {
            return Inventory.Count(i => i == "Flower Bundle") == 4;
        }
    }
}

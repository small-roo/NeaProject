namespace NeaProject.Classes
{
    public class Player
    {
        public int XPos { get; set; }
        public int YPos { get; set; }

        private readonly Map _map;

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
                XPos += moveX;
                YPos += moveY;
            }
        }
    }
}

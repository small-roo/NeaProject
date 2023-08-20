namespace NeaProject.Classes
{
    public class Player
    {
        public int XPos { get; set; }
        public int YPos { get; set; }
        public int CurrentHp { get; set; } = 100;

        public Player(int yPos, int xPos)
        {
            XPos = xPos;
            YPos = yPos;
        }
    }
}

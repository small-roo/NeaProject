namespace NeaProject.Classes
{
    public class Player : Character
    {
        public Player(Map map, List<char> allowedTiles) : base(map, allowedTiles)
        {
            CurrentHp = 100;
            XPos = map.Width / 2;
            YPos = map.Height / 2;
        }

        public bool HasWon()
        {
            return Inventory.Count(i => i == "Flower Bundle") == 4;
        }
    }
}

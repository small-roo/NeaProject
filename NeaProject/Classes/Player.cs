namespace NeaProject.Classes
{
    public class Player : Character
    {
        public bool HasWon()
        {
            return Inventory.Count(i => i == "Flower Bundle") == 4;
        }
    }
}

namespace NeaProject.Classes
{
    public abstract class Npc : Character
    {
        protected Npc(Map map, List<char> allowedTiles) : base(map, allowedTiles)
        {
        }
    }
}

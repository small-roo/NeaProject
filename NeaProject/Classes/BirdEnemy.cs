namespace NeaProject.Classes
{
    public class BirdEnemy : Npc
    {
        public override string Chat(Player player)
        {
            throw new NotImplementedException();
        }
        public override void MoveRules(int moveX, int moveY, Map _map)
        {
            if (NextOverlayTile == '.')
            {
                XPos += moveX;
                YPos += moveY;
            }
        }
    }
}

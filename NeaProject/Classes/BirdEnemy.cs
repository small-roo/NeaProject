namespace NeaProject.Classes
{
    public class BirdEnemy : Npc
    {
        private int _animationCountdown;
        private static readonly Random _random = new();
        public BirdEnemy()
        { 
            ResetAnimationCountdown();
        }
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
        public override void Animate()
        {
            _animationCountdown--;
            if (_animationCountdown == 0)
            {
                FrameIndex = -FrameIndex + 1;
                ResetAnimationCountdown();
            }
        }
        private void ResetAnimationCountdown()
        {
            _animationCountdown = _random.Next(5, 10);
        }
    }
}

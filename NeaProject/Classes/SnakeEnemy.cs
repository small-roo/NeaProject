using NeaProject.Engine;

namespace NeaProject.Classes
{
    public class SnakeEnemy :Npc
    {
        private int _animationCountdown;
        private static readonly Random _random = new();
        public SnakeEnemy()
        {
            ResetAnimationCountdown();
        }
        public override string Chat(Player player)
        {
            throw new NotImplementedException();
        }
        public override void MoveRules(int moveX, int moveY, Map _map, Camera _camera)
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
                int yMovement = _random.Next(-1, 2);
                if (yMovement == 0)
                {
                }
                else
                {
                }
                ResetAnimationCountdown();
            }
        }
        private void ResetAnimationCountdown()
        {
            _animationCountdown = _random.Next(10, 20);
        }
    }
}

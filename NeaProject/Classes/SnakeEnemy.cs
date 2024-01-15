using NeaProject.Engine;

namespace NeaProject.Classes
{
    public class SnakeEnemy :Npc
    {
        private int _animationCountdown;
        private static readonly Random _random = new();
        public SnakeEnemy() : base()
        {
            ResetAnimationCountdown();
        }
        public override string Chat(Player player)
        {
            throw new NotImplementedException();
        }
        public override void MoveRules(int moveX, int moveY, Map map, Camera camera)
        {
            if (NextOverlayTile == '.')
            {
                XPos += moveX;
                YPos += moveY;
            }
        }
        public override void Animate(Game game)
        {
            _animationCountdown--;
            if (_animationCountdown == 0)
            {
                int doMovement = _random.Next(0, 3); // if the movement  is greater than 1 here, it flips instead of moving
                if (doMovement == 0)
                {
                    Move(FrameIndex * -2 + 1, 0, game.Map, game.Camera);
                }
                else if (doMovement == 1 )
                {
                    int yMovement = _random.Next(0, 2);
                    Move(0, yMovement, game.Map, game.Camera);
                }
                else
                {
                    FrameIndex = -FrameIndex + 1;
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

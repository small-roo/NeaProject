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
                int doMovement = _random.Next(0, 3); //does one of 3 movements
                if (doMovement == 0)
                {
                    Move(FrameIndex * -2 + 1, 0, game.Map, game.Camera); //moves in the x direction it is facing
                }
                else if (doMovement == 1 )
                {
                    int yMovement = _random.Next(0, 2); //moves up or down
                    Move(0, yMovement * 2 - 1, game.Map, game.Camera);
                }
                else
                {
                    FrameIndex = -FrameIndex + 1; //flips
                }
                ResetAnimationCountdown();
            }
        }
        private void ResetAnimationCountdown()
        {
            _animationCountdown = _random.Next(10, 20); //amount of time between movements is random
        }
    }
}

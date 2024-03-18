using NeaProject.Engine;

namespace NeaProject.Classes
{
    public class FishEnemy:Npc
    {
            private int _animationCountdown;
            private static readonly Random _random = new();
            public FishEnemy() : base()
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
                    // if the movement is greater than 1 here, it flips instead of moving
                    int doMovement = _random.Next(0, 3); 
                    if (doMovement == 0 || doMovement == 1)
                    {
                        Move(FrameIndex * -2 + 1, 0, game.Map, game.Camera); //moves in the direction it's facing
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


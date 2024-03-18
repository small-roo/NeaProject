using NeaProject.Engine;

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
        //shouldn't be called
        public override string Chat(Player player) 
        {
            throw new NotImplementedException();
        }
        public override void MoveRules(int moveX, int moveY, Map map, Camera camera)
        {
            //allowed to move onto any of its allowed tiles if the overlay tile is empty
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
                FrameIndex = -FrameIndex + 1; //flips the bird
                ResetAnimationCountdown();
            }
        }
        //determines how long until bird next flips
        private void ResetAnimationCountdown() 
        {
            _animationCountdown = _random.Next(5, 10);
        }
    }
}

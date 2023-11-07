using NeaProject.Engine;

namespace NeaProject.Classes
{
    public class Game
    {
        public Player Player { get; set; } = new() { Name = "", SpriteRef = '\0'};
        public Map Map { get; set; } = new();
        public List<Npc> Npcs { get; set; } = new();
        public Camera Camera { get; set; } = new();
        // Movement
        public void MoveUp()
        {
            if (Player.IsDead() || Player.HasWon() )
            {
                return;
            }
            Player.DirectionFacing = 'U';
            Player.FrameIndex = 3;
            Player.Move(Map, 0, -1, Camera);
            
        }
        public void MoveRight()
        {
            if (Player.IsDead() || Player.HasWon())
            {
                return;
            }
            Player.DirectionFacing = 'R';
            Player.FrameIndex = 1;
            Player.Move(Map, 1, 0, Camera);
        }
        public void MoveDown()
        {
            if (Player.IsDead() || Player.HasWon())
            {
                return;
            }
            Player.DirectionFacing = 'D';
            Player.FrameIndex = 0;
            Player.Move(Map, 0, 1, Camera);
        }
        public void MoveLeft()
        {
            if (Player.IsDead() || Player.HasWon())
            {
                return;
            }
            Player.DirectionFacing = 'L';
            Player.FrameIndex = 2;
            Player.Move(Map, -1, 0, Camera);
        }
    }
}

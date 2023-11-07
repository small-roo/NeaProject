using NeaProject.Engine;

namespace NeaProject.Classes
{
    public abstract class Character
    {
        public int CurrentHp { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public char NextTile { get; private set; }
        public char NextOverlayTile { get; private set; }
        public char DirectionFacing { get; set; }
        public int FrameIndex { get; set; }
        public required string Name { get; set; }
        public required char SpriteRef { get; set; }
        public bool LookForDialogue { get; set; } = false;
        public bool LookForFight { get; set; } = false;
        public List<char> AllowedTiles { get; set; } = new List<char>();
        public List<string> Inventory { get; set; } = new List<string>();

        public void Move(Map _map, int moveX, int moveY, Camera _camera)
        {
            if (XPos + moveX <= -1 || XPos + moveX >= _map.Width || YPos + moveY <= -1 || YPos + moveY >= _map.Height)
            {
                return;
            }
            //character is moving within the map

            LookForDialogue = false;
            LookForFight = false;

            NextTile = _map.GetTileChar(XPos + moveX, YPos + moveY);
            NextOverlayTile = _map.GetOverlayTileChar(XPos + moveX, YPos + moveY);
            if (!AllowedTiles.Contains(NextTile))
            {
                return;
            }
            //character is moving to a tile it is allowed to move to

            MoveRules(moveX, moveY, _map, _camera);
            string? collidingNpcBehaviour = CollidingNpcBehaviour();
            if (collidingNpcBehaviour != null) 
            {
                if (collidingNpcBehaviour == "Passive" || collidingNpcBehaviour == "Neutral")
                { LookForDialogue = true; }
                if (collidingNpcBehaviour == "Hostile" || collidingNpcBehaviour == "Neutral")
                { LookForFight = true; }
            }
        }

        public virtual void Animate() { }
        public abstract void MoveRules(int moveX, int moveY, Map _map, Camera _camera);
        public virtual string? CollidingNpcBehaviour() { return ""; }

        public bool IsDead()
        {
            return CurrentHp <= 0;
        }
    }
}

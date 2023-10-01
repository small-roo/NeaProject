namespace NeaProject.Classes
{
    public abstract class Character
    {
        public int CurrentHp { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public char NextTile { get; private set; } = '\0';
        public char NextOverlayTile { get; private set; } = '\0';
        public char DirectionFacing { get; set; }
        public int FrameIndex { get; set; }
        public required string Name { get; set; }
        public bool LookForDialogue = false;
        public bool LookForFight = false;
        public List<char> AllowedTiles { get; set; } = new List<char>();
        public List<string> Inventory { get; set; } = new List<string>();

        public void Move(Map _map, int moveX, int moveY)
        {
            if (XPos + moveX <= -1 || XPos + moveX >= _map.Width || YPos + moveY <= -1 || YPos + moveY >= _map.Height)
            {
                return;
            }
            //character is moving within the map

            LookForDialogue = false;
            LookForFight = false;

            NextTile = _map.GetTileChar(YPos + moveY, XPos + moveX);
            NextOverlayTile = _map.GetOverlayTileChar(YPos + moveY, XPos + moveX);
            if (!AllowedTiles.Contains(NextTile))
            {
                return;
            }
            //character is moving to a tile it is allowed to move to

            MoveRules(moveX, moveY, NextTile, NextOverlayTile, _map);
            string? collidingNpcBehaviour = CollidingNpcBehaviour(NextOverlayTile);
            if (collidingNpcBehaviour != null) 
            {
                if (collidingNpcBehaviour == "Passive" || collidingNpcBehaviour == "Neutral")
                { LookForDialogue = true; }
                if (collidingNpcBehaviour == "Hostile" || collidingNpcBehaviour == "Neutral")
                { LookForFight = true; }
            }
        }

        public abstract void MoveRules(int moveX, int moveY, char nextTile, char nextOverlayTile, Map _map);
        public virtual string? CollidingNpcBehaviour(char nextOverlayTile) { return ""; }

        public bool IsDead()
        {
            return CurrentHp <= 0;
        }
    }
}

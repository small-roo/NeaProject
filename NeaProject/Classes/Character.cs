using NeaProject.Engine;

namespace NeaProject.Classes
{
    public abstract class Character
    {
        public int CurrentHp { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public char NextTile { get; set; }
        public char NextOverlayTile { get; set; }
        public char DirectionFacing { get; set; }
        public int FrameIndex { get; set; }
        public required string Name { get; set; }
        public required char SpriteRef { get; set; }
        public bool LookForDialogue { get; set; } = false;
        public bool LookForFight { get; set; } = false;
        public List<char> AllowedTiles { get; set; } = new List<char>();
        public List<string> Inventory { get; set; } = new List<string>();
        private readonly Map _map;
        public Character(Map map)
        {
            _map = map;
        }
        public virtual void Move(int moveX, int moveY)
        {
            if (XPos + moveX <= -1 || XPos + moveX >= _map.Width || YPos + moveY <= -1 || YPos + moveY >= _map.Height)
            {
                return;
            }
            //character is moving within the map

            NextTile = _map.GetTileChar(XPos + moveX, YPos + moveY);
            NextOverlayTile = _map.GetOverlayTileChar(XPos + moveX, YPos + moveY);
            if (!AllowedTiles.Contains(NextTile))
            {
                return;
            }
            //character is moving to a tile it is allowed to move to

            _map.SetOverlayTileChar(XPos, YPos, '.'); // deletes previous location from map
            MoveRules(moveX, moveY);
            _map.SetOverlayTileChar(XPos, YPos, SpriteRef); // adds current location to map
        }

        public virtual void Animate() { }
        public abstract void MoveRules(int moveX, int moveY);
        public virtual string? CollidingNpcBehaviour() { return ""; }
        public bool IsDead()
        {
            return CurrentHp <= 0;
        }
    }
}

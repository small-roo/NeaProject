using NeaProject.Engine;

namespace NeaProject.Classes
{
    public abstract class Character
    {
        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public char NextTile { get; set; }
        public char NextOverlayTile { get; set; }
        public char PreviousOverlayTile { get; set; } = '.';
        public char DirectionFacing { get; set; }
        public int FrameIndex { get; set; }
        public required string Name { get; set; }
        public required char SpriteRef { get; set; }
        public bool LookForDialogue { get; set; } = false;
        public bool LookForFight { get; set; } = false;
        public List<char> AllowedTiles { get; set; } = new List<char>();
        public List<string> Inventory { get; set; } = new List<string>();
        public virtual void Move(int moveX, int moveY, Map map, Camera camera)
        {
            //checks character is moving within the map
            if (XPos + moveX <= -1 || XPos + moveX >= map.Width || YPos + moveY <= -1 || YPos + moveY >= map.Height)
            {
                return;
            }

            NextTile = map.GetTileChar(XPos + moveX, YPos + moveY);
            NextOverlayTile = map.GetOverlayTileChar(XPos + moveX, YPos + moveY);

            //checks character is moving to a tile it is allowed to move to
            if (!AllowedTiles.Contains(NextTile))
            {
                return;
            }

            map.SetOverlayTileChar(XPos, YPos, '.'); // deletes previous location from map
            MoveRules(moveX, moveY, map, camera); //moves
            map.SetOverlayTileChar(XPos, YPos, SpriteRef); // adds current location to map
        }

        public virtual void Animate(Game game) { }
        public abstract void MoveRules(int moveX, int moveY, Map map, Camera camera);
        public virtual string? CollidingNpcBehaviour() { return ""; }
        //returns true if the character's health is 0 or lower
        public bool IsDead()
        {
            return CurrentHp <= 0;
        }
    }
}

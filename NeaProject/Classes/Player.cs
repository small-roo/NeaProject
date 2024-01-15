using Microsoft.AspNetCore.Components.RenderTree;
using NeaProject.Classes;
using NeaProject.Engine;
using System.Threading;

namespace NeaProject.Classes
{
    public class Player : Character
    {

        public bool HasWon()
        {
            return Inventory.Contains("Heartfelt Gift");
        }

        public override void MoveRules(int moveX, int moveY, Map map, Camera camera)
        {
            switch (NextOverlayTile)
            {
                case '.':
                    {
                        XPos += moveX;
                        YPos += moveY;
                        PreviousOverlayTile = map.GetOverlayTileChar(XPos, YPos);
                        break;
                    }
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                    {
                        XPos += moveX;
                        YPos += moveY;
                        Teleport(map, camera);
                        PreviousOverlayTile = map.GetOverlayTileChar(XPos, YPos);
                        break;
                    }
                case 'd':
                    {
                        XPos += moveX;
                        YPos += moveY;
                        Inventory.Add("Flower Bundle");
                        map.SetOverlayTileChar(XPos, YPos, '.');
                        PreviousOverlayTile = '.';
                        break;
                    }
                case 'F':
                    {
                        if (Inventory.Count(i => i == "Flower Bundle") >= 4)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Inventory.Remove("Flower Bundle");
                            }
                            Inventory.Add("Heartfelt Gift");
                        }
                        break;
                    }
                default:
                    {
                        CurrentHp--;
                        break;
                    }
            
            }
        }



        private void Teleport(Map map, Camera camera)
        {
            int charX = 0;
            int charY = 0;
            foreach (char[] charRow in map.OverlayCharMap)
            {
                foreach (char tile in map.OverlayCharMap[charY])
                {
                    if (tile == NextOverlayTile && (charX != XPos || charY != YPos))
                    {
                        XPos = charX;
                        YPos = charY;
                        camera.DrawingStartTileX = charX - 6;
                        camera.DrawingStartTileY = charY - 4;
                        return;
                    }
                    charX++;
                }
                charX = 0;
                charY++;
            }
        }

        public override void Move(int moveX, int moveY, Map map, Camera camera)
        {
            if (XPos + moveX <= -1 || XPos + moveX >= map.Width || YPos + moveY <= -1 || YPos + moveY >= map.Height)
            {
                return;
            }
            //character is moving within the map

            LookForDialogue = false;
            LookForFight = false;

            NextTile = map.GetTileChar(XPos + moveX, YPos + moveY);
            NextOverlayTile = map.GetOverlayTileChar(XPos + moveX, YPos + moveY);
            if (!AllowedTiles.Contains(NextTile))
            {
                return;
            }
            //character is moving to a tile it is allowed to move to

            map.SetOverlayTileChar(XPos, YPos, PreviousOverlayTile); // deletes previous location from map
            MoveRules(moveX, moveY, map, camera);
            map.SetOverlayTileChar(XPos, YPos, SpriteRef); // adds current location to map

            string? collidingNpcBehaviour = CollidingNpcBehaviour();
            if (collidingNpcBehaviour != null)
            {
                if (collidingNpcBehaviour == "Passive" || collidingNpcBehaviour == "Neutral")
                { LookForDialogue = true; }
                if (collidingNpcBehaviour == "Hostile" || collidingNpcBehaviour == "Neutral")
                { LookForFight = true; }
            }
        }

        public override string? CollidingNpcBehaviour()
        {
            switch (NextOverlayTile) // could be Neutral (will have dialogue and might fight you) Hostile (will fight you) or Passive (has dialogue)
            {
                case 'F':
                    { return "Neutral"; }
                default: return null;
            }
        }

        

    }
}

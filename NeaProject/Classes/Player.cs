using Microsoft.AspNetCore.Components.RenderTree;
using NeaProject.Classes;
using NeaProject.Engine;
using System.Threading;

namespace NeaProject.Classes
{
    public class Player : Character
    {
        private int stepCount = 0;
        private int stepCycle = 0;
        public bool HasWon()
        {
            return Inventory.Contains("Heartfelt Gift"); //Game is won if all 4 swords have been traded in to final boss NPC
        }

        public override void MoveRules(int moveX, int moveY, Map map, Camera camera)
        {
            switch (NextOverlayTile)
            {
                //tile is empty
                case '.': 
                    {
                        XPos += moveX;
                        YPos += moveY;
                        PreviousOverlayTile = map.GetOverlayTileChar(XPos, YPos);

                        //replenish 1 hp every 10 steps
                        stepCount++;
                        if (stepCount >= 10)
                        {
                            stepCount = 0;
                            if (CurrentHp < MaxHp)
                            {
                                CurrentHp++;
                            }
                        }
                        break;
                    }
                //contains a teleport tile - transports player to the other tile with the same character key
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
                        FrameIndex = 20; //player is in ufo
                        break;
                    }
                //contains a sword - add sword to inventory and empty the tile once the player steps off of it
                case 'd': 
                    {
                        XPos += moveX;
                        YPos += moveY;
                        stepCount++; //counts towards hp recovery
                        Inventory.Add("Sword");
                        map.SetOverlayTileChar(XPos, YPos, '.');
                        PreviousOverlayTile = '.';
                        break;
                    }
                //is the final boss - win condition - if you have all 4 swords, takes them and gives you the item to end the game
                case 'F': 
                    {
                        if (Inventory.Count(i => i == "Sword") >= 4)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Inventory.Remove("Sword");
                            }
                            Inventory.Add("Heartfelt Gift");
                        }
                        break;
                    }
                //is an NPC or a wall tile
                case 'B': 
                case 'I':
                case 'S':
                case '□':
                    {
                        break;
                    }
                default:
                    {
                        CurrentHp--;
                        break;
                    }
            
            }
            //sprite corrections if the player is swimming
            if (map.GetTileChar(XPos, YPos) == 'w')
            {
                FrameIndex += 16;
            }
            //don't try to walk inside a ufo
            else if (FrameIndex != 20)
            {
                //walk cycle has 4 frames in each direction
                stepCycle++;
                FrameIndex += (stepCycle % 4) * 4;
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
                    //if tile being checked matches the one the player just stepped onto but is not the same tile
                    if (tile == NextOverlayTile && (charX != XPos || charY != YPos)) 
                    {
                        //player moves to that location
                        XPos = charX; 
                        YPos = charY;
                        //camera is moved so the player is in the centre
                        camera.DrawingStartTileX = charX - map.VisibleWidth / 2; 
                        camera.DrawingStartTileY = charY - map.VisibleHeight / 2;
                        return;
                    }
                    charX++;
                }
                //start on a new row
                charX = 0; 
                charY++;
            }
        }

        public override void Move(int moveX, int moveY, Map map, Camera camera)
        {
            //check character is moving within the map
            if (XPos + moveX <= -1 || XPos + moveX >= map.Width || YPos + moveY <= -1 || YPos + moveY >= map.Height)
            {
                return;
            }

            LookForDialogue = false;
            LookForFight = false;

            NextTile = map.GetTileChar(XPos + moveX, YPos + moveY);
            NextOverlayTile = map.GetOverlayTileChar(XPos + moveX, YPos + moveY);
            //check character is moving to a tile it is allowed to move to
            if (!AllowedTiles.Contains(NextTile))
            {
                return;
            }

            map.SetOverlayTileChar(XPos, YPos, PreviousOverlayTile); //deletes previous location from map
            MoveRules(moveX, moveY, map, camera); //moves player
            map.SetOverlayTileChar(XPos, YPos, SpriteRef); //adds current location to map

            //interactions with NPCs
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
            switch (NextOverlayTile) //could be Neutral (has dialogue, might fight) Hostile (will fight) or Passive (has dialogue)
            {
                case 'F':
                    { return "Passive"; } //might be neutral later on, but for now you only talk to them
                case 'B':
                case 'I':
                case 'S':
                    { return "Hostile"; }
                default: return null;
            }
        }
    }
}

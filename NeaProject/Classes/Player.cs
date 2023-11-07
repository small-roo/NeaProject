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

        public override void MoveRules(int moveX, int moveY, Map _map, Camera _camera)
        {
            switch (NextOverlayTile)
            {
                case '.':
                    {
                        XPos += moveX;
                        YPos += moveY;
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
                        int charX = 0;
                        int charY = 0;
                        foreach (char[] charRow in _map.OverlayCharMap)
                        {
                            foreach (char tile in _map.OverlayCharMap[charY])
                            {
                                if (tile == NextOverlayTile && charX != XPos && charY != YPos)
                                {
                                    XPos = charX;
                                    YPos = charY;
                                    _camera.DrawingStartTileX = charX - 8;
                                    _camera.DrawingStartTileY = charY - 4;
                                    break;
                                }
                                charX++;
                            }
                            charY++;
                        }
                        break;
                    }
                case 'd':
                    {
                        XPos += moveX;
                        YPos += moveY;
                        Inventory.Add("Flower Bundle");
                        _map.SetOverlayTileChar(XPos, YPos, '.');
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

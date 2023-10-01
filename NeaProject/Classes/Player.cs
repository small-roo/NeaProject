using Microsoft.AspNetCore.Components.RenderTree;
using NeaProject.Classes;
using System.Threading;

namespace NeaProject.Classes
{
    public class Player : Character
    {
        public bool HasWon()
        {
            return Inventory.Contains("Heartfelt Gift");
        }

        public override void MoveRules(int moveX, int moveY, char nextTile, char nextOverlayTile, Map _map)
        {
            if (nextOverlayTile == '.')
            {
                XPos += moveX;
                YPos += moveY;
            }
            else if (nextOverlayTile == 'd')
            {
                XPos += moveX;
                YPos += moveY;
                Inventory.Add("Flower Bundle");
                _map.SetOverlayTileChar(XPos, YPos, '.');
            }
            else if (nextOverlayTile == 'f')
            {
                if (Inventory.Count(i => i == "Flower Bundle") >= 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Inventory.Remove("Flower Bundle");
                    }
                    Inventory.Add("Heartfelt Gift");
                }
            }
            else
            {
                CurrentHp--;
            }
        }

        public override string? CollidingNpcBehaviour(char overlayTileChar)
        {
            switch (overlayTileChar) // could be Neutral (will have dialogue and might fight you) Hostile (will fight you) or Passive (has dialogue)
            {
                case 'f':
                    { return "Neutral"; }
                default: return null;
            }
        }

    }
}

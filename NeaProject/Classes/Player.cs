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

        public override void MoveRules(int moveX, int moveY,Map _map)
        {
            if (NextOverlayTile == '.')
            {
                XPos += moveX;
                YPos += moveY;
            }
            else if (NextOverlayTile == 'd')
            {
                XPos += moveX;
                YPos += moveY;
                Inventory.Add("Flower Bundle");
                _map.SetOverlayTileChar(XPos, YPos, '.');
            }
            else if (NextOverlayTile == 'F')
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

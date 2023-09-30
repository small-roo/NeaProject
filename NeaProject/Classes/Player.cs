using Microsoft.AspNetCore.Components.RenderTree;

namespace NeaProject.Classes
{
    public class Player : Character
    {
        public bool HasWon()
        {
            return Inventory.Contains("Heartfelt Gift");
        }
    }
}

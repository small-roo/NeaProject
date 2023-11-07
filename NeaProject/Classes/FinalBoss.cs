using NeaProject.Engine;

namespace NeaProject.Classes
{
    public class FinalBoss : Npc
    {
        public int SpokenToCount { get; set; } = 0;
        public override string Chat(Player player)
        {
            int flowerCount = player.Inventory.Count(i => i == "Flower Bundle");
            if (SpokenToCount == 2 && flowerCount > 0)
            {
                SpokenToCount++;
            }
            switch (SpokenToCount)
            {
                case 0:
                    {
                        SpokenToCount++;
                        return $"Hello, {player.Name}"; 
                    }
                case 1:
                    {
                        SpokenToCount++;
                        return "Please bring me 4 Flower Bundles"; 
                    }
                case 2:
                    {
                        return "They have small red flowers on them"; 
                    }
                case 3:
                    {
                        SpokenToCount++;
                        return "Yep, that's a Flower Bundle"; 
                    }
                case 4:
                    {
                        return $"You still need {4 - flowerCount} Flower Bundle{(flowerCount == 1 ? "": "s")}";
                    }
                default:
                    { return ""; }
            }
        }
        public override void MoveRules(int moveX, int moveY, Map _map, Camera _camera)
        {
            throw new NotImplementedException();
        }
    }
}

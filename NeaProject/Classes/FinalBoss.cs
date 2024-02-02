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
                        return $"Hey, {player.Name}! I think I've figured out how to get you home!";
                    }
                case 1:
                    {
                        SpokenToCount++;
                        return "But first, you left your sword is down by the water";
                    }
                case 2:
                    {
                        return "I'll explain more once you've got your sword from the beach";
                    }
                case 3:
                    {
                        SpokenToCount++;
                        return "I thought so! See this symbol on the hilt? It belonged to one of the Lost!";
                    }
                case 4:
                    {
                        SpokenToCount++;
                        return "You remember the myth? 4 travellers? Weapons that could guide you home?";
                    }
                case 5:
                    {
                        SpokenToCount++;
                        return "I bet between all 4 of them, I could easily muster up the magic to open a portal!";
                    }
                case 6:
                    {
                        SpokenToCount++;
                        return "Most of them are rumoured to be on other planets, though";
                    }
                case 7:
                    {
                        SpokenToCount++;
                        return "Here, you can use the saucer. Bring it back in one piece!";
                    }
                    case 8:
                    {
                        return "Good luck!";
                    }
                case 9:
                    {
                        return $"You still need {4 - flowerCount} Legendary Weapon{(flowerCount == 1 ? "" : "s")}";
                    }
                default:
                    { return ""; }
            }
        }
        public override void MoveRules(int moveX, int moveY, Map map, Camera camera)
        {
            throw new NotImplementedException();
        }
    }
}

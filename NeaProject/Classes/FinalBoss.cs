using NeaProject.Engine;

namespace NeaProject.Classes
{
    public class FinalBoss : Npc
    {
        public int SpokenToCount { get; set; } = 0;
        public override string Chat(Player player)
        {
            int weaponCount = player.Inventory.Count(i => i == "Sword");
            if (SpokenToCount == 2 && weaponCount > 0)
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
                        return "You got it! You've swung it with [ . ], right? Let me have a closer look...";
                    }
                case 4:
                    {
                        SpokenToCount++;
                        return "I thought so! See this symbol on the hilt? It belonged to one of the Lost!";
                    }
                case 5:
                    {
                        SpokenToCount++;
                        return "You remember the myth? 4 travellers? Weapons that could guide you home?";
                    }
                case 6:
                    {
                        SpokenToCount++;
                        return "I bet between all 4 of them, I could easily muster up the magic to open a portal!";
                    }
                case 7:
                    {
                        SpokenToCount++;
                        return "Most of them are rumoured to be on other planets, though";
                    }
                case 8:
                    {
                        SpokenToCount++;
                        return "Here, you can use the saucer. Bring it back in one piece!";
                    }
                case 9:
                    {
                        return "Good luck!";
                    }
                case 10:
                    {
                        return $"You still need {4 - weaponCount} Legendary Weapon{(weaponCount == 1 ? "" : "s")}";
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

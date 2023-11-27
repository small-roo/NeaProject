using System.Text.Json.Serialization;

namespace NeaProject.Classes
{
    [JsonDerivedType(typeof(BirdEnemy), typeDiscriminator:"Bird")]
    [JsonDerivedType(typeof(FinalBoss), typeDiscriminator: "FinalBoss")]
    public abstract class Npc : Character
    {
        public Npc(Map map):base(map)
        { }

        public abstract string Chat(Player player);
    }
}

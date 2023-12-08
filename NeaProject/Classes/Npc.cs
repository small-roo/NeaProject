using System.Text.Json.Serialization;

namespace NeaProject.Classes
{
    [JsonDerivedType(typeof(BirdEnemy), typeDiscriminator:"Bird")]
    [JsonDerivedType(typeof(FinalBoss), typeDiscriminator: "FinalBoss")]
    [JsonDerivedType(typeof(FishEnemy), typeDiscriminator: "Fish")]
    [JsonDerivedType(typeof(SnakeEnemy), typeDiscriminator: "Snake")]
    public abstract class Npc : Character
    {
        public abstract string Chat(Player player);
    }
}

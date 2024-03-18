using System.Text.Json.Serialization;

namespace NeaProject.Classes
{
    //for json serialisation and deserialisation - allows the list of NPCs to be saved with the game
    [JsonDerivedType(typeof(BirdEnemy), typeDiscriminator:"Bird")]
    [JsonDerivedType(typeof(FinalBoss), typeDiscriminator: "FinalBoss")]
    [JsonDerivedType(typeof(FishEnemy), typeDiscriminator: "Fish")]
    [JsonDerivedType(typeof(SnakeEnemy), typeDiscriminator: "Snake")]
    public abstract class Npc : Character
    {
        public abstract string Chat(Player player);
    }
}

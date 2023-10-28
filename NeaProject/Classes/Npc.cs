using System.Text.Json.Serialization;

namespace NeaProject.Classes
{
    [JsonDerivedType(typeof(BirdEnemy), typeDiscriminator:"Bird")]
    [JsonDerivedType(typeof(FinalBoss), typeDiscriminator: "FinalBoss")]
    public abstract class Npc : Character
    {
        protected Npc():base() { }
        public abstract string Chat(Player player);
    }
}

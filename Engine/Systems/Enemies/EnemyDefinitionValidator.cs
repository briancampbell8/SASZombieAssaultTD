namespace SASZombieAssaultTD.Engine.Systems.Enemies
{
    public static class EnemyDefinitionValidator
    {
        public static bool Validate(EnemyDefinition def)
        {
            if (string.IsNullOrWhiteSpace(def.Id)) return false;
            if (string.IsNullOrWhiteSpace(def.Name)) return false;
            if (def.MaxHealth <= 0) return false;
            if (def.Speed <= 0) return false;
            if (def.Reward < 0) return false;
            if (string.IsNullOrWhiteSpace(def.SpriteId)) return false;
            return true;
        }
    }
}
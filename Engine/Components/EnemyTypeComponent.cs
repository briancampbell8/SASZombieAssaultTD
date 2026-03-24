using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Represents the type of an enemy entity.
    /// </summary>
    public class EnemyTypeComponent : BaseComponent
    {
        public EnemyType Type { get; set; }
    }
}

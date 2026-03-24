using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Represents the active state of an entity.
    /// </summary>
    public class ActiveComponent : BaseComponent
    {
        public bool IsActive { get; set; }
    }
}

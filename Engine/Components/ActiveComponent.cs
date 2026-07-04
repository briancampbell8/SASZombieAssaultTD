using SASZombieAssaultTD.Engine.ECS;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Components
{
    ///<summary>
    ///Represents the active state of an entity.
    ///</summary>
    public class ActiveComponent : BaseComponent
    {
        public bool IsActive { get; set; }
    }
}

using SASZombieAssaultTD.Engine.Navigation;

namespace SASZombieAssaultTD.Engine.Components
{
    // Numeric standardization: All continuous values use double for precision
    /// <summary>
    /// Represents the remaining lifetime of an entity.
    /// </summary>
    public class LifetimeComponent : BaseComponent
    {
        public double RemainingSeconds { get; set; }
    }
}

using System;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Base class for all tower types.
    /// </summary>
    public abstract class TowerBase
    {
        /// <summary>
        /// Unique identifier for this tower instance.
        /// </summary>
        public string Id { get; private set; }

        protected TowerBase()
        {
            // Assign a safe default ID to satisfy nullability rules.
            Id = Guid.NewGuid().ToString();
        }
    }
}
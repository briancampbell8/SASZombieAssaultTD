/*
File:    TowerComponent.cs
Purpose: ECS component for tower entities.
Features: Tower reference integration with ECS system.

P11-04-07-B: ECS component for tower entities following established component pattern.
*/

using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// ECS component that represents a tower entity.
    /// Links tower data with the ECS system.
    /// </summary>
    public class TowerComponent : BaseComponent
    {
        /// <summary>
        /// The tower instance this component represents.
        /// </summary>
        public Tower Tower { get; set; }

        /// <summary>
        /// Initializes a new tower component.
        /// </summary>
        /// <param name="tower">The tower instance.</param>
        public TowerComponent(Tower tower)
        {
            Tower = tower ?? throw new System.ArgumentNullException(nameof(tower));
        }
    }
}

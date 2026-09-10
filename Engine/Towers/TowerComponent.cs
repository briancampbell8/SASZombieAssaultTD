// ====================================================================================================
//  FILE: TowerComponent.cs
//  PATH: ./Engine/Towers/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the TowerComponent module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
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
    /// ECS component that represents a tower ECSEntityCore. Links tower data with the ECS system.
    /// </summary>
    public class TowerComponent : ECSComponents
    {
        /// <summary>
        /// The tower instance this component represents.
        /// </summary>
        public Tower Tower { get; set; }

        /// <summary>
        /// Initializes a new tower component.
        /// </summary>
        /// <param name="tower">The tower instance.</param>
        public TowerComponent(Tower tower) => Tower = tower ?? throw new System.ArgumentNullException(nameof(tower));
    }
}

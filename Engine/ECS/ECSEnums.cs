// =====================================================================================================
//  FILE: ECSEnums.cs
//  PATH: Engine/ECS/ECSEnums.cs
//  SUBSYSTEM: ECS Enums
//
//  ROLE:
//      Centralized definition hub for all ECS-wide enumerations.
//      Provides deterministic, engine-stable enum types used across ECS subsystems,
//      including systems, entities, factories, and migration modules.
//
//  RESPONSIBILITIES:
//      - Define all ECS-wide enum types in a single authoritative location.
//      - Maintain deterministic enum ordering and values for engine stability.
//      - Serve as the shared enum repository for all ECS modules.
//      - Prevent enum duplication or scattering across unrelated subsystems.
//
//  NON-RESPONSIBILITIES:
//      - Implementing system logic, ECSEntityCore logic, or factory behavior.
//      - Managing component data, runtime state, or lifecycle operations.
//      - Providing serialization or persistence for enum values.
//
//  ARCHITECTURAL NOTES:
//      - All ECS modules must reference enums exclusively from this file.
//      - Enum definitions must remain stable to preserve deterministic behavior.
//      - This file replaces all legacy enum definitions previously embedded
//        inside ECSSystem, EntityFactory, and other engine modules.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.ECS
{
    public class ECSEnums
    {
        // ---------------------------------------------------------------------------------------------
        // System Priority Definitions
        // ---------------------------------------------------------------------------------------------
        public enum SystemPriority
        {
            Lowest,
            Low,
            Normal,
            High,
            Highest
        }
        // ---------------------------------------------------------------------------------------------
        // Enemy Type Definitions
        // ---------------------------------------------------------------------------------------------

        public enum EnemyType
        {
            Zombie,
            Runner,
            Tank,
            Swarm,
            Boss,
            Aggressive,
            Patrol,
            Objective,
            Flocking
        }

        // ---------------------------------------------------------------------------------------------
        // Legacy Enemy Type Definitions
        // ---------------------------------------------------------------------------------------------

        public enum LegacyEnemyType
        {
            Basic,
            Fast,
            Tank,
            Swarm,
            Boss







        }
    }
}

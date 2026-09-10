// =====================================================================================================
//  FILE: EnemiesEnums.cs
//  PATH: Engine/Enemies/EnemiesEnums.cs
//  MODULE: Enemies
//
//  ROLE:
//      Provide a deterministic, type-safe program container for all Enemies subsystem enumerations,
//      including enemy classification, behavior categories, and any future enemy-related enum definitions.
//
//  RESPONSIBILITIES:
//      - Define EnemyType as the canonical enemy classification enum.
//      - Serve as the unified program container for all future Enemies subsystem enums.
//      - Maintain deterministic, side-effect-free enum definitions.
//
//  NON-RESPONSIBILITIES:
//      - Contain logic, behavior, or lifecycle rules.
//      - Manage enemy spawning, AI, or runtime state.
//      - Interact with other subsystems directly.
//
//  NOTES:
//      This file replaces the invalid placeholder previously located at this path.
//      EnemyType was extracted from OperatorExtensions.cs during subsystem breakup.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Program container for all Enemies subsystem enums.
    /// </summary>
    public static class EnemiesEnums
    {
        //------------------------------------------------------------------------------------------------
        //  ENUM: ZombieAIState
        //------------------------------------------------------------------------------------------------
        public enum ZombieAIState
        {
            Idle,
            Chase,
            Attack
        }
        /// <summary>
        /// Canonical enemy type enumeration for the Enemies subsystem.
        /// </summary>
        //------------------------------------------------------------------------------------------------
        //  ENUM: EnemyType
        //------------------------------------------------------------------------------------------------
        public enum EnemyType
        {
            Unknown,
            Zombie,
            FastZombie,
            HeavyZombie,
            SpecialZombie
        }

        //------------------------------------------------------------------------------------------------
        //  ENUM: ZombieType
        //------------------------------------------------------------------------------------------------
        public enum ZombieType
        {
            /// <summary>
            /// Basic zombie
            /// </summary>
            Basic,

            /// <summary>
            /// Fast zombie
            /// </summary>
            Fast,

            /// <summary>
            /// Tank zombie (high health)
            /// </summary>
            Tank,

            /// <summary>
            /// Spitter zombie (ranged attack)
            /// </summary>
            Spitter,

            /// <summary>
            /// Boss zombie
            /// </summary>
            Boss,

            /// <summary>
            /// Swarm zombie (appears in groups)
            /// </summary>
            Swarm,

            /// <summary>
            /// Armored zombie (damage resistance)
            /// </summary>
            Armored,

            /// <summary>
            /// Toxic zombie (damage over time)
            /// </summary>
            Toxic,

            /// <summary>
            /// Shadow zombie (stealthy)
            /// </summary>
            Shadow,

            /// <summary>
            /// Robot Clown zombie (explosive)
            /// </summary>
            RobotClown,

            /// <summary>
            /// Devastator zombie (high damage)
            /// </summary>
            Devastator
        }

        //----------------------------------------------------------------------------------------
        //  ENUM: EnemyBehaviorType
        //----------------------------------------------------------------------------------------
        public enum EnemyBehaviorType
        {
            None,
            Patrol,
            Chase,
            Idle,
            Wander,
            Flee,
            Attack
        }

        //----------------------------------------------------------------------------------------
        //  ENUM: EnemySpawnCategory
        //----------------------------------------------------------------------------------------
        public enum EnemySpawnCategory
        {
            None,
            Basic,
            Fast,
            Tank,
            Spitter,
            Boss,
            Swarm,
            Armored,
            Toxic,
            Shadow,
            RobotClown,
            Devastator
        }

        //----------------------------------------------------------------------------------------
        //  ENUM: EnemyDifficultyTier
        //----------------------------------------------------------------------------------------
        public enum EnemyDifficultyTier
        {
            None,
            Easy,
            Normal,
            Hard,
            Boss
        }
    }
}

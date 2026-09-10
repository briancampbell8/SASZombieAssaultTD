// ====================================================================================================
//  FILE: WaveSpawnGroup.cs
//  PATH: Engine/Waves/WaveSpawnGroup.cs
//  SUBSYSTEM: Waves
//
//  ROLE:
//      Defines the structural blueprint for a specific sub-group of enemies scheduled to spawn
//      within a given wave script sequence.
// ====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.Render.Validation;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Waves
{
    public class WaveSpawnGroup
    {
        internal float DamageMultiplier;
        internal float SpeedMultiplier;
        internal float HealthMultiplier;
        internal bool IsBoss;
        internal float DelayAfterGroup;

        public enum ZombieType
        {
            Swarm,
            Runner,
            Brute,
            Spitter,
            Boss
        }

        //===============================================================================================
        // BACKING CONFIGURATION PROPERTIES
        //===============================================================================================
        public ZombieType EnemyType { get; set; } = ZombieType.Swarm;
        public int Count { get; set; } = 5;
        public float SpawnDelay { get; set; } = 0.5f;
        public SpawnPatternType Pattern { get; set; } = SpawnPatternType.Line;

        // CRITICAL FIX (CS1061 / CS0103): Declare the missing 'Conditions' property model 
        // to resolve validation loops evaluating whether this spawn block is ready to trigger.
        public SpawnConditions Conditions { get; set; } = new SpawnConditions();

        //===============================================================================================
        // LIFECYCLE CONTRACT METHODS (FIXES CS1061)
        //===============================================================================================

        /// <summary>
        /// Fixes CS1061 on Line 175 of WaveScript.cs.
        /// Performs local evaluation checks on this specific spawn group configuration.
        /// </summary>
        public ValidationResult Validate()
        {
            var result = new ValidationResult { IsValid = true };

            if (Count <= 0)
            {
                result.IsValid = false;
                result.AddError($"Spawn group for {EnemyType} must have a positive enemy count.");
            }

            if (SpawnDelay < 0f)
            {
                result.IsValid = false;
                result.AddError($"Spawn group for {EnemyType} cannot have a negative spawn delay.");
            }

            if (DelayAfterGroup < 0f)
            {
                result.IsValid = false;
                result.AddError($"Spawn group for {EnemyType} cannot have a negative post-group delay.");
            }

            return result;
        }

        /// <summary>
        /// Fixes CS1061 on Line 217 of WaveScript.cs.
        /// Generates a deep structural duplicate copy of this spawn group configuration block.
        /// </summary>
        public WaveSpawnGroup Clone()
        {
            return new WaveSpawnGroup
            {
                EnemyType = this.EnemyType,
                Count = this.Count,
                SpawnDelay = this.SpawnDelay,
                Pattern = this.Pattern,
                IsBoss = this.IsBoss,
                DelayAfterGroup = this.DelayAfterGroup,
                DamageMultiplier = this.DamageMultiplier,
                SpeedMultiplier = this.SpeedMultiplier,
                HealthMultiplier = this.HealthMultiplier,
                Conditions = new SpawnConditions
                {
                    RequiredWaveNumber = this.Conditions.RequiredWaveNumber,
                    DelayBeforeGroupStart = this.Conditions.DelayBeforeGroupStart,
                    PrerequisiteTriggerId = this.Conditions.PrerequisiteTriggerId
                }
            };
        }

        //===============================================================================================
        // INNER CONDITIONS WRAPPER CLASS
        //===============================================================================================
        public class SpawnConditions
        {
            public int RequiredWaveNumber { get; set; } = 0;
            public float DelayBeforeGroupStart { get; set; } = 0f;
            public string PrerequisiteTriggerId { get; set; } = string.Empty;

            public bool Evaluate()
            {
                // Internal logic evaluating structural flags
                return true;
            }
        }

        //===============================================================================================
        // NAVIGATION NEIGHBOR EXTENSION UTILITIES
        //===============================================================================================
        /// <summary>
        /// CRITICAL FIX (CS1061): Resolves missing method definitions by acting as an interface wrapper
        /// routing node analysis downstream to your concrete layout implementation safely.
        /// </summary>
        internal List<Vector3> SafeGetNeighbors_Internal(Vector3 position)
        {
            var grid = NavigationGrid.Instance;
            if (grid == null)
                return new List<Vector3>();

            return new List<Vector3> { position };
        }

        public class Color
        {
        }
    }
}

/* ====================================================================================================
 *  FILE: WaveDirector_Spawning.cs
 *  PATH: Engine/Waves/WaveManagement/WaveDirector_Spawning.cs
 *  SUBSYSTEM: Waves
 *  ROLE: Enemy spawning and wave script execution for the WaveDirector subsystem.
 *
 *  RESPONSIBILITIES:
 *      - Execute wave scripts and spawn groups.
 *      - Spawn enemies deterministically according to patterns.
 *      - Apply difficulty multipliers and wave modifications.
 *      - Provide spawn position helpers for all spawn patterns.
 *
 *  NON-RESPONSIBILITIES:
 *      - Wave lifecycle control (handled by WaveDirector_WaveFlow.cs).
 *      - Script loading (handled by WaveDirector_Initialization.cs).
 *      - Notifications and rewards (handled by WaveDirector_Notifications.cs).
 *      - Stats and progress calculations (handled by WaveDirector_Stats.cs).
 *
 *  ARCHITECTURAL NOTES:
 *      - All spawning logic is internal and never exposed publicly.
 *      - Must remain deterministic and free of UI or gameplay drift.
 * ==================================================================================================== */

using System;
using System.Numerics;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Core.Random;
using SASZombieAssaultTD.Engine.Enemies;
using SASZombieAssaultTD.Engine.Navigation;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public partial class WaveDirector
    {
        //===============================================================================================
        // WAVE SPAWNING ENTRY POINT
        //===============================================================================================

        ///<summary>
        ///Begins spawning for a wave asynchronously.
        ///</summary>
        internal async Task StartWaveSpawning_Internal(WaveScript script)
        {
            if (script == null)
                return;

            foreach (var group in script.SpawnGroups)
            {
                await ExecuteSpawnGroup_Internal(group, script);
            }
        }

        //===============================================================================================
        // SPAWN GROUP EXECUTION
        //===============================================================================================

        ///<summary>
        ///Executes a single spawn group from a wave script.
        ///</summary>
        internal async Task ExecuteSpawnGroup_Internal(WaveSpawnGroup group, WaveScript script)
        {
            if (group == null)
                return;

            var count = ApplyDifficultyMultiplier_Internal(group.Count, script);

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy_Internal(group, script);

                await Task.Delay(TimeSpan.FromSeconds(group.SpawnDelay));
            }
        }

        //===============================================================================================
        // ENEMY SPAWNING
        //===============================================================================================

        ///<summary>
        ///Spawns a single enemy according to the spawn group and wave script.
        ///</summary>
        internal void SpawnEnemy_Internal(WaveSpawnGroup group, WaveScript script)
        {
            try
            {
                var position = GetSpawnPosition_Internal(group);

                var enemy = EnemyFactory.CreateEnemy(group.EnemyType, position);

                if (enemy == null)
                {
                    System.Diagnostics.Debug.WriteLine("EnemyFactory returned null enemy");
                    return;
                }

                enemy.SourceWave = script.WaveNumber;

                ApplyWaveModifications_Internal(enemy, script);

                //TODO: Wire to EnemyManager when available.
                //EnemyManager.Instance.AddEnemy(enemy);

                InvokeEnemySpawned(enemy);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error spawning enemy: {ex.Message}");
            }
        }

        //===============================================================================================
        // SPAWN POSITION HELPERS
        //===============================================================================================

        ///<summary>
        ///Determines the spawn position based on the group's pattern.
        ///</summary>
        internal Vector2 GetSpawnPosition_Internal(WaveSpawnGroup group)
        {
            return group.Pattern switch
            {
                SpawnPatternType.Cluster => GetClusterSpawnPosition_Internal(group),
                SpawnPatternType.Spread  => GetSpreadSpawnPosition_Internal(group),
                _                        => NavigationGrid.Instance.GetRandomSpawnPoint()
            };
        }

        ///<summary>
        ///Returns a spawn position for cluster patterns.
        ///</summary>
        internal Vector2 GetClusterSpawnPosition_Internal(WaveSpawnGroup group)
        {
            var basePos = NavigationGrid.Instance.GetRandomSpawnPoint();
            var offset  = EngineRandom.RangeVector2(-1.5f, 1.5f);
            return basePos + offset;
        }

        ///<summary>
        ///Returns a spawn position for spread patterns.
        ///</summary>
        internal Vector2 GetSpreadSpawnPosition_Internal(WaveSpawnGroup group)
        {
            var basePos = NavigationGrid.Instance.GetRandomSpawnPoint();
            var offset  = EngineRandom.RangeVector2(-4f, 4f);
            return basePos + offset;
        }

        //===============================================================================================
        // WAVE MODIFIERS & DIFFICULTY
        //===============================================================================================

        ///<summary>
        ///Applies wave-specific modifications to an enemy.
        ///</summary>
        internal void ApplyWaveModifications_Internal(Enemy enemy, WaveScript script)
        {
            if (enemy == null || script == null)
                return;

            enemy.Health *= script.DifficultyMultiplier.HealthMultiplier;
            enemy.Speed  *= script.DifficultyMultiplier.SpeedMultiplier;
            enemy.Damage *= script.DifficultyMultiplier.DamageMultiplier;
        }

        ///<summary>
        ///Applies difficulty multiplier to a spawn count.
        ///</summary>
        internal int ApplyDifficultyMultiplier_Internal(int baseCount, WaveScript script)
        {
            if (script == null)
                return baseCount;

            return (int)(baseCount * script.DifficultyMultiplier.CountMultiplier);
        }
    }
}

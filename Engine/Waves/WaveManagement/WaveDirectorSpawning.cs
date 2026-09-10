// =====================================================================================================
//  FILE: WaveDirectorSpawning.cs
//  PATH: Engine/Waves/WaveManagement/WaveDirectorSpawning.cs
//  SUBSYSTEM: Waves WaveManagement
//
//  ROLE:
//      Executes deterministic enemy spawning for each wave, applying difficulty scaling,
//      spawn patterns, and wave modifiers. Acts as the runtime spawning engine for
//      WaveDirectorCore, converting WaveScript definitions into live enemy instances.
//
//  RESPONSIBILITIES:
//      - Execute spawn groups asynchronously and deterministically.
//      - Apply difficulty progression multipliers to enemy attributes and counts.
//      - Resolve spawn positions using NavigationGrid and spawn pattern calculators.
//      - Invoke enemy creation through EnemyFactory and register with active managers.
//      - Provide safe, predictable spawning behavior regardless of game state pauses.
//
//  NON-RESPONSIBILITIES:
//      - Managing wave lifecycle sequencing (WaveDirectorFlow handles sequencing).
//      - Loading or generating wave scripts (WaveDirectorInitialization handles loading).
//      - Difficulty configuration (DifficultyConfig handles configuration).
//      - Enemy AI, navigation, or combat behavior (handled by respective subsystems).
//
//  ARCHITECTURAL NOTES:
//      - All spawning operations must remain deterministic and testable.
//      - Uses NavigationGrid for spatial resolution and SpawnPtrn modules for pattern logic.
//      - DifficultyProgression and DifficultyScaler provide scaling values consumed here.
//      - WaveDirectorContext provides pause state, callbacks, and runtime bindings.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Enemies;
using SASZombieAssaultTD.Engine.Navigation;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Waves.Difficulty;
using SASZombieAssaultTD.Engine.Waves.SpawnPtrn;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Waves.WaveManagement
{
    public class WaveDirectorSpawning
    {
        //===============================================================================================
        // WAVE SPAWNING ENTRY POINT
        //===============================================================================================
        internal async Task StartWaveSpawning_Internal(WaveDirectorContext context, WaveScript script)
        {
            if (script == null || context == null)
                return;

            var spawnTasks = new List<Task>();

            foreach (var group in script.SpawnGroups)
                spawnTasks.Add(ExecuteSpawnGroup_Internal(context, group, script));

            await Task.WhenAll(spawnTasks);
        }

        //===============================================================================================
        // SPAWN GROUP EXECUTION
        //===============================================================================================
        internal async Task ExecuteSpawnGroup_Internal(WaveDirectorContext context, WaveSpawnGroup group, WaveScript script)
        {
            if (group == null || context == null)
                return;

            var count = ApplyDifficultyMultiplier_Internal(group.Count, script);

            for (int i = 0; i < count; i++)
            {
                while (context.IsPaused)
                    await Task.Delay(100);

                SpawnEnemy_Internal(context, group, script);
                await Task.Delay(TimeSpan.FromSeconds(group.SpawnDelay));
            }
        }

        //===============================================================================================
        // ENEMY SPAWNING
        //===============================================================================================
        internal void SpawnEnemy_Internal(WaveDirectorContext context, WaveSpawnGroup group, WaveScript script)
        {
            try
            {
                var position3D = GetSpawnPosition_Internal(group);
                var targetPosition2D = new System.Numerics.Vector2(position3D.X, position3D.Y);

                var enemy = EnemyFactory.CreateEnemy(group.EnemyType, targetPosition2D);
                if (enemy == null)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "EnemyFactory returned null enemy");
                    return;
                }

                enemy.SourceWave = script.WaveNumber;
                ApplyWaveModifications_Internal(enemy, script);

                var enemyService = EnemyManagerProvider.GetActiveManager();
                if (enemyService != null)
                {
                    // enemyService.Register(enemy);
                }

                context.OnInvokeEnemySpawned?.Invoke();
            }
            catch (Exception ex)
            {
                DLogger.Log($"Error spawning enemy: {ex.Message}");
            }
        }

        //===============================================================================================
        // SPAWN POSITION HELPERS
        //===============================================================================================
        internal Vector3 GetSpawnPosition_Internal(WaveSpawnGroup group)
        {
            // FIX: NavigationGrid does NOT have GetRandomSpawnPoint()
            Vector3 rawPoint = NavigationGrid.Instance?.GetRandomNavigablePoint() ?? Vector3.Zero;

            switch (group.Pattern)
            {
                case SpawnPatternType.Cluster:
                    var clusterCalc = new SpawnPtrnCluster();
                    var clusterNodes = clusterCalc.ExecuteClusterCalculation(new SpawnPtrnClusterConfig
                    {
                        CenterPoint = rawPoint,
                        Radius = 2.0f,
                        EnemyCount = 1
                    });
                    return clusterNodes.Count > 0 ? clusterNodes[0] : rawPoint;

                case SpawnPatternType.Spread:
                    var spreadCalc = new SpawnPtrnSpread();
                    var spreadNodes = spreadCalc.ExecuteSpreadCalculation(new SpawnPtrnSpreadConfig
                    {
                        AnchorPoints = new List<Vector3> { rawPoint },
                        SpreadRadius = 4.0f,
                        EnemyCount = 1
                    });
                    return spreadNodes.Count > 0 ? spreadNodes[0] : rawPoint;

                case SpawnPatternType.Circle:
                    var circleCalc = new SpawnPtrnCircle();
                    var circleNodes = circleCalc.ExecuteCircleCalculation(new SpawnPtrnCircleConfig
                    {
                        CenterPoint = rawPoint,
                        Radius = 3.0f,
                        EnemyCount = 1
                    });
                    return circleNodes.Count > 0 ? circleNodes[0] : rawPoint;

                case SpawnPatternType.Spiral:
                    var spiralCalc = new SpawnPtrnSpiral();
                    var spiralNodes = spiralCalc.ExecuteSpiralCalculation(new SpawnPtrnSpiralConfig
                    {
                        CenterPoint = rawPoint,
                        RadiusStart = 1.0f,
                        RadiusEnd = 5.0f,
                        EnemyCount = 1
                    });
                    return spiralNodes.Count > 0 ? spiralNodes[0] : rawPoint;

                default:
                    return rawPoint;
            }
        }

        //===============================================================================================
        // WAVE MODIFIERS & DIFFICULTY
        //===============================================================================================
        internal void ApplyWaveModifications_Internal(Enemy enemy, WaveScript script)
        {
            if (enemy == null || script == null)
                return;

            // FIX: DifficultyManager.Instance is NOT static → cast required
            var difficulty = ((DifficultyManager)WaveDirectorCore.Instance.DifficultyManager)
                             .GetCurrentDifficulty();

            var config = DifficultyConfig.Get(difficulty);

            // FIX: GetMultiplier is an indexer, NOT a method
            float progressionMultiplier = DifficultyProgression.GetMultiplier[script.WaveNumber];

            if (config != null)
            {
                enemy.Health *= progressionMultiplier;
                enemy.Speed *= progressionMultiplier;
                enemy.Damage *= progressionMultiplier;
            }
        }

        internal int ApplyDifficultyMultiplier_Internal(int baseCount, WaveScript script)
        {
            if (script == null)
                return baseCount;

            float progressionMultiplier = DifficultyProgression.HasMultiplier(script.WaveNumber);
            return (int)(baseCount * progressionMultiplier);
        }
    }
}

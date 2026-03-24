/*
File:    IWaveSpawnGroup.cs
Purpose: Interface to break circular dependency between Enemy and WaveSpawnGroup.
Features: Defines the WaveSpawnGroup interface that Enemy can reference without circular dependency.
*/

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Interface for wave spawn group to avoid circular dependencies.
    /// </summary>
    public interface IWaveSpawnGroup
    {
        /// <summary>
        /// Trigger enemy spawned callback.
        /// </summary>
        /// <param name="enemy">Spawned enemy.</param>
        void OnEnemySpawnedCallback(Enemy enemy);

        /// <summary>
        /// Get the count of enemies in this spawn group.
        /// </summary>
        int Count { get; }
    }
}

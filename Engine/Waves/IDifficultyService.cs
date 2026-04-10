// FILE PATH: Engine/Waves/IDifficultyService.cs
// EXECUTION TRIGGER: Implemented by difficulty service and injected into WaveSpawnGroup.GetEffectiveCount
// PROGRAM PURPOSE: Defines interface for querying game difficulty multipliers for enemy count, health, damage, and speed
// PROGRAM CALLS: None (interface definition)
// PROGRAM CONTENTS: IDifficultyService interface with CurrentLevel property and GetEnemyCountMultiplier, GetEnemyHealthMultiplier, GetEnemyDamageMultiplier, GetEnemySpeedMultiplier methods, plus DifficultyLevel enum

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Interface for difficulty service providing game difficulty information.
    /// </summary>
    public interface IDifficultyService
    {
        /// <summary>
        /// Current difficulty level.
        /// </summary>
        DifficultyLevel CurrentLevel { get; }

        /// <summary>
        /// Gets the enemy count multiplier based on current difficulty.
        /// </summary>
        /// <returns>Multiplier for enemy count (e.g., 1.0f for Normal, 1.2f for Hard, 1.5f for Elite).</returns>
        float GetEnemyCountMultiplier();

        /// <summary>
        /// Gets the enemy health multiplier based on current difficulty.
        /// </summary>
        /// <returns>Multiplier for enemy health.</returns>
        float GetEnemyHealthMultiplier();

        /// <summary>
        /// Gets the enemy damage multiplier based on current difficulty.
        /// </summary>
        /// <returns>Multiplier for enemy damage.</returns>
        float GetEnemyDamageMultiplier();

        /// <summary>
        /// Gets the enemy speed multiplier based on current difficulty.
        /// </summary>
        /// <returns>Multiplier for enemy speed.</returns>
        float GetEnemySpeedMultiplier();
    }

    /// <summary>
    /// Difficulty level enumeration.
    /// </summary>
    public enum DifficultyLevel
    {
        Normal,
        Hard,
        Elite,
        Nightmare
    }
}

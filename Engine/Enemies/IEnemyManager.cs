// =====================================================================================================
//  FILE: IEnemyManager.cs
//  PATH: Engine/Enemies/IEnemyManager.cs
//  SUBSYSTEM: Enemies Management
//
//  ROLE:
//      Defines the deterministic contract for any enemy‑management subsystem.
//      Provides the minimal API required by WaveDirectorFlow, WaveDirectorSpawning,
//      and other engine systems that need to query or mutate enemy state.
//
//  RESPONSIBILITIES:
//      - Expose active enemy collections for wave‑flow evaluation.
//      - Provide deterministic clearing of all enemies between waves or resets.
//      - Serve as the stable abstraction for any concrete EnemyManager implementation.
//
//  NON-RESPONSIBILITIES:
//      - Spawning enemies (EnemyFactory handles creation).
//      - Navigation, AI, or combat behavior (handled by respective subsystems).
//      - Difficulty scaling (DifficultyProgression handles scaling).
//
//  ARCHITECTURAL NOTES:
//      - WaveDirectorWaveFlow and WaveDirectorSpawning rely on this interface.
//      - EnemyManagerProvider returns an object that must be cast to IEnemyManager.
//      - Concrete EnemyManager MUST implement this interface without exception.
// =====================================================================================================

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Enemies
{
    public interface IEnemyManager
    {
        /// <summary>
        /// Returns all active enemies currently tracked by the manager.
        /// </summary>
        IEnumerable<Enemy> GetAllEnemies();

        /// <summary>
        /// Removes all enemies from the game world deterministically.
        /// </summary>
        void ClearAllEnemies();
    }
}

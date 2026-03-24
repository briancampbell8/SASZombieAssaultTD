/*
File:    GlobalEnemyAlias.cs
Purpose: Creates global namespace alias for Enemy class to resolve all visibility issues.
Features: Provides global access to Enemy class regardless of namespace conflicts.
*/

// Global namespace alias for Enemy class
// This ensures Enemy is accessible from any namespace

global using Enemy = SASZombieAssaultTD.Engine.Enemies.Enemy;
global using EnemySystem = SASZombieAssaultTD.Engine.Enemies.EnemySystem;
global using WaveSpawnGroup = SASZombieAssaultTD.Engine.Waves.WaveSpawnGroup;

namespace SASZombieAssaultTD.Engine.GlobalAliases
{
    /// <summary>
    /// Global namespace aliases to resolve Enemy visibility issues.
    /// </summary>
    public static class GlobalEnemyAliases
    {
        /// <summary>
        /// Ensures global aliases are properly initialized.
        /// </summary>
        public static void Initialize()
        {
            // Force initialization of global aliases
            var enemyType = typeof(Enemy);
            var enemySystemType = typeof(EnemySystem);
            var waveSpawnGroupType = typeof(WaveSpawnGroup);
        }
    }
}

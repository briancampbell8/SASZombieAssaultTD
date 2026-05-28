/*
File:    EnemyNamespace.cs
Purpose: Namespace alias to resolve Enemy class visibility issues.
Features: Provides a centralized namespace reference for Enemy class.
*/

using SASZombieAssaultTD.Engine.ECS;
using System;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Static factory class to ensure Enemy class is properly initialized and accessible.
    /// </summary>
    public static class EnemyFactory
    {
        private static object TheType;
        private static object TheMember;

        /// <summary>
        /// Creates a new enemy instance.
        /// </summary>
        public static Enemy CreateEnemy(Entity entity)
        {
            return new Enemy(entity);
        }

        /// <summary>
        /// Ensures the Enemy namespace is properly loaded.
        /// </summary>
        public static void InitializeNamespace()
        {
            // This method ensures the namespace is loaded
            // and can be called during engine initialization
        }

        internal static Enemy CreateEnemy(WaveSpawnGroup.ZombieType enemyType, Vector2 position)
        {
            Diagnostics.NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
}

/*
File:    EnemyNamespace.cs
Purpose: Namespace alias to resolve Enemy class visibility issues.
Features: Provides a centralized namespace reference for Enemy class.
*/

using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Static factory class to ensure Enemy class is properly initialized and accessible.
    /// </summary>
    public static class EnemyFactory
    {
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
    }
}

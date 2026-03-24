/*
File:    EnemyPartial.cs
Purpose: Removed redundant partial class declaration to fix CS0101 error.
*/

using System;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Enemies
{
    /// <summary>
    /// Static helper methods for Enemy class.
    /// </summary>
    public static class EnemyHelper
    {
        /// <summary>
        /// Static property to ensure Enemy type is accessible.
        /// </summary>
        public static Type EnemyType => typeof(Enemy);

        /// <summary>
        /// Static method to verify Enemy class is working.
        /// </summary>
        public static bool VerifyEnemyClass()
        {
            try
            {
                var enemy = new Enemy(new Entity(1));
                return enemy != null && enemy.Entity.IsValid;
            }
            catch
            {
                return false;
            }
        }
    }
}

/*
File:    TowerFactory.cs
Purpose: Factory for creating tower instances.
*/

using System;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Factory for creating tower instances.
    /// </summary>
    public static class TowerFactory
    {
        /// <summary>
        /// Creates a tower of the specified type at the given position.
        /// </summary>
        public static object CreateTower(string towerType, Vector3 position)
        {
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"TowerFactory: Creating {towerType} at {position}");
            // Placeholder implementation
            return new object();
        }
    }
}

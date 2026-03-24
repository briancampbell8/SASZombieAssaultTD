/*
File:    TowerDatabase.cs
Purpose: Database for tower information.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Towers
{
    /// <summary>
    /// Database for tower information.
    /// </summary>
    public static class TowerDatabase
    {
        /// <summary>
        /// Gets tower data for the specified tower type.
        /// </summary>
        public static object GetTowerData(string towerType)
        {
            ModernLoggingSystem.Log("INFO", $"TowerDatabase: Getting data for {towerType}");
            // Placeholder implementation
            return new object();
        }
    }
}

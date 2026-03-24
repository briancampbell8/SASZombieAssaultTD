/*
File:    IsAfford.cs
Purpose: Simple helper that determines whether the player can afford a tower or upgrade.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Economy
{
    /// <summary>
    /// Helper class for affordability checks.
    /// </summary>
    public static class IsAfford
    {
        /// <summary>
        /// Determines if the player can afford the specified cost.
        /// </summary>
        /// <param name="cost">Cost to check.</param>
        /// <returns>True if affordable, false otherwise.</returns>
        public static bool CanAfford(int cost)
        {
            return EconomyManager.CurrentCash >= cost;
        }
        
        /// <summary>
        /// Determines if the player can afford the specified cost with a buffer.
        /// </summary>
        /// <param name="cost">Cost to check.</param>
        /// <param name="buffer">Buffer amount to maintain.</param>
        /// <returns>True if affordable with buffer, false otherwise.</returns>
        public static bool CanAfford(int cost, int buffer)
        {
            return EconomyManager.CurrentCash >= (cost + buffer);
        }
    }
}
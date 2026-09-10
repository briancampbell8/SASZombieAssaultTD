// ====================================================================================================
//  FILE: EnemyPartial.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EnemyPartial module.
//
//  RESPONSIBILITIES:
//      - Provide VerifyEnemyClass() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    EnemyPartial.cs
Purpose: Removed redundant partial class declaration to fix CS0101 error.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
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
                var enemy = new Enemy(new ECSEntityCore(1));
                return enemy != null && enemy.ECSEntityCore.IsValid;
            }
            catch
            {
                return false;
            }
        }
    }
}

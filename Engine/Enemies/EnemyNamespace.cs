// ====================================================================================================
//  FILE: EnemyNamespace.cs
//  PATH: ./Engine/Enemies/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the EnemyNamespace module.
//
//  RESPONSIBILITIES:
//      - Provide CreateEnemy() behavior for the Core subsystem.
//      - Provide InitializeNamespace() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    EnemyNamespace.cs
Purpose: Namespace alias to resolve Enemy class visibility issues.
Features: Provides a centralized namespace reference for Enemy class.
*/

using SASZombieAssaultTD.Engine.ECS;
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Enemies
{
    ///<summary>
    ///Static factory class to ensure Enemy class is properly initialized and accessible.
    ///</summary>
    public static class EnemyFactory
    {
        private static object TheType;
        private static object TheMember;

        ///<summary>
        ///Creates a new enemy instance.
        ///</summary>
        public static Enemy CreateEnemy(ECSEntityCore ECSEntityCore)
        {
            return new Enemy(ECSEntityCore);
        }

        ///<summary>
        ///Ensures the Enemy namespace is properly loaded.
        ///</summary>
        public static void InitializeNamespace()
        {
            //This method ensures the namespace is loaded
            //and can be called during engine initialization
        }

        internal static Enemy CreateEnemy(WaveSpawnGroup.ZombieType enemyType, Vector2 position)
        {
            Diagnostics.NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
}


// =====================================================================================================
//  FILE: FactoryData.cs
//  PATH: Engine/GameRoot/GamePlay/Factory/FactoryData.cs
//  SUBSYSTEM: GameRoot GamePlay Factory
//
//  ROLE:
//      Provides deterministic, centralized data definitions and lookup tables for the GamePlay
//      Factory subsystem. This module contains all static enemy stats, type definitions, and
//      legacy-to-modern type mapping rules used by GamePlay FactoryCreate and FactoryMigration.
//
//  RESPONSIBILITIES:
//      - Define EnemyType enumeration (sourced from ECS enums).
//      - Define LegacyEnemyType enumeration (sourced from ECS enums).
//      - Provide deterministic enemy stat lookup via GetEnemyStats().
//      - Provide legacy → GamePlay type mapping via MapLegacyEnemyType().
//      - Provide supporting legacy data structures (LegacyEnemy, LegacyProjectile).
//
//  NON-RESPONSIBILITIES:
//      - Entity creation logic (handled by GamePlay FactoryCreate).
//      - Legacy migration logic (handled by GamePlay FactoryMigration).
//      - Component storage or lifecycle operations.
//
//  ARCHITECTURAL NOTES:
//      - This module is part of the GamePlay Factory subsystem, not the ECS subsystem.
//      - All continuous numeric values use double for precision.
//      - All files in this subsystem use the Factory prefix per engine naming doctrine.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;
using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay.Factory
{
    internal static class FactoryData
    {
        // ---------------------------------------------------------------------------------------------
        // Enemy Stats Table
        // ---------------------------------------------------------------------------------------------

        private static readonly Dictionary<EnemyType, (double Health, double Speed, int Score)> _enemyStats =
            new Dictionary<EnemyType, (double Health, double Speed, int Score)>
            {
                { EnemyType.Zombie, (100.0, 1.0, 10) },
                { EnemyType.Runner, (60.0, 2.5, 15) },
                { EnemyType.Tank, (300.0, 0.5, 25) },
                { EnemyType.Swarm, (30.0, 1.5, 8) },
                { EnemyType.Boss, (1000.0, 1.2, 100) }
            };

        public static (double Health, double Speed, int Score) GetEnemyStats(EnemyType type)
        {
            return _enemyStats[type];
        }

        // ---------------------------------------------------------------------------------------------
        // Legacy → GamePlay Type Mapping
        // ---------------------------------------------------------------------------------------------

        public static EnemyType MapLegacyEnemyType(LegacyEnemyType legacyType)
        {
            return legacyType switch
            {
                LegacyEnemyType.Basic => EnemyType.Zombie,
                LegacyEnemyType.Fast => EnemyType.Runner,
                LegacyEnemyType.Tank => EnemyType.Tank,
                LegacyEnemyType.Swarm => EnemyType.Swarm,
                LegacyEnemyType.Boss => EnemyType.Boss,
                _ => EnemyType.Zombie
            };
        }

        // ---------------------------------------------------------------------------------------------
        // Legacy Data Structures
        // ---------------------------------------------------------------------------------------------

        public class LegacyEnemy
        {
            public LegacyEnemyType Type { get; set; }
            public Vector3 Position { get; set; }
            public float Health { get; set; }
            public float Speed { get; set; }
            public int Score { get; set; }
            public bool IsActive { get; set; }
        }

        public class LegacyProjectile
        {
            public Vector3 Position { get; set; }
            public Vector3 Velocity { get; set; }
            public float Damage { get; set; }
            public bool IsActive { get; set; }
        }
    }
}

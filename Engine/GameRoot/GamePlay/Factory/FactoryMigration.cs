// =====================================================================================================
//  FILE: FactoryMigration.cs
//  PATH: Engine/GameRoot/GamePlay/Factory/FactoryMigration.cs
//  SUBSYSTEM: Engine Migration
//
//  ROLE:
//      Provides deterministic migration routines for converting legacy engine objects into modern
//      Engine/Components-based data objects. This module is the authoritative migration surface for
//      legacy → Engine conversions, responsible for translating legacy data structures into fully
//      constructed Engine-side objects.
//
//  RESPONSIBILITIES:
//      - Provide MigrateLegacyEnemy() behavior
//      - Provide MigrateLegacyProjectile() behavior
//      - Validate legacy objects before migration
//      - Preserve legacy state (position, health, speed, score, active state)
//      - Produce stable Engine-side data objects for gameplay systems
//
//  NON-RESPONSIBILITIES:
//      - ECS ECSEntityCore creation or lifecycle management
//      - Component attachment or ECS storage
//      - World allocation or ECS runtime operations
//
//  ARCHITECTURAL NOTES:
//      - All migration routines are deterministic and side‑effect free
//      - Output objects are pure Engine data containers with no ECS dependencies
//      - This subsystem replaces legacy ECS migration logic
// =====================================================================================================
using System.Numerics;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.ECS.ECSComponents;

namespace SASZombieAssaultTD.Engine.GameRoot.GamePlay.Factory
{
    internal static class FactoryMigration
    {
        // ---------------------------------------------------------------------------------------------
        // Engine Data Models (Migration Output)
        // ---------------------------------------------------------------------------------------------

        public sealed class MigratedEnemy
        {
            private readonly ECSComponents _components;
            public string Type { get; set; }
            public Vector2 Position { get; set; }
            public HealthComponent Health { get; internal set; }
            public MovementComponent Movement { get; internal set; }
            public ScoreComponent Score { get; internal set; }
            public ActiveComponent Active { get; internal set; }
        }

        public sealed class MigratedProjectile
        {
            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
            public float Damage { get; set; }
            public ActiveComponent Active { get; internal set; }
        }

        // ---------------------------------------------------------------------------------------------
        // Legacy Enemy Migration
        // ---------------------------------------------------------------------------------------------

        public static MigratedEnemy MigrateLegacyEnemy(FactoryData.LegacyEnemy legacy)
        {
            ValidateLegacyObject(legacy);

            var migrated = new MigratedEnemy
            {
                Type = legacy.Type.ToString(),
                Position = new Vector2(legacy.Position.X, legacy.Position.Y),

                Health = new HealthComponent(legacy.Health),

                Movement = new MovementComponent
                {
                    // Legacy.Speed is a scalar; map it to X velocity
                    Velocity = new Vector2(legacy.Speed, 0)
                },

                Score = new ScoreComponent
                {
                    ScoreValue = legacy.Score
                },

                Active = new ActiveComponent
                {
                    IsActive = legacy.IsActive
                }
            };

            DLogger.Log(LogSubsystems.GamePlayFactory,
                "MIGRATION", $"Migrated legacy enemy '{legacy.Type}'");

            return migrated;
        }

        // ---------------------------------------------------------------------------------------------
        // Legacy Projectile Migration
        // ---------------------------------------------------------------------------------------------

        public static MigratedProjectile MigrateLegacyProjectile(FactoryData.LegacyProjectile legacy)
        {
            ValidateLegacyObject(legacy);

            var migrated = new MigratedProjectile
            {
                Position = new Vector2(legacy.Position.X, legacy.Position.Y),
                Velocity = new Vector2(legacy.Velocity.X, legacy.Velocity.Y),
                Damage = legacy.Damage,

                Active = new ActiveComponent
                {
                    IsActive = legacy.IsActive
                }
            };

            DLogger.Log(LogSubsystems.GamePlayFactory,
                "MIGRATION", "Migrated legacy projectile");

            return migrated;
        }

        // ---------------------------------------------------------------------------------------------
        // Internal Validation Helpers
        // ---------------------------------------------------------------------------------------------

        private static void ValidateLegacyObject(object legacy)
        {
            if (legacy == null)
                throw new System.ArgumentNullException(nameof(legacy));
        }
    }
}

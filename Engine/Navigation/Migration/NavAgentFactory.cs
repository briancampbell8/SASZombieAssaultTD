// =====================================================================================================
//  FILE: NavAgentFactory.cs
//  PATH: Engine/Navigation/Migration/NavAgentFactory.cs
//  SUBSYSTEM: Navigation – Migration
//
//  ROLE:
//      Creates new ECS entities preconfigured for NavAgent-based navigation.
//
//  RESPONSIBILITIES:
//      - Create a new ECSEntityCore.
//      - Add TransformComponent.
//      - Add NavAgentComponent with initial settings.
//      - Log creation.
//
//  NON-RESPONSIBILITIES:
//      - Migrating existing entities.
//      - Configuring NavAgent behavior based on EnemyType.
//      - Validating or analyzing migration results.
//      - Managing ECS world lifecycle.
//
//  ARCHITECTURAL NOTES:
//      - Stateless factory module.
//      - Extracted directly from NavigationMigrationHelper.cs.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Navigation.Migration
{
    internal static class NavAgentFactory
    {
        /// <summary>
        /// Creates a fully-initialized NavAgent entity using the ECS runtime.
        /// </summary>
        public static ECSEntityCore CreateNavAgentEntity(ECSRuntimeCore world, float speed, Vector3 position)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            // Create ECS entity through the runtime (correct modern ECS behavior)
            var entity = world.CreateEntity();

            // Add TransformComponent
            world.AddComponent(entity, new TransformComponent
            {
                X = position.X,
                Y = position.Y
            });

            // Add NavAgentComponent
            world.AddComponent(entity, new NavAgentComponent
            {
                Speed = speed,
                StoppingDistance = 0.5f,
                MaxRepathAttempts = 3,
                RepathOnBlock = true,
                UseFlowField = false
            });

            DLogger.Log(LogSubsystems.ResourcesPipeline,
                "MIGRATION",
                $"Created NavAgent entity {entity.Id} at {position}");

            return entity;
        }
    }
}

// =====================================================================================================
//  FILE: NavAgentConfigurator.cs
//  PATH: Engine/Navigation/Migration/NavAgentConfigurator.cs
//  SUBSYSTEM: Navigation – Migration
//
//  ROLE:
//      Applies deterministic behavior-based configuration rules to NavAgentComponent instances.
//      Used by migration programs to ensure NavAgents behave correctly after conversion.
//
//  RESPONSIBILITIES:
//      - Configure NavAgentComponent based on EnemyType behavior profiles.
//      - Apply speed, stopping distance, repath logic, and flowfield usage flags.
//      - Provide a stateless, deterministic configuration surface for migration modules.
//
//  NON-RESPONSIBILITIES:
//      - Performing ECSEntityCore migration (handled by NavMigrationSingle / NavMigrationBatch).
//      - Creating NavAgent entities (handled by NavAgentFactory).
//      - Validating or analyzing migration results.
//      - Managing ECS lifecycle or world interactions.
//
//  ARCHITECTURAL NOTES:
//      - Pure configuration module; no side effects beyond modifying the provided NavAgentComponent.
//      - Called by NavigationMigrationHelper and related migration modules.
// =====================================================================================================

using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Engine.Navigation.Migration
{
    internal static class NavAgentConfigurator
    {
        public static void ConfigureNavAgentForEnemyType(NavAgentComponent agent, EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Aggressive:
                    agent.MaxRepathAttempts = 5;
                    agent.RepathOnBlock = true;
                    break;

                case EnemyType.Patrol:
                    agent.Speed *= 0.8f;
                    agent.MaxRepathAttempts = 3;
                    agent.RepathOnBlock = true;
                    break;

                case EnemyType.Objective:
                    agent.StoppingDistance = 0.2f;
                    agent.MaxRepathAttempts = 4;
                    agent.RepathOnBlock = true;
                    break;

                case EnemyType.Flocking:
                    agent.UseFlowField = true;
                    break;
            }
        }
    }
}

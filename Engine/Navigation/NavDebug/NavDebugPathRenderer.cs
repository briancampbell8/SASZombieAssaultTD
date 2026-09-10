// =====================================================================================================
//  FILE: NavDebugPathRenderer.cs
//  PATH: Engine/Navigation/NavDebug/NavDebugPathRenderer.cs
//  SUBSYSTEM: Navigation.NavDebug
//
//  ROLE:
//      Dedicated renderer for agent path visualization.
//      Draws path segments and waypoints for agents with NavAgentComponent.
//
//  RESPONSIBILITIES:
//      - Query ECSRuntimeCore for entities containing NavAgentComponent.
//      - Render path lines between consecutive waypoints.
//      - Render waypoint circles.
//      - Enforce deterministic Option‑B subsystem boundaries.
//
//  NON-RESPONSIBILITIES:
//      - Grid rendering (NavDebugGridRenderer).
//      - Target rendering (NavDebugTargetRenderer).
//      - Flow field rendering (NavDebugFlowFieldRenderer).
//      - Debug statistics (NavDebugStatsRenderer).
//      - Async orchestration (NavDebugRenderer).
//
//  NOTES:
//      - This module replaces the path portion of the old NavigationDebugRenderer.cs.
//      - All rendering logic is isolated here to preserve subsystem boundaries.
// =====================================================================================================
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.ECS.ECSRuntime;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Navigation.NavDebug
{
    /// <summary>
    /// Renders agent navigation paths for debug visualization.
    /// </summary>
    public sealed class NavDebugPathRenderer
    {
        private readonly ECSComponents components;
        private readonly ECSEntityCore ECSEntityCoreCore;
        private readonly ComponentQueries compQueries;
        public static object ecsWorld { get; private set; }
        public object ECSEntityCore { get; private set; }

        /// <summary>
        /// Renders all agent paths.
        /// </summary>
        public void Render(
            D3D11Adapter_Core context,
            IECSRuntimeCore ecsWorld,
            object? navigationSystem)
        {
            if (context == null || ecsWorld == null)
                return;

            // ------------------------------------------------------------------------------------------------
            // Query ECSRuntimeCore for agents with NavAgentComponent
            // ------------------------------------------------------------------------------------------------
            IEnumerable<ECSEntityCore> agents = compQueries.GetEntitiesWith<NavAgentComponent>();

            foreach (var ECSEntityCore in agents)
            {
                var navAgent = components.GetComponent<NavAgentComponent>(ECSEntityCore.Id);
                var transform = components.GetComponent<TransformComponent>(ECSEntityCore.Id);

                if (navAgent == null || transform == null)
                    continue;

                if (!navAgent.PathValid || navAgent.CurrentPath == null)
                    continue;

                var path = navAgent.CurrentPath;
                if (path.Count < 2)
                    continue;

                // ------------------------------------------------------------------------------------------------
                // Draw path segments
                // ------------------------------------------------------------------------------------------------
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var start = path[i];
                    var end = path[i + 1];

                    context.DrawLine(
                                    (int)start.X, (int)start.Y,
                                    (int)end.X, (int)end.Y,
                                    NavDebugCore.Instance.PathColor
                                );
                }

                // ------------------------------------------------------------------------------------------------
                // Draw waypoints
                // ------------------------------------------------------------------------------------------------
                foreach (var waypoint in path)
                {
                    context.DrawCircle(
                        (int)waypoint.X,
                        (int)waypoint.Y,
                        3,
                        NavDebugCore.Instance.PathColor
                    );
                }
            }
        }
    }
}
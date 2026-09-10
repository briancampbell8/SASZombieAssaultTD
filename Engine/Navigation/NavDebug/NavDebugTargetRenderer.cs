// =====================================================================================================
//  FILE: NavDebugTargetRenderer.cs
//  PATH: Engine/Navigation/NavDebug/NavDebugTargetRenderer.cs
//  SUBSYSTEM: Navigation.NavDebug
//
//  ROLE:
//      Dedicated renderer for agent target visualization.
//      Draws target circles, target lines, and agent state summaries.
//
//  RESPONSIBILITIES:
//      - Query ECSRuntimeCore for entities containing NavAgentComponent + TransformComponent.
//      - Render target position indicators.
//      - Render line from agent to target.
//      - Render agent state summary text.
//
//  NON-RESPONSIBILITIES:
//      - Grid rendering (NavDebugGridRenderer).
//      - Path rendering (NavDebugPathRenderer).
//      - Flow field rendering (NavDebugFlowFieldRenderer).
//      - Debug statistics (NavDebugStatsRenderer).
//      - Async orchestration (NavDebugRenderer).
//
//  NOTES:
//      - This module replaces the target portion of the old NavigationDebugRenderer.cs.
//      - All rendering logic is isolated here to preserve subsystem boundaries.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.ECS.ECSRuntime;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Navigation.NavDebug
{
    /// <summary>
    /// Renders agent navigation targets for debug visualization.
    /// </summary>
    public sealed class NavDebugTargetRenderer
    {
        /// <summary>
        /// Renders all agent targets.
        /// </summary>
        /// 
        private readonly ECSEntityCore ECSEntityCoreCore;
        private readonly ECSComponents components;
        private readonly ComponentQueries compQueries;
        public void Render(D3D11Adapter_Core context, IECSRuntimeCore ecsWorld, object? navigationSystem)
        {
            if (context == null || ecsWorld == null)
                return;

            // Query ECSRuntimeCore for agents with NavAgentComponent + TransformComponent
            IEnumerable<ECSEntityCore> agents = ecsWorld.FindEntitiesWithComponent<NavAgentComponent>();

            foreach (var ECSEntityCore in agents)
            {
                var navAgent = components.GetComponent<NavAgentComponent>(ECSEntityCore.Id);
                var transform = components.GetComponent<TransformComponent>(ECSEntityCore.Id);

                if (navAgent == null || transform == null)
                    continue;

                var target = navAgent.TargetPosition;

                context.DrawCircle(
                    (int)target.X,
                    (int)target.Y,
                    8,
                    NavDebugCore.Instance.TargetColor
                );

                context.DrawLine(
                    (int)transform.Position.X,
                    (int)transform.Position.Y,
                    (int)target.X,
                    (int)target.Y,
                    NavDebugCore.Instance.TargetColor
                );

                string stateText = navAgent.GetStateSummary();

                context.DrawText(
                    stateText,
                    (int)transform.Position.X + 20,
                    (int)transform.Position.Y - 20,
                    Color.White
                );
            }
        }

        internal void Render(
            D3D11Adapter_Core context,
            ECSRuntimeCore ecsWorld,
            object navigationSystem)
        {
            // Basic guards: ensure required inputs are present.
            if (context == null || ecsWorld == null)
                return;

            try
            {
                // Try to obtain the entity collection from the runtime. If the project exposes
                // an Entities collection/property this will iterate it. Use reflection for
                // best‑effort inspection to avoid hard dependencies on specific component types.
                var entitiesProp = ecsWorld.GetType().GetProperty("Entities");
                if (entitiesProp == null)
                    return;

                var entitiesObj = entitiesProp.GetValue(ecsWorld);
                if (entitiesObj == null)
                    return;

                // Enumerate entities in a safe manner without making assumptions about the concrete collection type.
                if (entitiesObj is System.Collections.IEnumerable enumerable)
                {
                    foreach (var entity in enumerable)
                    {
                        if (entity == null)
                            continue;

                        // Best-effort: inspect whether the entity exposes a Position property and skip if not.
                        // Concrete rendering logic should use context and known component types; here we avoid
                        // invoking unknown APIs to remain compatible with the surrounding codebase.
                        try
                        {
                            var posProp = entity.GetType().GetProperty("Position");
                            if (posProp == null)
                                continue;

                            var posValue = posProp.GetValue(entity);

                            // At this point a concrete renderer would convert 'posValue' to the project's
                            // vector type and issue draw calls via 'context'. We intentionally do not attempt
                            // to cast or call rendering helpers here to avoid coupling to unknown APIs.
                        }
                        catch (Exception)
                        {
                            // Ignore per-entity inspection errors; continue rendering others.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Debug logging only — do not throw from a renderer helper.
                DLogger.Log($"NavDebugTargetRenderer.Render: Error during rendering: {ex.Message}");
                DLogger.Log(ex);
            }
        }
    }
}
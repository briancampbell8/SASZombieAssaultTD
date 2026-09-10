// =====================================================================================================
//  FILE: NavDebugRenderer.cs
//  PATH: Engine/Navigation/NavDebug/NavDebugRenderer.cs
//  SUBSYSTEM: Navigation.NavDebug
//
//  ROLE:
//      Orchestrator for all navigation debug visualization modules.
//      Delegates rendering work to NavDebugCore-managed modules.
//      Handles async task scheduling and render context dispatch.
//
//  RESPONSIBILITIES:
//      - Coordinate module execution based on NavDebugCore flags.
//      - Provide deterministic, stable entry point for NavDebugCore.Render().
//      - Manage async task batching for debug rendering.
//      - Enforce subsystem boundaries (no rendering logic here).
//
//  NON-RESPONSIBILITIES:
//      - Performing any rendering directly.
//      - Querying ECSRuntimeCore or NavigationGridCore.
//      - Owning debug flags or module state (NavDebugCore owns these).
//      - Mutating engine state outside NavDebug.
//
//  NOTES:
//      - This orchestrator replaces the old NavigationDebugRenderer.cs monolith.
//      - All rendering logic is now isolated in NavDebugGridRenderer, NavDebugPathRenderer,
//        NavDebugTargetRenderer, NavDebugFlowFieldRenderer, and NavDebugStatsRenderer.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Navigation.NavDebug
{
    /// <summary>
    /// Orchestrates all NavDebug rendering modules.
    /// </summary>
    public sealed class NavDebugRenderer
    {
        private readonly NavDebugCore _core;

        /// <summary>
        /// Creates a new NavDebugRenderer bound to the NavDebugCore pipeline manager.
        /// </summary>
        public NavDebugRenderer(NavDebugCore core)
        {
            _core = core ?? throw new ArgumentNullException(nameof(core));
        }

        /// <summary>
        /// Asynchronously renders all enabled NavDebug modules.
        /// </summary>
        public async Task RenderAsync(
            D3D11Adapter_Core context,
            IECSRuntimeCore ecsRuntime,
            object? navigationSystem)
        {
            if (context == null || ecsRuntime == null)
                return;

            var tasks = new List<Task>();

            try
            {
                // Grid Rendering
                if (_core.ShowGrid && _core.Grid != null)
                {
                    tasks.Add(Task.Run(() =>
                        _core.Grid.Render(context, ecsRuntime, navigationSystem)));
                }

                // Path Rendering
                if (_core.ShowPaths && _core.Paths != null)
                {
                    tasks.Add(Task.Run(() =>
                        _core.Paths.Render(context, ecsRuntime, navigationSystem)));
                }

                // Target Rendering
                if (_core.ShowTargets && _core.Targets != null)
                {
                    tasks.Add(Task.Run(() =>
                        _core.Targets.Render(context, ecsRuntime, navigationSystem)));
                }

                // Flow Field Rendering
                if (_core.ShowFlowFields && _core.FlowFields != null)
                {
                    tasks.Add(Task.Run(() =>
                        _core.FlowFields.Render(context, ecsRuntime, navigationSystem)));
                }

                // Stats Rendering
                if (_core.ShowStats && _core.Stats != null)
                {
                    tasks.Add(Task.Run(() =>
                        _core.Stats.Render(context, ecsRuntime, navigationSystem)));
                }

                if (tasks.Count > 0)
                    await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NavDebugRenderer: Error during rendering: {ex.Message}");
            }
        }

        internal object Render(D3D11Adapter_Core context, IECSRuntimeCore ecsWorld, object navigationSystem)
        {
            throw new NotImplementedException();
        }
    }
}

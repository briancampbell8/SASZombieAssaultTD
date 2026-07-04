/*
File:    NavigationDebugRenderer.cs
Purpose: P11-15-08 - Debug visualization for navigation systems.
*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Extensions;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Navigation
////using SASZombieAssaultTD.Engine.Diagnostics;
{
    ///<summary>
    ///Optimized debug visualization system for navigation components.
    ///Renders navigation grids, paths, agent targets, and flow fields.
    ///</summary>
    public sealed class NavigationDebugRenderer
    {
        private readonly ECSWorld _ecsWorld;
        private readonly object? _navigationSystem;

        ///<summary>
        ///Gets or sets whether debug rendering is enabled.
        ///</summary>
        public bool Enabled { get; set; }

        ///<summary>
        ///Gets or sets whether to render navigation grid.
        ///</summary>
        public bool ShowGrid { get; set; }

        ///<summary>
        ///Gets or sets whether to render agent paths.
        ///</summary>
        public bool ShowPaths { get; set; }

        ///<summary>
        ///Gets or sets whether to render agent targets.
        ///</summary>
        public bool ShowAgentTargets { get; set; }

        ///<summary>
        ///Gets or sets whether to render flow fields.
        ///</summary>
        public bool ShowFlowFields { get; set; }

        ///<summary>
        ///Gets or sets whether to render debug statistics.
        ///</summary>
        public bool ShowStats { get; set; }

        ///<summary>
        ///Gets or sets color for grid cell rendering.
        ///</summary>
        public uint GridColor { get; set; } = 0x40404040;

        ///<summary>
        ///Gets or sets color for blocked grid cells.
        ///</summary>
        public uint BlockedColor { get; set; } = 0x40FF4040;

        ///<summary>
        ///Gets or sets color for path rendering.
        ///</summary>
        public uint PathColor { get; set; } = 0x4040FF40;

        ///<summary>
        ///Gets or sets color for agent target rendering.
        ///</summary>
        public uint TargetColor { get; set; } = 0x40FF4040;

        ///<summary>
        ///Gets or sets color for flow field rendering.
        ///</summary>
        public uint FlowFieldColor { get; set; } = 0x40FFFF40;

        ///<summary>
        ///Initializes a new NavigationDebugRenderer.
        ///</summary>
        ///<param name="ecsWorld">The ECS world to render.</param>
        ///<param name="navigationSystem">The navigation system to visualize (optional).</param>
        public NavigationDebugRenderer(ECSWorld ecsWorld, object? navigationSystem = null)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));
            _navigationSystem = navigationSystem;
            Enabled = false;
            ShowGrid = true;
            ShowPaths = true;
            ShowAgentTargets = true;
            ShowFlowFields = true;
            ShowStats = true;

            DLogger.Log(LogSubsystems.Navigation,LogLevel.Info, "NavigationDebugRenderer: Initialized");
        }

        ///<summary>
        ///P11-15-08: Renders debug navigation information.
        ///</summary>
        ///<param name="context">The render context.</param>
        public async Task RenderAsync(IRenderContext context)
        {
            if (!Enabled || context == null)
                return;

            try
            {
                var renderTasks = new List<Task>();

                //Render navigation grid
                if (ShowGrid)
                    renderTasks.Add(Task.Run(() => RenderNavigationGrid(context)));

                //Render agent paths
                if (ShowPaths)
                    renderTasks.Add(Task.Run(() => RenderAgentPaths(context)));

                //Render agent targets
                if (ShowAgentTargets)
                    renderTasks.Add(Task.Run(() => RenderAgentTargets(context)));

                //Render flow fields
                if (ShowFlowFields)
                    renderTasks.Add(Task.Run(() => RenderFlowFields(context)));

                //Render debug statistics
                if (ShowStats)
                    renderTasks.Add(Task.Run(() => RenderDebugStats(context)));

                await Task.WhenAll(renderTasks);
            }
            catch (Exception ex)
            {
DLogger.Log(LogSubsystems.Navigation,LogLevel.Info,"ERROR",$"NavigationDebugRenderer: Error during rendering: {ex.Message}");
            }
        }

        ///<summary>
        ///Renders the navigation grid.
        ///</summary>
        ///<param name="context">The render context.</param>
        private void RenderNavigationGrid(IRenderContext context)
        {
            if (_navigationSystem is not { } navSystem) return;

            var gridProperty = navSystem.GetType().GetProperty("NavigationGrid");
            var grid = gridProperty?.GetValue(navSystem) as NavigationGrid ?? new NavigationGrid(10, 10);
            var cellSize = grid.CellSize;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var gridPos = new Vector3Int(x, y);
                    var worldPos = grid.GridToWorld(gridPos);
                    var isWalkable = grid.IsWalkable(gridPos);

                    var color = isWalkable ? Color.FromUint(GridColor) : Color.FromUint(BlockedColor);

                    //Draw cell outline
                    context.DrawRectangle(
                    (int)worldPos.X,
                    (int)worldPos.Y,
                    (int)cellSize,
                    (int)cellSize,
                    color
                    );
                }
            }

            //Draw grid border
            var origin = grid.WorldOrigin;
            var width = grid.Width * cellSize;
            var height = grid.Height * cellSize;
            context.DrawRectangle(origin.X, origin.Y, (int)width, (int)height, Color.White);

        }

        ///<summary>
        ///Renders agent paths.
        ///</summary>
        ///<param name="context">The render context.</param>
        private void RenderAgentPaths(IRenderContext context)
        {
            //TODO: Fix GetEntitiesWith method call - ECSWorld may not have this method signature
            //var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
            var agents = Array.Empty<Entity>(); //Placeholder to prevent compilation error

            foreach (var entity in agents)
            {
                var navAgent = entity.GetComponent<NavAgentComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                if (navAgent?.PathValid == true && transform != null)
                {
                    var path = navAgent.CurrentPath;
                    if (path.Count < 2)
                        continue;

                    //Draw path lines
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        var start = path[i];
                        var end = path[i + 1];

                        context.DrawLine(
                        (int)start.X, (int)start.Y,
                        (int)end.X, (int)end.Y,
                        PathColor
                        );
                    }

                    //Draw waypoints
                    foreach (var waypoint in path)
                    {
                        context.DrawCircle((int)waypoint.X, (int)waypoint.Y, 3, PathColor);
                    }
                }
            }
        }

        ///<summary>
        ///Renders agent targets.
        ///</summary>
        ///<param name="context">The render context.</param>
        private void RenderAgentTargets(IRenderContext context)
        {
            //TODO: Fix GetEntitiesWith method call - ECSWorld may not have this method signature
            //var agents = _ecsWorld.GetEntitiesWith<NavAgentComponent, TransformComponent>();
            var agents = Array.Empty<Entity>(); //Placeholder to prevent compilation error

            foreach (var entity in agents)
            {
                var navAgent = entity.GetComponent<NavAgentComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                if (navAgent != null && transform != null)
                {
                    //Draw target position
                    context.DrawCircle(
                    (int)navAgent.TargetPosition.X,
                    (int)navAgent.TargetPosition.Y,
                    8,
                    TargetColor
                    );

                    //Draw line from agent to target
                    context.DrawLine(
                    (int)transform.Position.X,
                    (int)transform.Position.Y,
                    (int)navAgent.TargetPosition.X,
                    (int)navAgent.TargetPosition.Y,
                    TargetColor
                    );

                    //Draw agent state
                    var stateText = navAgent.GetStateSummary();
                    context.DrawText(stateText, (int)transform.Position.X + 20, (int)transform.Position.Y - 20);
                }
            }
        }

        ///<summary>
        ///Renders flow fields.
        ///</summary>
        ///<param name="context">The render context.</param>
        private void RenderFlowFields(IRenderContext context)
        {
            //This would render flow field directions
            //For now, render a placeholder since flow fields are optional
            if (_navigationSystem == null) return;

            //Cast to dynamic or check for NavigationSystem interface
            dynamic navSystem = _navigationSystem;
            var grid = navSystem.NavigationGrid;
            var cellSize = grid.CellSize;

            for (int y = 0; y < grid.Height; y += 2) //Sample every other cell for performance
            {
                for (int x = 0; x < grid.Width; x += 2)
                {
                    var gridPos = new Vector3Int(x, y);
                    var worldPos = grid.GridToWorld(gridPos);

                    //Draw flow direction arrow (placeholder)
                    context.DrawCircle(worldPos, 2.0f, Color.FromUint(FlowFieldColor));
                }
            }
        }

        ///<summary>
        ///Gets the grid size display string.
        ///</summary>
        private string GetGridSize()
        {
            if (_navigationSystem == null) return "0x0";
            try
            {
                var grid = ((dynamic)_navigationSystem).NavigationGrid;
                if (grid == null) return "0x0";
                return $"{grid.Width}x{grid.Height}";
            }
            catch
            {
                return "0x0";
            }
        }

        ///<summary>
        ///Gets the agents processed count.
        ///</summary>
        private int GetAgentsProcessed()
        {
            if (_navigationSystem == null) return 0;
            try
            {
                return ((dynamic)_navigationSystem).AgentsProcessed ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        ///<summary>
        ///Gets the paths requested count.
        ///</summary>
        private int GetPathsRequested()
        {
            if (_navigationSystem == null) return 0;
            try
            {
                return ((dynamic)_navigationSystem).PathsRequested ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        ///<summary>
        ///Gets the paths completed count.
        ///</summary>
        private int GetPathsCompleted()
        {
            if (_navigationSystem == null) return 0;
            try
            {
                return ((dynamic)_navigationSystem).PathsCompleted ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        ///<summary>
        ///Renders debug statistics.
        ///</summary>
        ///<param name="context">The render context.</param>
        private void RenderDebugStats(IRenderContext context)
        {
            var stats = new List<string>
            {
                $"Navigation Debug Stats:",
                $"  Grid: {GetGridSize()}",
                $"  Agents Processed: {GetAgentsProcessed()}",
                $"  Paths Requested: {GetPathsRequested()}",
                $"  Paths Completed: {GetPathsCompleted()}",
                $"  Show Grid: {ShowGrid}",
                $"  Show Paths: {ShowPaths}",
                $"  Show Targets: {ShowAgentTargets}",
                $"  Show Flow Fields: {ShowFlowFields}"
            };

            var y = 10;
            foreach (var line in stats)
            {
                context.DrawText(line, new Vector3(10, y, 0), Color.White);
                y += 15;
            }
        }

        ///<summary>
        ///Gets debug information about the debug renderer.
        ///</summary>
        ///<returns>Debug information string.</returns>
        public string GetDebugInfo()
        {
            var info = $"NavigationDebugRenderer Debug Info:\n";
            info += $"  Enabled: {Enabled}\n";
            info += $"  Show Grid: {ShowGrid}\n";
            info += $"  Show Paths: {ShowPaths}\n";
            info += $"  Show Agent Targets: {ShowAgentTargets}\n";
            info += $"  Show Flow Fields: {ShowFlowFields}\n";
            info += $"  Show Stats: {ShowStats}\n";

            return info;
        }

        ///<summary>
        ///Toggles all debug rendering options.
        ///</summary>
        public void ToggleAll()
        {
            ShowGrid = !ShowGrid;
            ShowPaths = !ShowPaths;
            ShowAgentTargets = !ShowAgentTargets;
            ShowFlowFields = !ShowFlowFields;
            ShowStats = !ShowStats;
        }

        ///<summary>
        ///Resets all debug rendering options to defaults.
        ///</summary>
        public void ResetToDefaults()
        {
            ShowGrid = true;
            ShowPaths = true;
            ShowAgentTargets = true;
            ShowFlowFields = true;
            ShowStats = true;
        }
    }
}





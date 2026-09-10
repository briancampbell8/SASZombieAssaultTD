// =====================================================================================================
//  FILE: NavDebugCore.cs
//  PATH: Engine/Navigation/NavDebug/NavDebugCore.cs
//  SUBSYSTEM: Navigation.NavDebug
//
//  ROLE:
//      Central pipeline manager for all navigation debug visualization modules.
//      Owns the NavDebugRenderer orchestrator and exposes deterministic entry points
//      for grid, path, target, flow field, and statistics debug rendering.
//
//  RESPONSIBILITIES:
//      - Maintain NavDebug subsystem state (enabled flags, module activation).
//      - Initialize and coordinate all NavDebug renderer modules.
//      - Provide a stable API surface for NavigationDebugRenderer to call into.
//      - Enforce subsystem boundaries and deterministic Option‑B architecture.
//
//  NON-RESPONSIBILITIES:
//      - Performing any rendering directly.
//      - Querying ECSRuntimeCore or NavigationGridCore.
//      - Managing navigation logic or pathfinding systems.
//      - Owning or mutating engine state outside NavDebug.
//
//  NOTES:
//      - This is the authoritative NavDebug pipeline manager.
//      - All debug modules must be invoked through this core.
//      - Ensures clean separation between debug visualization and engine logic.
// =====================================================================================================

using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Navigation.NavDebug
{
    /// <summary>
    /// Pipeline manager for the NavDebug subsystem. Coordinates all debug visualization modules.
    /// </summary>
    public sealed class NavDebugCore
    {
        // ------------------------------------------------------------------------------------------------
        // Debug Colors
        // ------------------------------------------------------------------------------------------------
        public uint GridColor { get; set; } = 0x40404040;

        public uint BlockedColor { get; set; } = 0x40FF4040;
        public uint PathColor { get; set; } = 0x4040FF40;
        public uint TargetColor { get; set; } = 0x40FF4040;
        public uint FlowFieldColor { get; set; } = 0x40FFFF40;
        public uint GridBorderColor { get; set; } = 0xFFFFFFFF;

        // ------------------------------------------------------------------------------------------------
        // Singleton Instance
        // ------------------------------------------------------------------------------------------------
        public static NavDebugCore Instance { get; } = new NavDebugCore();

        // ------------------------------------------------------------------------------------------------
        // Internal Module References
        // ------------------------------------------------------------------------------------------------
        private readonly NavDebugGridRenderer _gridRenderer;

        private readonly NavDebugPathRenderer _pathRenderer;
        private readonly NavDebugTargetRenderer _targetRenderer;
        private readonly NavDebugFlowFieldRenderer _flowFieldRenderer;
        private readonly NavDebugStatsRenderer _statsRenderer;

        private readonly NavDebugRenderer _orchestrator;

        // ------------------------------------------------------------------------------------------------
        // Flags
        // ------------------------------------------------------------------------------------------------
        public bool Enabled { get; set; } = false;

        public bool ShowGrid { get; set; } = true;
        public bool ShowPaths { get; set; } = true;
        public bool ShowTargets { get; set; } = true;
        public bool ShowFlowFields { get; set; } = true;
        public bool ShowStats { get; set; } = true;

        // ------------------------------------------------------------------------------------------------
        // Constructor
        // ------------------------------------------------------------------------------------------------
        private NavDebugCore()
        {
            _gridRenderer = new NavDebugGridRenderer();
            _pathRenderer = new NavDebugPathRenderer();
            _targetRenderer = new NavDebugTargetRenderer();
            _flowFieldRenderer = new NavDebugFlowFieldRenderer();
            _statsRenderer = new NavDebugStatsRenderer();

            _orchestrator = new NavDebugRenderer(this);
        }

        // ------------------------------------------------------------------------------------------------
        // Public API
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Renders all enabled debug modules through the orchestrator.
        /// </summary>
        public static object Render(D3D11Adapter_Core context, IECSRuntimeCore ecsWorld, object? navigationSystem)
        {
            if (!Instance.Enabled)
                return null;
            return Instance._orchestrator.Render(context, ecsWorld, navigationSystem);





        }

        /// <summary>
        /// Accessor for the grid renderer.
        /// </summary>
        public NavDebugGridRenderer Grid => _gridRenderer;

        /// <summary>
        /// Accessor for the path renderer.
        /// </summary>
        public NavDebugPathRenderer Paths => _pathRenderer;

        /// <summary>
        /// Accessor for the target renderer.
        /// </summary>
        public NavDebugTargetRenderer Targets => _targetRenderer;

        /// <summary>
        /// Accessor for the flow field renderer.
        /// </summary>
        public NavDebugFlowFieldRenderer FlowFields => _flowFieldRenderer;

        /// <summary>
        /// Accessor for the stats renderer.
        /// </summary>
        public NavDebugStatsRenderer Stats => _statsRenderer;

        /// <summary>
        /// Toggles all debug modules.
        /// </summary>
        public void ToggleAll()
        {
            ShowGrid = !ShowGrid;
            ShowPaths = !ShowPaths;
            ShowTargets = !ShowTargets;
            ShowFlowFields = !ShowFlowFields;
            ShowStats = !ShowStats;
        }

        /// <summary>
        /// Resets all debug flags to defaults.
        /// </summary>
        public void ResetToDefaults()
        {
            ShowGrid = true;
            ShowPaths = true;
            ShowTargets = true;
            ShowFlowFields = true;
            ShowStats = true;
        }
    }
}

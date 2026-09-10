// ====================================================================================================
//  FILE: ImagingRenderer.cs
//  PATH: Engine/Physics/Imaging/
//  MODULE: Imaging
//
//  ROLE:
//      High‑level coordinator for the Physics Imaging subsystem. Delegates all visualization work to
//      specialized imaging modules (grid, shapes, text, stats).
//
//  RESPONSIBILITIES:
//      - Hold Imaging subsystem flags (ShowGrid, ShowShapes, ShowBounds, ShowStats).
//      - Hold Imaging subsystem colors.
//      - Invoke ImagingGrid, ImagingShapes, ImagingText, and ImagingStatsParser modules.
//      - Provide Render() entry point for physics visualization.
//      - Provide ResetToDefaults() and ToggleAll() behavior.
//      - Provide GetInfo() for developer inspection.
//
//  NON-RESPONSIBILITIES:
//      - Performing collision detection.
//      - Performing physics simulation.
//      - Performing diagnostics or debugging in the dictionary sense.
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Physics.Imaging
{
    public sealed class ImagingRenderer
    {
        private readonly ECSRuntimeCore _ecsWorld;

        // Imaging flags
        public bool Enabled { get; set; } = true;
        public bool ShowGrid { get; set; } = true;
        public bool ShowShapes { get; set; } = true;
        public bool ShowBounds { get; set; } = false;
        public bool ShowStats { get; set; } = true;

        // Imaging colors
        public uint GridColor { get; set; } = 0x40404040;
        public uint ShapeColor { get; set; } = 0x40FF4040;
        public uint BoundsColor { get; set; } = 0x4040FFFF;
        public uint TriggerColor { get; set; } = 0x40FFFF40;

        // Sub‑modules
        private readonly ImagingGrid _grid;
        private readonly ImagingShapes _shapes;
        private readonly ImagingText _text;
        private readonly ImagingStatsParser _stats;

        public ImagingRenderer(ECSRuntimeCore ecsWorld)
        {
            _ecsWorld = ecsWorld ?? throw new ArgumentNullException(nameof(ecsWorld));

            _grid = new ImagingGrid(ecsWorld);
            _shapes = new ImagingShapes(ecsWorld);
            _text = new ImagingText(ecsWorld);
            _stats = new ImagingStatsParser(ecsWorld);

            DLogger.Log(LogSubsystems.Physics, LogLevel.Info, "ImagingRenderer: Initialized");
        }

        // -------------------------------------------------------------------------------------------------
        // Render (UI Context)
        // -------------------------------------------------------------------------------------------------
        public void Render(D3D11Adapter_Core context)
        {
            if (!Enabled || context == null)
                return;

            try
            {
                // 1. Grid
                if (ShowGrid)
                    _grid.Render(context, GridColor);

                // 2. Shapes + Bounds
                _shapes.GetRender(
                    context,
                    ShapeColor,
                    BoundsColor,
                    TriggerColor,
                    ShowShapes,
                    ShowBounds);

                // 3. Stats
                if (ShowStats)
                {
                    _text.GetRenderStats(
                        context,
                        ShowGrid,
                        ShowShapes,
                        ShowBounds,
                        ShowStats);

                    _stats.GetRender(context);
                }
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Physics, LogLevel.Error,
                    $"ImagingRenderer: Render failed: {ex.Message}");
            }
        }

        // -------------------------------------------------------------------------------------------------
        // Render (Adapter Context)
        // -------------------------------------------------------------------------------------------------
        public void GetRender(D3D11Adapter_Core adapter)
        {
            if (!Enabled || adapter == null)
                return;

            // Forward to UI context if available
            if (adapter is D3D11Adapter_Core ui)
                Render(ui);
        }

        // -------------------------------------------------------------------------------------------------
        // Info / Flags
        // -------------------------------------------------------------------------------------------------
        public string GetInfo() =>
            $"ImagingRenderer Info:\n" +
            $"  Enabled: {Enabled}\n" +
            $"  Show Grid: {ShowGrid}\n" +
            $"  Show Shapes: {ShowShapes}\n" +
            $"  Show Bounds: {ShowBounds}\n" +
            $"  Show Stats: {ShowStats}\n";

        public void ToggleAll()
        {
            ShowGrid = !ShowGrid;
            ShowShapes = !ShowShapes;
            ShowBounds = !ShowBounds;
            ShowStats = !ShowStats;
        }

        public void ResetToDefaults()
        {
            ShowGrid = true;
            ShowShapes = true;
            ShowBounds = false;
            ShowStats = true;
        }
    }
}

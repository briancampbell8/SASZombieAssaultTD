// ====================================================================================================
//  FILE: StaticLayoutRenderer.cs
//  PATH: ./Engine/UI/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide Render() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
// File Path: Engine/UI/StaticLayoutRenderer.cs
// File: StaticLayoutRenderer.cs
// Program: StaticLayoutRenderer
// Subsystem: UI / Legacy Static Layout Rendering
//
// Purpose:
//     Renders static UI layout images using texture handles supplied by
//     StaticLayoutLoader. This renderer draws only pre-defined static UI
//     elements with no animation or behavioral logic. All visibility,
//     ordering, and asset resolution are handled externally.
//
// Responsibilities:
//     - Iterate through StaticLayout image definitions
//     - Validate visibility and texture handle availability
//     - Issue deterministic draw commands via D3D11Adapter_Core
//     - Emit EngineDiagnostics trace events for all rendering actions
//
// Doctrine:
//     - No System.Diagnostics.Debug in modern engine (use DLogger instead)
//     - No silent failures; all skips must be logged
//     - No fallback rendering logic beyond explicit visibility checks
//     - Renderer performs no state mutation; pure read → draw pipeline
//
// Modernization Notes:
//     - This class is part of the legacy UI/Rendering subsystem
//     - Scheduled for migration into unified Engine.Render pipeline
//     - Color pipeline will be upgraded to Engine.Core.Color
//     - StaticLayout + StaticLayoutImage will be replaced by modern UIState
//============================================================================

using System;
using System.Collections.Generic;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.UI
{
    internal sealed class StaticLayoutRenderer
    {
        private readonly StaticLayout _layout;
        private readonly StaticLayoutLoader _loader;

        internal StaticLayoutRenderer(StaticLayout layout, StaticLayoutLoader loader)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        public void Render(D3D11Adapter_Core adapter_Core)
        {
            DLogger.Log($"[StaticLayoutRenderer] Render called, image count: {_layout.Images.Count}");

            foreach (var image in _layout.Images)
            {
                DLogger.Log($"[StaticLayoutRenderer] Processing image: {image.Id}");

                if (!image.Visible)
                {
                    DLogger.Log($"[StaticLayoutRenderer] Image {image.Id} skipped - not visible");
                    continue;
                }

                var handle = _loader.TryGetHandle(image.Id);
                if (handle == null)
                {
                    DLogger.Log($"[StaticLayoutRenderer] Image {image.Id} skipped - handle is null");
                    continue;
                }

                DLogger.Log($"[StaticLayoutRenderer] Drawing" +
                    $" {image.Id} at ({image.X},{image.Y}) size {image.Width}x{image.Height}");

                adapter_Core.DrawSprite(
                    handle,
                    new System.Drawing.Rectangle(image.X, image.Y, image.Width, image.Height),
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    0.0f
                );

                DLogger.Log($"[StaticLayoutRenderer] DrawSprite completed for {image.Id}");
            }
        }
    }

    internal sealed class StaticLayout
    {
        internal List<StaticLayoutImage> Images { get; } = new();
    }

    internal sealed class StaticLayoutImage
    {
        internal string Id { get; set; }
        internal string Path { get; set; }
        internal int X { get; set; }
        internal int Y { get; set; }
        internal int Width { get; set; }
        internal int Height { get; set; }
        internal bool Visible { get; set; } = true;
    }
}


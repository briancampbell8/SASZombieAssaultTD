// ============================================================================
// File Path: Engine/UI/UIFinalizer.cs
// File: UIFinalizer.cs
// Program: UIFinalizer
// Subsystem: UI / HUD Finalization Pipeline
//
// Purpose:
//     Acts as the authoritative HUD panel state container and finalization
//     engine. Converts HUD configuration + runtime UI state into a list of
//     UIRenderable objects ready for GPU submission.
//
// Responsibilities:
//     - Hold HUD panel geometry and crosshair state
//     - Hold manual color override state
//     - Accept normalized colors from HUDColorInputProcessor
//     - Accept geometry from HUDConfigManager
//     - Convert UIState → UIRenderable list
//     - Emit deterministic EngineDiagnostics trace events
//
// Doctrine:
//     - No System.Diagnostics
//     - No silent failures
//     - No fallback logic except explicit transparent defaults
//     - Deterministic output
// ============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class UIFinalizer
    {
        // =====================================================================
        // PANEL GEOMETRY (required by HUDConfigManager)
        // =====================================================================

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; } = 0f;

        // =====================================================================
        // CROSSHAIR (required by HUDConfigManager)
        // =====================================================================

        public int CrosshairCenterX { get; set; }
        public int CrosshairCenterY { get; set; }
        public int CrosshairArmLength { get; set; }
        public int CrosshairThickness { get; set; }

        public System.Drawing.Color CrosshairColor { get; set; } = System.Drawing.Color.White;
        public bool UseFlashColor { get; set; }
        public System.Drawing.Color FlashColor { get; set; } = System.Drawing.Color.Red;

        // =====================================================================
        // COLOR PIPELINE (required by HUDColorInputProcessor)
        // =====================================================================

        public System.Drawing.Color FillColor { get; set; } = System.Drawing.Color.Transparent;
        public System.Drawing.Color TextColor { get; set; } = System.Drawing.Color.White;
        public bool ManualColorOverride { get; set; }

        // =====================================================================
        // FINAL OUTPUT TEXTURE (optional)
        // =====================================================================

        public object FinalTexture { get; set; }

        // =====================================================================
        // FINALIZATION PIPELINE
        // =====================================================================

        /// <summary>
        /// Converts UIState → UIRenderable list.
        /// </summary>
        public IReadOnlyList<UIRenderable> FinalizeUI(UIState uiState)
        {
            Engine.Diagnostics.DebugLogger.Trace("UIFinalizer.FinalizeUI.Start",
                uiState == null ? "uiState=NULL" : $"Elements={uiState.UIStateElements?.Count}");

            if (uiState == null)
            {
                Engine.Diagnostics.DebugLogger.Trace("UIFinalizer.FinalizeUI.Error", "uiState is NULL");
                return Array.Empty<UIRenderable>();
            }

            if (uiState.UIStateElements == null || uiState.UIStateElements.Count == 0)
            {
                Engine.Diagnostics.DebugLogger.Trace("UIFinalizer.FinalizeUI.Empty", "No elements to finalize");
                return Array.Empty<UIRenderable>();
            }

            var finalized = new List<UIRenderable>(uiState.UIStateElements.Count);

            foreach (var element in uiState.UIStateElements)
            {
                if (element == null)
                {
                    Engine.Diagnostics.DebugLogger.Trace("UIFinalizer.FinalizeUI.Skip.NullElement", "NULL");
                    continue;
                }

                // If already a UIRenderable, pass through
                if (element is UIRenderable renderable)
                {
                    finalized.Add(renderable);
                    continue;
                }

                // Default conversion for non-renderable elements
                finalized.Add(new UIRenderable
                {
                    Id = element.ToString() ?? "unknown",
                    Bounds = Rectangle.Empty,
                    TextureId = string.Empty,
                    Color = System.Drawing.Color.Transparent,
                    Depth = 0f,
                    Clip = null,
                    Type = element.GetType().Name,
                    Texture = null
                });
            }

            Engine.Diagnostics.DebugLogger.Trace("UIFinalizer.FinalizeUI.Complete",
                $"Output={finalized.Count}");

            return finalized;
        }
    }

    public class UIRenderable
    {
        internal Rectangle Bounds;
        internal string Id;
        internal string TextureId;
        internal System.Drawing.Color Color;
        internal float Depth;
        internal object Clip;
        internal string Type;
        internal object Texture;
    }
}

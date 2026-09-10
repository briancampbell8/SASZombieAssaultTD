// =====================================================================================================
//  FILE: Finalizer.cs
//  PATH: Engine/UI/Finalizer.cs
//  SUBSYSTEM: UI / Overlay Validation & Diagnostic Finalization
//
//  ROLE:
//      Optional diagnostic subsystem responsible for validating UI overlay correctness,
//      verifying HUD visibility state, and performing post-render consistency checks.
//      Finalizer performs no rendering and no state management.
//
//  RESPONSIBILITIES:
//      - Validate HUDManager state after HUDRenderer execution.
//      - Validate dynamic overlay correctness after ModernUIRenderer execution.
//      - Provide diagnostic hooks for UI debugging.
//
//  NON-RESPONSIBILITIES:
//      - Rendering (handled by HUDRenderer and ModernUIRenderer).
//      - HUD state management (handled by HUDManager).
//      - Texture loading (handled by TextureManager).
//
//  ARCHITECTURAL NOTES:
//      - Finalizer runs after all render subsystems.
//      - All legacy HUDPanel* classes have been removed.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class UIFinalizer
    {
        //=====================================================================
        //PANEL GEOMETRY (required by HUDConfigManager)
        //=====================================================================

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; } = 0f;

        //=====================================================================
        //CROSSHAIR (required by HUDConfigManager)
        //=====================================================================

        public int CrosshairCenterX { get; set; }
        public int CrosshairCenterY { get; set; }
        public int CrosshairArmLength { get; set; }
        public int CrosshairThickness { get; set; }

        public System.Drawing.Color CrosshairColor { get; set; } = System.Drawing.Color.White;
        public bool UseFlashColor { get; set; }
        public System.Drawing.Color FlashColor { get; set; } = System.Drawing.Color.Red;

        //=====================================================================
        //COLOR PIPELINE (required by HUDColorInputProcessor)
        //=====================================================================

        public System.Drawing.Color FillColor { get; set; } = System.Drawing.Color.Transparent;
        public System.Drawing.Color TextColor { get; set; } = System.Drawing.Color.White;
        public bool ManualColorOverride { get; set; }

        //=====================================================================
        //FINAL OUTPUT TEXTURE (optional)
        //=====================================================================

        public object FinalTexture { get; set; }

        //=====================================================================
        //FINALIZATION PIPELINE
        //=====================================================================

        ///<summary>
        ///Converts UIState → UIRenderable list.
        ///</summary>
        public IReadOnlyList<UIRenderable> FinalizeUI(UIState uiState)
        {
            // Fixed CS1061: Changed UIStateElements to Elements
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIFinalizer.FinalizeUI.Start",
                uiState == null ? "uiState=NULL" : $"Elements={uiState.Elements?.Count}");

            if (uiState == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIFinalizer.FinalizeUI.Error", "uiState is NULL");
                return Array.Empty<UIRenderable>();
            }

            // Fixed CS1061: Changed UIStateElements to Elements
            if (uiState.Elements == null || uiState.Elements.Count == 0)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIFinalizer.FinalizeUI.Empty", "No elements to finalize");
                return Array.Empty<UIRenderable>();
            }

            // Fixed CS1061: Changed UIStateElements to Elements
            var finalized = new List<UIRenderable>(uiState.Elements.Count);

            // Fixed CS1061: Changed UIStateElements to Elements
            foreach (var element in uiState.Elements)
            {
                if (element == null)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIFinalizer.FinalizeUI.Skip.NullElement", "NULL");
                    continue;
                }

                //If already a UIRenderable, pass through
                if ((object)element is UIRenderable renderable)
                {
                    finalized.Add(renderable);
                    continue;
                }

                //Default conversion for non-renderable elements
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

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIFinalizer.FinalizeUI.Complete",
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

// ====================================================================================================
//  FILE: UIFinalizer.cs
//  PATH: ./Engine/UI/
//  MODULE: UI Finalizer Pipeline
//
//  ROLE:
//      Central pipeline manager coordinating UIStateBuilder transformations.
//
//  RESPONSIBILITIES:
//      - Coordinate element updates through the builder chain.
//      - Execute safe interface resolution operations.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using System.Collections.Generic;
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
            DLogger.Log("UIFinalizer.FinalizeUI.Start",
                uiState == null ? "uiState=NULL" : $"Elements={uiState.UIStateElements?.Count}");

            if (uiState == null)
            {
                DLogger.Log("UIFinalizer.FinalizeUI.Error", "uiState is NULL");
                return Array.Empty<UIRenderable>();
            }

            if (uiState.UIStateElements == null || uiState.UIStateElements.Count == 0)
            {
                DLogger.Log("UIFinalizer.FinalizeUI.Empty", "No elements to finalize");
                return Array.Empty<UIRenderable>();
            }

            var finalized = new List<UIRenderable>(uiState.UIStateElements.Count);

            foreach (var element in uiState.UIStateElements)
            {
                if (element == null)
                {
                    DLogger.Log("UIFinalizer.FinalizeUI.Skip.NullElement", "NULL");
                    continue;
                }

                //If already a UIRenderable, pass through
                if (element is UIRenderable renderable)
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

            DLogger.Log("UIFinalizer.FinalizeUI.Complete",
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





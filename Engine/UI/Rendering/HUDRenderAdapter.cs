// ====================================================================================================
//  FILE: HUDRenderAdapter.cs
//  PATH: Engine/UI/Rendering/
//  MODULE: UI Rendering Adapter (P80 Integration)
//
//  ROLE:
//      Adapts legacy HUD rendering calls to the P80 UI rendering backend.
//
//  RESPONSIBILITIES:
//      - Provide DrawTexture, DrawRect, DrawText, and scissor management using P80 UIRenderer.
//      - Maintain minimal diagnostic counters for draw call conversion and batch submission.
//      - Ensure thread-safety for render-target and state transitions.
//
//  NON-RESPONSIBILITIES:
//      - Low-level GPU resource management beyond the LegacyRenderer abstraction.
//
//  ARCHITECTURAL NOTES:
//      - Acts as a bridge during modernization; keep surface compatibility with previous APIs.
// ====================================================================================================

using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// P80 UI renderer: System.Drawing-based, HUDRenderAdapter-compatible.
    /// </summary>
    public class LegacyRenderer
    {
        private readonly Font _defaultFont = new Font("Arial", 12f);

        public void Initialize()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "LegacyRenderer: Initialize");
        }

        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "LegacyRenderer: Shutdown");
        }

        // --------------------------------------------------------------------
        // Rectangles
        // --------------------------------------------------------------------

        public void RenderRectangle(RectangleF rect, Color color)
        {
            DLogger.Log($"LegacyRenderer: RenderRectangle → {rect} Color={color}");
            // TODO: hook into actual GPU backend; for now this is the contract.
        }

        public void RenderRectangle(RectangleF rect, Color color, string textureName)
        {
            DLogger.Log($"LegacyRenderer: RenderRectangle (textured) → {rect} Color={color} Texture={textureName}");
            // TODO: use textureName to select texture in backend.
        }

        // --------------------------------------------------------------------
        // Text
        // --------------------------------------------------------------------

        public void RenderText(string text, PointF position, string fontName, Color color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Font font = _defaultFont;
            DLogger.Log($"LegacyRenderer: RenderText → \"{text}\" at {position} Color={color} Font={font.Name},{font.Size}");
            // TODO: actual text rendering via backend.
        }

        // --------------------------------------------------------------------
        // Clipping
        // --------------------------------------------------------------------

        public void SetClipRect(RectangleF rect)
        {
            DLogger.Log($"LegacyRenderer: SetClipRect → {rect}");
            // TODO: apply clip rect in backend.
        }

        public void ClearClipRect()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "LegacyRenderer: ClearClipRect");
            // TODO: clear clip rect in backend.
        }

        // --------------------------------------------------------------------
        // Frame lifecycle
        // --------------------------------------------------------------------

        public void EndFrame()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "LegacyRenderer: EndFrame");
            // TODO: submit any buffered commands if needed.
        }
    }
}


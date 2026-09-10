// =====================================================================================================
//  FILE: Renderer-Clear.cs
//  PATH: Engine/Render/LegacyRender/Renderer-Clear.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Implements the renderer’s clear operation, providing deterministic behavior for resetting
//      the framebuffer state prior to issuing draw commands. This partial handles validation,
//      logging, and safe fallback behavior when the renderer is not initialized.
//
//  RESPONSIBILITIES:
//      - Perform safe framebuffer clear operations.
//      - Validate renderer initialization state before clearing.
//      - Apply the configured default clear color when none is provided.
//      - Emit deterministic diagnostic logs for all clear operations.
//
//  NON-RESPONSIBILITIES:
//      - Managing GPU resources or issuing draw calls.
//      - Performing initialization or shutdown logic.
//      - Handling viewport configuration or frame lifecycle operations.
//      - Implementing color grading or screenshot behavior.
//
//  ARCHITECTURAL NOTES:
//      - Operational logic is isolated from Renderer-Core.cs and Renderer-Frame.cs.
//      - Clear() must remain safe to call at any stage of the engine lifecycle.
//      - This partial provides legacy-compatible behavior until full modernization occurs.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Clears the framebuffer using the provided color or the renderer’s default clear color.
        /// </summary>
        public void Clear(Color? color = null)
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Warning, "Renderer: Cannot clear - not initialized");
                return;
            }

            try
            {
                var clearColor = color ?? _clearColor;

                // In the legacy pipeline, this is a stub. Modern pipelines clear via D3D11Adapter_Core.
                // This call intentionally performs no GPU operations to maintain compatibility.
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Debug, $"Renderer: Cleared with color {clearColor}");
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Error, $"Renderer: Failed to clear - {ex.Message}");
            }
        }
    }
}

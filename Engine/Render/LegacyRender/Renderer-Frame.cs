// =====================================================================================================
//  FILE: Renderer-Frame.cs
//  PATH: Engine/Render/LegacyRender/Renderer-Frame.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Implements deterministic frame lifecycle operations for the Renderer driver program.
//      This partial provides BeginFrame, EndFrame, and Present — the legacy-compatible hooks
//      that bracket per-frame rendering work and emit diagnostic output.
//
//  RESPONSIBILITIES:
//      - Begin and end the logical frame lifecycle.
//      - Validate initialization state before frame operations.
//      - Emit deterministic diagnostic logs for frame boundaries.
//      - Provide a safe legacy Present() stub until the modern GPU pipeline is active.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU presentation or swapchain operations.
//      - Managing draw calls or render passes.
//      - Handling initialization, viewport configuration, or timing logic.
//      - Implementing color grading, screenshots, or GPU timing.
//
//  ARCHITECTURAL NOTES:
//      - These methods intentionally remain lightweight and non-invasive.
//      - Present() returns a boolean for legacy compatibility; modern pipelines will replace it.
//      - All logging must remain deterministic and safe at any engine stage.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Marks the beginning of a logical frame. Safe to call even if no GPU context is active.
        /// </summary>
        public void BeginFrame()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "Renderer: BeginFrame skipped (not initialized)");
                return;
            }

            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "Renderer: BeginFrame");
        }

        /// <summary>
        /// Marks the end of a logical frame. Completes the legacy frame boundary.
        /// </summary>
        public void EndFrame()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "Renderer: EndFrame skipped (not initialized)");
                return;
            }

            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "Renderer: EndFrame");
        }

        /// <summary>
        /// Legacy-compatible Present() stub. Modern pipelines will replace this with swapchain presentation.
        /// </summary>
        public bool Present()
        {
            if (!_isInitialized)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Warning, "Renderer: Cannot present - not initialized");
                return false;
            }

            try
            {
                // Legacy stub: no GPU presentation occurs here.
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Debug, "Renderer: Present");
                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Error, $"Renderer: Present failed - {ex.Message}");
                return false;
            }
        }
    }
}

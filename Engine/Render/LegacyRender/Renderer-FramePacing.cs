// =====================================================================================================
//  FILE: Renderer-FramePacing.cs
//  PATH: Engine/Render/LegacyRender/Renderer-FramePacing.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Provides deterministic, legacy-compatible frame pacing configuration for the renderer.
//      This partial validates target FPS, updates internal pacing flags, and emits diagnostic logs.
//      No GPU timing or swapchain pacing occurs here — modernization will introduce real pacing.
//
//  RESPONSIBILITIES:
//      - Validate and apply target FPS values.
//      - Update VSync and adaptive VSync configuration flags.
//      - Emit deterministic diagnostic logs for all pacing changes.
//      - Maintain safe, non-invasive behavior compatible with legacy pipelines.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU frame pacing or swapchain throttling.
//      - Managing frame timing, delta time, or tick rate.
//      - Implementing vsync logic at the GPU or OS level.
//      - Handling initialization, frame lifecycle, or presentation.
//
//  ARCHITECTURAL NOTES:
//      - This partial provides a stable configuration surface until the modern timing subsystem
//        (Finalizer + GPU timing) is activated.
//      - All values must remain validated and safe to avoid undefined behavior downstream.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Applies legacy-compatible frame pacing configuration with safe validation and logging.
        /// </summary>
        public void ApplyFramePacing(int targetFPS = 60, bool vsyncEnabled = true, bool adaptiveVSync = false)
        {
            try
            {
                // Clamp FPS to a safe minimum of 1 to avoid divide-by-zero or undefined pacing.
                var validatedFPS = System.Math.Max(1, targetFPS);

                _targetFPS = validatedFPS;
                _vsyncEnabled = vsyncEnabled;
                _adaptiveVSync = adaptiveVSync;

                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Debug,
                    $"Renderer: Frame pacing applied — TargetFPS={validatedFPS}, " +
                    $"VSync={vsyncEnabled}, AdaptiveVSync={adaptiveVSync}"
                );
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Error,
                    $"Renderer: Failed to apply frame pacing — {ex.Message}"
                );
            }
        }
    }
}

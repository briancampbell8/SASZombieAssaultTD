// =====================================================================================================
//  FILE: Renderer-Viewport.cs
//  PATH: Engine/Render/LegacyRender/Renderer-Viewport.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Provides deterministic viewport and render‑scale configuration for the legacy renderer.
//      These APIs update internal viewport state and emit stable diagnostic logs without
//      performing GPU viewport operations.
//
//  RESPONSIBILITIES:
//      - Update viewport dimensions deterministically.
//      - Apply render scale with safe clamping and derived viewport recalculation.
//      - Emit diagnostic logs for all viewport operations.
//      - Maintain compatibility with legacy rendering paths.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU viewport or scissor operations.
//      - Managing swapchains, render passes, or framebuffer resizing.
//      - Handling frame lifecycle or presentation logic.
//
//  ARCHITECTURAL NOTES:
//      - RenderScale is validated and applied deterministically.
//      - Viewport recalculation uses System.Math for strict numeric behavior.
//      - All viewport operations must remain safe at any engine stage.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Sets the logical viewport size. No GPU viewport operations occur here.
        /// </summary>
        public void SetViewport(int width, int height)
        {
            if (!_isInitialized)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Warning,
                    "Renderer: Cannot set viewport — renderer not initialized"
                );
                return;
            }

            try
            {
                _viewportSize = new Vector3(width, height, 0f);

                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Debug,
                    $"Renderer: Viewport set to {width}x{height}"
                );
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Error,
                    $"Renderer: Failed to set viewport — {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Applies render scale and recalculates the logical viewport accordingly.
        /// </summary>
        public void SetRenderScale(float scale)
        {
            // Clamp scale to safe deterministic range
            float validatedScale = System.Math.Max(0.1f, System.Math.Min(8.0f, scale));
            RenderScale = validatedScale;

            int scaledWidth = (int)(System.Math.Max(1.0f, _viewportSize.X * validatedScale));
            int scaledHeight = (int)(System.Math.Max(1.0f, _viewportSize.Y * validatedScale));

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                $"Renderer: RenderScale={validatedScale:F2}, ScaledViewport={scaledWidth}x{scaledHeight}"
            );

            SetViewport(scaledWidth, scaledHeight);
        }
    }
}

// =====================================================================================================
//  FILE: Renderer-Screenshot.cs
//  PATH: Engine/Render/LegacyRender/Renderer-Screenshot.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Provides deterministic, legacy-compatible screenshot capture hooks. These APIs do not perform
//      GPU readback or file encoding; instead they emit stable diagnostic logs and maintain screenshot
//      configuration state.
//
//  RESPONSIBILITIES:
//      - Expose a safe screenshot trigger method.
//      - Maintain and validate screenshot output paths.
//      - Emit deterministic diagnostic logs for all screenshot operations.
//      - Avoid throwing exceptions or performing real GPU capture work.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU framebuffer readback.
//      - Encoding PNG/JPEG/BMP data.
//      - Managing file IO beyond path composition.
//      - Handling frame lifecycle or presentation logic.
//
//  ARCHITECTURAL NOTES:
//      - Screenshot operations remain no-ops until the modern GPU capture pipeline is activated.
//      - All screenshot paths must remain stable and deterministic.
//      - Screenshot calls must be safe at any engine stage.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Triggers a legacy-compatible screenshot capture. No GPU readback occurs; this method
        /// simply logs the intended output path for deterministic behavior.
        /// </summary>
        public bool TakeScreenshot(string? filename = null)
        {
            if (!_isInitialized || !_screenshotEnabled)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Warning,
                    "Renderer: Screenshot skipped — renderer not initialized or screenshot disabled"
                );
                return false;
            }

            try
            {
                string screenshotFile = filename ??
                    $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";

                string fullPath = System.IO.Path.Combine(_screenshotPath, screenshotFile);

                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Info,
                    $"Renderer: Screenshot saved to {fullPath}"
                );

                return true;
            }
            catch (Exception ex)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Error,
                    $"Renderer: Failed to take screenshot — {ex.Message}"
                );
                return false;
            }
        }

        /// <summary>
        /// Sets the screenshot output directory. Path is stored deterministically.
        /// </summary>
        public void SetScreenshotPath(string path)
        {
            _screenshotPath = path ?? "screenshots";

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                $"Renderer: Screenshot path set to {_screenshotPath}"
            );
        }
    }
}

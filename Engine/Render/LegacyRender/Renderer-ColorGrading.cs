// =====================================================================================================
//  FILE: Renderer-ColorGrading.cs
//  PATH: Engine/Render/LegacyRender/Renderer-ColorGrading.cs
//  SUBSYSTEM: Rendering (Legacy Driver Program)
//
//  ROLE:
//      Implements legacy-compatible color grading configuration for the renderer. This partial
//      updates tint, contrast, brightness, and enabled state, providing deterministic behavior
//      and diagnostic logging without performing GPU-side grading operations.
//
//  RESPONSIBILITIES:
//      - Toggle color grading on or off.
//      - Apply tint, contrast, and brightness values with safe clamping.
//      - Maintain deterministic state for downstream rendering subsystems.
//      - Emit diagnostic logs describing all color grading changes.
//
//  NON-RESPONSIBILITIES:
//      - Executing GPU color grading operations.
//      - Managing shaders or post-processing pipelines.
//      - Handling initialization, frame lifecycle, or screenshot logic.
//      - Performing any draw operations.
//
//  ARCHITECTURAL NOTES:
//      - This partial maintains legacy behavior until the modern post-processing pipeline is active.
//      - All GPU operations are deferred to D3D11Adapter_Core in future modernization phases.
//      - Values must remain clamped to safe ranges to avoid undefined behavior in downstream systems.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    public partial class Renderer
    {
        /// <summary>
        /// Applies color grading configuration to the renderer with safe clamping and diagnostic logging.
        /// </summary>
        public void ApplyColorGrading(bool enabled, Color? tint = null, float? contrast = null, float? brightness = null)
        {
            _colorGradingEnabled = enabled;

            if (tint.HasValue)
                _colorGradeTint = tint.Value;

            if (contrast.HasValue)
                _colorGradeContrast = System.MathF.Max(0.0f, System.MathF.Min(2.0f, contrast.Value));

            if (brightness.HasValue)
                _colorGradeBrightness = System.MathF.Max(-1.0f, System.MathF.Min(1.0f, brightness.Value));

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                $"Renderer: Applied color grading - Enabled={enabled}, Tint={_colorGradeTint}, " +
                $"Contrast={_colorGradeContrast:F2}, Brightness={_colorGradeBrightness:F2}"
            );
        }
    }
}

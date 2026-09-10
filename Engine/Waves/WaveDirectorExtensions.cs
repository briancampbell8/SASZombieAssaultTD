// =====================================================================================================
//  FILE: WaveDirectorExtensions.cs
//  PATH: Engine/Waves/WaveDirectorExtensions.cs
//  SUBSYSTEM: Waves
//
//  ROLE:
//      Provides deterministic, type‑safe extension methods for WaveDirector to support wave scheduling,
//      pacing adjustments, spawn timing helpers, and lightweight orchestration utilities.
//
//  RESPONSIBILITIES:
//      - Extend WaveDirector with helper methods for computing spawn intervals and pacing curves.
//      - Provide deterministic wave‑math utilities (e.g., scaling, clamping, interval shaping).
//      - Support readability and maintainability of wave‑control logic without modifying core systems.
//      - Remain pure: no side effects outside WaveDirector parameter computation.
//
//  NON-RESPONSIBILITIES:
//      - Managing enemy entities, spawn pools, or gameplay logic directly.
//      - Performing frame‑level update sequencing or rendering operations.
//      - Allocating or mutating engine subsystems outside WaveDirector’s domain.
//      - Replacing or overriding WaveDirector’s core deterministic behavior.
//
//  ARCHITECTURAL NOTES:
//      - WaveDirector is intentionally minimal; extensions provide convenience without expanding subsystem
//        responsibilities.
//      - All extension methods must remain stateless and side‑effect free.
//      - Relocated from Engine/Extensions during subsystem cleanup.
// =====================================================================================================
namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Deterministic helper extensions for WaveDirector.
    /// </summary>
    internal static class WaveDirectorExtensions
    {
        /// <summary>
        /// Scales a wave’s spawn interval by a deterministic multiplier.
        /// </summary>
        public static float ScaleInterval(this float interval, float multiplier)
        {
            if (multiplier <= 0f)
                return interval;

            return interval * multiplier;
        }

        /// <summary>
        /// Clamps a wave’s spawn interval to a minimum threshold.
        /// </summary>
        public static float ClampMinimum(this float interval, float minimum)
        {
            return interval < minimum ? minimum : interval;
        }

        /// <summary>
        /// Applies a pacing curve to a wave interval (simple exponential decay).
        /// </summary>
        public static float ApplyPacingCurve(this float interval, float factor)
        {
            if (factor <= 0f)
                return interval;

            return interval / (1f + factor);
        }

        /// <summary>
        /// Computes a deterministic spawn delay based on wave index.
        /// </summary>
        public static float ComputeWaveDelay(this int waveIndex, float baseDelay)
        {
            if (waveIndex < 0)
                return baseDelay;

            return baseDelay * (1f - (waveIndex * 0.05f));
        }
    }
}

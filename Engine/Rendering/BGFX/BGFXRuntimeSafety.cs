//
// * File:    BGFXRuntimeSafety.cs
// * Path:    Engine/Rendering/BGFX/BGFXRuntimeSafety.cs
// * Purpose: BGFX runtime safety layer - structural guardrails for BGFX runtime behavior.
// //

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    /// <summary>
    /// BGFX runtime safety layer - structural guardrails for BGFX runtime behavior.
    /// This class contains only boolean flags for runtime safety configuration.
    /// </summary>
    internal class BGFXRuntimeSafety
    {
        /// <summary>
        /// Suppress BGFX errors during startup.
        /// </summary>
        public bool SuppressErrors;

        /// <summary>
        /// Suppress BGFX warnings during startup.
        /// </summary>
        public bool SuppressWarnings;

        /// <summary>
        /// Enable debug validation layer.
        /// </summary>
        public bool EnableDebugValidation;

        /// <summary>
        /// Enable GPU capture tools.
        /// </summary>
        public bool EnableGpuCapture;

        /// <summary>
        /// Enable BGFX stats/debug UI.
        /// </summary>
        public bool EnableStats;
    }
}

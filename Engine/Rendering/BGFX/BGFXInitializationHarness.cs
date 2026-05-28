//
// * File:    BGFXInitializationHarness.cs
// * Path:    Engine/Rendering/BGFX/BGFXInitializationHarness.cs
// * Purpose: BGFX initialization harness - simulated initialization state tracking.
// //

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    /// <summary>
    /// BGFX initialization harness - simulated initialization state tracking.
    /// This class contains only boolean flags for initialization step tracking.
    /// </summary>
    internal class BGFXInitializationHarness
    {
        /// <summary>
        /// Whether BGFX initialization has been called.
        /// </summary>
        public bool InitCalled;

        /// <summary>
        /// Whether BGFX reset has been called.
        /// </summary>
        public bool ResetCalled;

        /// <summary>
        /// Whether BGFX views have been configured.
        /// </summary>
        public bool ViewsConfigured;

        /// <summary>
        /// Whether BGFX resources have been created.
        /// </summary>
        public bool ResourcesCreated;

        /// <summary>
        /// Whether BGFX shutdown has been called.
        /// </summary>
        public bool ShutdownCalled;

        // TODO: Add frame tracking for future phases
        // - FrameCount: Track number of frames submitted
        // - LastFrameTime: Track timing for performance analysis
        // - FrameErrors: Track frame submission failures
    }
}

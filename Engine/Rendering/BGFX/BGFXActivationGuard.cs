//
// * File:    BGFXActivationGuard.cs
// * Path:    Engine/Rendering/BGFX/BGFXActivationGuard.cs
// * Purpose: BGFX activation boundary - safety rails for BGFX API calls.
// //

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    /// <summary>
    /// BGFX activation boundary - safety rails for BGFX API calls.
    /// This class contains only boolean flags for activation state management.
    /// </summary>
    internal class BGFXActivationGuard
    {
        /// <summary>
        /// BGFX is fully initialized and ready for API calls.
        /// </summary>
        public bool IsReady;

        /// <summary>
        /// Initialization is in progress.
        /// </summary>
        public bool IsInitializing;

        /// <summary>
        /// Shutdown is in progress.
        /// </summary>
        public bool IsShuttingDown;
    }
}

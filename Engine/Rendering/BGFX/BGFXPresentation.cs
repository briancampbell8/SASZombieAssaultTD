//
// * File:    BGFXPresentation.cs
// * Path:    Engine/Rendering/BGFX/BGFXPresentation.cs
// * Purpose: BGFX swap chain presentation.
// *          Parallel implementation to BGFX for comparison testing.
// //
using SASZombieAssaultTD.Engine.Diagnostics;

using System;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    /// <summary>
    /// BGFX presentation and frame management.
    /// Handles the final step of showing content on screen.
    /// </summary>
    internal sealed class BGFXPresentation : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the BGFXPresentation class using the specified BGFX device core.  private BGFXDeviceCore _core;
        /// </summary>
        /// <remarks>This constructor does not perform a null check on the core parameter due to the current
        /// BGFX stub implementation. In future implementations, a null check may be reinstated to ensure proper exception
        /// handling.</remarks>
        /// <param name="core">The BGFXDeviceCore instance to associate with this presentation. May be null in stub implementations.</param>

        // FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************
        // public BGFXPresentation(BGFXDeviceCore core)
        // {
        //     _core = core ?? throw new ArgumentNullException(nameof(core));
        // }
        // FUTURE IMPLEMENTATION: exception handling removed during BGFX bypass *************************************

        // public BGFXPresentation(BGFXDeviceCore core)
        // {
        //           _core = core; // Null check bypassed for BGFX stub implementation
        //     }

        public void PresentFramebuffer()
        {
            // BGFX present placeholder
            System.Diagnostics.Debug.WriteLine("[BGFX] BGFXPresentation.PresentFramebuffer()");
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("[BGFX] BGFXPresentation.Dispose()");
        }
    }
}

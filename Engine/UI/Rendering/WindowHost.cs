// =====================================================================================================
//  FILE: WindowHost.cs
//  PATH: Engine/UI/Rendering/WindowHost.cs
//  SUBSYSTEM: UI Rendering
//
//  ROLE: Logical window host that owns the primary framebuffer and exposes it as an D3D11Adapter_Core
//        to the engine. Platform-agnostic; concrete platform windows sit beneath this type.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Interfaces;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Logical window host that owns the primary framebuffer and exposes it as an <see cref="D3D11Adapter_Core"/>.
    /// Platform-specific window implementations are expected to coordinate with this host but are defined elsewhere.
    /// </summary>
    public sealed class WindowHost : IDisposable
    {
        /// <summary>
        /// Logical width of the window in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Logical height of the window in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Logical window title used by platform-specific hosts.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Primary framebuffer used as the engine's render target. Also serves as the concrete implementation of
        /// <see cref="D3D11Adapter_Core"/>.
        /// </summary>
        private FramebufferDrawing? _framebuffer;

        /// <summary>
        /// Cached render context reference (same instance as <see cref="_framebuffer"/> ). Exposed to higher-level
        /// systems such as RenderManager.
        /// </summary>


        private bool _disposed;

        /// <summary>
        /// Creates a new logical window host with the specified dimensions and title.
        /// </summary>
        /// <param name="width">Logical width in pixels.</param>
        /// <param name="height">Logical height in pixels.</param>
        /// <param name="title">Logical window title.</param>
        public WindowHost(int width, int height, string title)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            Title = string.IsNullOrWhiteSpace(title) ? "SAS Zombie Assault TD" : title;

            DLogger.Log($"[WindowHost] Constructed ({Width}x{Height}, Title='{Title}').");
        }

        /// <summary>
        /// Creates (or returns an existing) render context for this window host. The returned instance is the primary
        /// framebuffer used by the engine.
        /// </summary>
        /// <returns>The active <see cref="D3D11Adapter_Core"/> for this window host.</returns>

        /// <summary>
        /// Processes window or platform events. On Win32 this is typically driven by a lower-level window class; here
        /// it remains a no-op placeholder so non-Win32 backends can integrate their own event loops if needed.
        /// </summary>
        public void PumpEvents()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowHost));

            // Intentionally empty: platform-specific hosts (e.g., Win32Window) are responsible
            // for actual OS message pumping. WindowHost remains platform-agnostic.
        }

        /// <summary>
        /// Releases resources owned by this host. Does not own OS window handles; only managed render resources and
        /// references.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if ((object?)_framebuffer is IDisposable disposableFramebuffer)
            {
                disposableFramebuffer.Dispose();
            }

        }
    }
}

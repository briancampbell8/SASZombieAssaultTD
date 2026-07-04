/* ====================================================================================================
 *  FILE:       WindowHost.cs
 *  PATH:       Engine/Rendering/
 *  SUBSYSTEM:  Rendering
 *  ROLE:       Logical window host that owns the primary framebuffer and exposes it as an IRenderContext
 *              to the engine. Platform-agnostic; concrete platform windows sit beneath this type.
 *
 *  RESPONSIBILITIES:
 *      - Maintain logical window dimensions and title.
 *      - Own the primary Framebuffer instance for the engine.
 *      - Expose the framebuffer as an IRenderContext to higher-level systems.
 *      - Provide a stable abstraction for platform-specific window/message pumping.
 *
 *  NON-RESPONSIBILITIES:
 *      - Creating OS windows or handling OS messages (delegated to platform-specific hosts).
 *      - Game logic, ECS operations, or simulation.
 *      - Render scheduling (delegated to RenderManager).
 *      - GPU device or swap chain management (delegated to D3D11DeviceCore or other backends).
 *
 *  DEPENDENCIES:
 *      - Framebuffer (CPU-side deterministic render target).
 *      - IRenderContext (render context contract).
 *      - DebugLogger (diagnostics).
 *
 *  CALLED BY:
 *      - GameRoot / engine bootstrap code.
 *      - Platform-specific window host (for lifecycle coordination).
 *
 *  CALLS INTO:
 *      - Framebuffer constructor.
 *      - DebugLogger for lifecycle diagnostics.
 *
 *  ARCHITECTURAL NOTES:
 *      - WindowHost is a logical/engine-level construct, not a platform window.
 *      - Framebuffer is the concrete IRenderContext implementation returned to the engine.
 *      - This type must remain free of platform-specific APIs and OS handles.
 *      - This file is complete and must not be split into partials.
 * ==================================================================================================== */

using System;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Logical window host that owns the primary framebuffer and exposes it as an <see cref="IRenderContext"/>.
    ///Platform-specific window implementations are expected to coordinate with this host but are defined elsewhere.
    ///</summary>
    public sealed class WindowHost : IDisposable
    {
        ///<summary>
        ///Logical width of the window in pixels.
        ///</summary>
        public int Width { get; }

        ///<summary>
        ///Logical height of the window in pixels.
        ///</summary>
        public int Height { get; }

        ///<summary>
        ///Logical window title used by platform-specific hosts.
        ///</summary>
        public string Title { get; }

        ///<summary>
        ///Primary framebuffer used as the engine's render target.
        ///Also serves as the concrete implementation of <see cref="IRenderContext"/>.
        ///</summary>
        private Framebuffer? _framebuffer;

        ///<summary>
        ///Cached render context reference (same instance as <see cref="_framebuffer"/>).
        ///Exposed to higher-level systems such as RenderManager.
        ///</summary>
        private IRenderContext? _renderContext;

        private bool _disposed;

        ///<summary>
        ///Creates a new logical window host with the specified dimensions and title.
        ///</summary>
        ///<param name="width">Logical width in pixels.</param>
        ///<param name="height">Logical height in pixels.</param>
        ///<param name="title">Logical window title.</param>
        public WindowHost(int width, int height, string title)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            Title = string.IsNullOrWhiteSpace(title) ? "SAS Zombie Assault TD" : title;

            DLogger.Log($"[WindowHost] Constructed ({Width}x{Height}, Title='{Title}').");
        }

        ///<summary>
        ///Creates (or returns an existing) render context for this window host.
        ///The returned instance is the primary framebuffer used by the engine.
        ///</summary>
        ///<returns>The active <see cref="IRenderContext"/> for this window host.</returns>
        public IRenderContext CreateRenderContext()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowHost));

            if (_renderContext != null)
                return _renderContext;

            //Framebuffer is the concrete IRenderContext implementation.
            _framebuffer = new Framebuffer(Width, Height);
            _renderContext = _framebuffer;

            DLogger.Log($"[WindowHost] Render context created ({Width}x{Height}).");

            return _renderContext;
        }

        ///<summary>
        ///Processes window or platform events.
        ///On Win32 this is typically driven by a lower-level window class; here it remains a no-op
        ///placeholder so non-Win32 backends can integrate their own event loops if needed.
        ///</summary>
        public void PumpEvents()
        {
            //Intentionally empty: platform-specific hosts (e.g., Win32Window) are responsible
            //for actual OS message pumping. WindowHost remains platform-agnostic.
        }

        ///<summary>
        ///Releases resources owned by this host.
        ///Does not own OS window handles; only managed render resources and references.
        ///</summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _framebuffer = null;
            _renderContext = null;

            _disposed = true;

            DLogger.Log(LogSubsystems.Rendering, LogLevel.Info, "[WindowHost] Disposed.");
        }
    }
}

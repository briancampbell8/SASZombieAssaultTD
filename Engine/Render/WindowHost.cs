// =====================================================================================================
//  FILE: WindowHost.cs
//  PATH: Engine/Rendering/
//  SUBSYSTEM: Rendering / Logical Window Descriptor
//
//  ROLE:
//      Pure logical window host. Provides deterministic window dimensions and title for the engine.
//      Does NOT create or expose any render context. Does NOT own a framebuffer. Does NOT interact
//      with GPU or CPU rendering paths.
//
//  RESPONSIBILITIES:
//      - Store logical window width, height, and title.
//      - Provide a stable abstraction for platform-specific window hosts (Win32Window, SDLWindow, etc.).
//      - Allow PumpEvents() for platform integration (no-op here).
//
//  NON-RESPONSIBILITIES:
//      - Rendering or GPU upload.
//      - Framebuffer creation or management.
//      - Swap chain or device ownership.
//      - D3D11Adapter_Core exposure.
//      - Any legacy CPU rendering path.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    /// <summary>
    /// Logical window descriptor used by the engine. Platform-specific window implementations
    /// (e.g., Win32Window) own the actual OS handles, swap chain, and GPU device.
    /// WindowHost provides only deterministic window metadata.
    /// </summary>
    public sealed class WindowHost : IDisposable
    {
        /// <summary>Logical width of the window in pixels.</summary>
        public int Width { get; }

        /// <summary>Logical height of the window in pixels.</summary>
        public int Height { get; }

        /// <summary>Logical window title.</summary>
        public string Title { get; }

        private bool _disposed;

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
        /// Platform event pump. No-op for logical window host; platform-specific windows override this.
        /// </summary>
        public void PumpEvents()
        {
            // Intentionally empty.
        }

        /// <summary>
        /// Releases logical window resources. No GPU or framebuffer resources exist here.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DLogger.Log(LogSubsystems.Rendering, LogLevel.Info, "[WindowHost] Disposed.");
        }
    }
}

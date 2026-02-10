using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class WindowHost : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public string Title { get; }

        private Framebuffer? _framebuffer;
        private bool _disposed;

        public WindowHost(int width, int height, string title)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            Title = title ?? "SAS Zombie Assault TD";
        }

        public IRenderContext CreateRenderContext()
        {
            // Return a Framebuffer-backed render context matching this host's dimensions.
            // The engine's IRenderContext is implemented by Framebuffer.
            _framebuffer = new Framebuffer(Width, Height);

            DebugLogger.Log("Info",
                $"[WindowHost] Created render context ({Width}x{Height}).");

            return _framebuffer;
        }

        public void PumpEvents()
        {
            // WindowHost is the abstract/portable host layer.
            // Actual message pumping is handled by Win32Window.PumpMessages().
            // This method exists so non-Win32 backends can integrate their own
            // event loops. Currently a no-op on Win32.
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            // Release the framebuffer render context
            _framebuffer = null;
            _disposed = true;

            DebugLogger.Log("Info",
                "[WindowHost] Disposed.");
        }
    }
}

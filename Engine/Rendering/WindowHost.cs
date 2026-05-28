using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class WindowHost : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public string Title { get; }

        private Framebuffer? _framebuffer;
        private bool _disposed;
        private int windowWidth;
        private int windowHeight;
        private IRenderContext _renderContext;

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
                    
            _framebuffer = new Framebuffer();
            _framebuffer.Width = windowWidth;   // Use your actual width variable name here
            _framebuffer.Height = windowHeight; // Use your actual height variable name here

            
            _renderContext = (IRenderContext)_framebuffer;
            return null;

          
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

            Engine.Diagnostics.DebugLogger.LogDebug("Info",
            "[WindowHost] Disposed.");
        }
    }
}




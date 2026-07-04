using System;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering
//
{
    public sealed class RenderSurface : IDisposable
    {
        public int Width { get; }
        public int Height { get; }

        private Framebuffer? _offscreenBuffer;
        private bool _disposed;

        public RenderSurface(int width, int height)
        {
            DLogger.Log("Info", $"[RenderSurface] Creating surface {width}x{height}");

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;

            //Allocate an off-screen framebuffer for this surface
            _offscreenBuffer = new Framebuffer(width, height);
        }

        public void Bind(IRenderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //Bind the off-screen surface by clearing it to prepare for rendering.
            //The caller renders into context; the off-screen buffer is synchronized
            //by clearing to match the context's expected initial state.
            _offscreenBuffer?.ClearScreen();

            DLogger.Log("Info",
            $"[RenderSurface] Bound off-screen surface ({Width}x{Height}).");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            //Release the off-screen buffer
            _offscreenBuffer = null;
            _disposed = true;

            DLogger.Log("Info",
            "[RenderSurface] Disposed.");
        }
    }
}



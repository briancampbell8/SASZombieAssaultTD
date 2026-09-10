// =====================================================================================================
//  FILE: SoftwareRenderTarget.cs
//  PATH: Engine/Graphics/Software/SoftwareRenderTarget.cs
//  SUBSYSTEM: Software Rendering Backend
//
//  ROLE:
//      CPU-side render target used by the software renderer and as a deterministic fallback
//      for backends that do not yet implement GPU render target creation.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Interfaces;
using Vortice.DCommon;
using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Graphics.Software
{
    public sealed class SoftwareRenderTarget : IRenderTarget
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public PixelFormat Format { get; }

        public string Name => throw new NotImplementedException();

        public RenderTargetUsage Usage => throw new NotImplementedException();

        string IRenderTarget.Usage => throw new NotImplementedException();

        // Removed 'readonly' modifier so Array.Resize can swap the buffer pointer
        private int[] _pixels;
        private UI.Rendering.PixelFormat format;

        public SoftwareRenderTarget(int width, int height, PixelFormat format)
        {
            Width = System.Math.Max(1, width);
            Height = System.Math.Max(1, height);
            Format = format;

            _pixels = new int[Width * Height];
        }

        public SoftwareRenderTarget(int width, int height, UI.Rendering.PixelFormat format)
        {
            Width = width;
            Height = height;
            this.format = format;
        }

        public void Clear(int argb)
        {
            for (int i = 0; i < _pixels.Length; i++)
                _pixels[i] = argb;
        }

        public void Resize(int width, int height)
        {
            Width = System.Math.Max(1, width);
            Height = System.Math.Max(1, height);

            // This now compiles cleanly because _pixels is no longer readonly
            Array.Resize(
                array: ref _pixels,
                newSize: Width * Height);
        }

        public void SetPixel(int x, int y, int argb)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;

            _pixels[y * Width + x] = argb;
        }

        public int GetPixel(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return 0;

            return _pixels[y * Width + x];
        }

        public void Dispose()
        {

            // Free managed resources
            _pixels = null;



        }
    }
}

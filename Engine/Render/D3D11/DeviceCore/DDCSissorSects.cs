// =====================================================================================================
//  FILE: DDC_ScissorRects.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_ScissorRects.cs
//  SUBSYSTEM: D3D11 Backend – Scissor Rectangle Program
//
//  ROLE:
//      Owns deterministic creation, binding, and disposal of the D3D11 scissor rectangle(s).
//
//  RESPONSIBILITIES:
//      - Create scissor rectangle(s) for the current swap-chain size.
//      - Bind scissor rectangle(s) to the D3D11 device context.
//      - Recreate scissor rectangle(s) on resize.
//      - Expose scissor rectangle(s) to higher-level GPU programs.
//
//  NON-RESPONSIBILITIES:
//      - Viewport management.
//      - Rasterizer, blend, or depth-stencil state.
//      - Device/context lifetime management.
//      - Presentation.
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - Deterministic Option-B formatting.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    public class DDCScissorRects
    {
        // FIX 1 & 2: Added 'public' to fix accessibility errors CS0051 & CS0053
        // FIX 3: Ensured fields use raw, unmanaged ints to satisfy the struct constraint for CS8377
        public struct RawRectangle
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        public DDCScissorRects(RawRectangle[] scissorRects)
        {
            _scissorRects = scissorRects;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_ScissorRects: created.");

            _disposed = false;
        }
        private bool _disposed;

        private int _width;
        private int _height;
        private RawRectangle[] _scissorRects;
        private ID3D11DeviceContext _context;

        public ReadOnlySpan<RawRectangle> ScissorRects => _scissorRects;



        public DDCScissorRects(
            ID3D11DeviceContext context,
            int width,
            int height,
            ReadOnlySpan<RawRectangle> scissorRects)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Scissor rectangle dimensions must be positive.");

            _width = width;
            _height = height;

            CreateScissorRects(_width, _height);
            _scissorRects = scissorRects.ToArray();
        }

        private void CreateScissorRects(int width, int height)
        {
            _scissorRects = new[]
            {
                new RawRectangle
                {
                    Left = 0,
                    Top = 0,
                    Right = width,
                    Bottom = height
                }
            };

            DLogger.Log($"DDC_ScissorRects: scissor rectangle created ({width}x{height}).");
        }

        public void Recreate(int width, int height)
        {
            if (_disposed)
                return;

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Scissor rectangle dimensions must be positive.");

            if (width == _width && height == _height)
                return;

            _width = width;
            _height = height;

            DLogger.Log($"DDC_ScissorRects: recreating scissor rectangle {_width}x{_height}.");

            CreateScissorRects(_width, _height);
        }

        public void Bind()
        {
            if (_disposed)
                return;

            if (_scissorRects == null || _scissorRects.Length == 0)
                throw new InvalidOperationException(
                    "DDC_ScissorRects: scissor rectangle array is null or empty.");

            _context.RSSetScissorRects(_scissorRects);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _scissorRects = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_ScissorRects: disposed.");
        }


    }
}

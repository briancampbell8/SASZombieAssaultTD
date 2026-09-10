// =====================================================================================================
//  FILE: DDC_ViewPorts.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_ViewPorts.cs
//  SUBSYSTEM: D3D11 Backend – Viewport Program
//
//  ROLE:
//      Owns deterministic creation, binding, and disposal of the D3D11 viewport(s).
//
//  RESPONSIBILITIES:
//      - Create viewport(s) for the current swap-chain size.
//      - Bind viewport(s) to the D3D11 device context.
//      - Recreate viewport(s) on resize.
//      - Expose viewport(s) to higher-level GPU programs.
//
//  NON-RESPONSIBILITIES:
//      - Scissor rectangles.
//      - Rasterizer state.
//      - Blend or depth-stencil state.
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
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    internal sealed class DDC_ViewPorts : IDisposable
    {
        private readonly ID3D11DeviceContext _context;

        private Viewport[] _viewports;
        private bool _disposed;

        private int _width;
        private int _height;

        public ReadOnlySpan<Viewport> Viewports => _viewports;

        public DDC_ViewPorts(
            ID3D11DeviceContext context,
            int width,
            int height)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Viewport dimensions must be positive.");

            _width = width;
            _height = height;

            CreateViewports(_width, _height);
        }

        private void CreateViewports(int width, int height)
        {
            _viewports = new[]
            {
                new Viewport(
                    x: 0.0f,
                    y: 0.0f,
                    width: width,
                    height: height,
                    minDepth: 0.0f,
                    maxDepth: 1.0f)
            };

            DLogger.Log($"DDC_ViewPorts: viewport created ({width}x{height}).");
        }

        public void Recreate(int width, int height)
        {
            if (_disposed)
                return;

            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Viewport dimensions must be positive.");

            if (width == _width && height == _height)
                return;

            _width = width;
            _height = height;

            DLogger.Log($"DDC_ViewPorts: recreating viewport {_width}x{_height}.");

            CreateViewports(_width, _height);
        }

        public void Bind()
        {
            if (_disposed)
                return;

            if (_viewports == null || _viewports.Length == 0)
                throw new InvalidOperationException("DDC_ViewPorts: viewport array is null or empty.");

            _context.RSSetViewports(_viewports);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _viewports = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "DDC_ViewPorts: disposed.");
        }
    }
}

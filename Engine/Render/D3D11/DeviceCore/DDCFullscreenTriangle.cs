// =====================================================================================================
//  FILE: DDCFullscreenTriangle.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDCFullscreenTriangle.cs
//  SUBSYSTEM: D3D11 Backend – Fullscreen Triangle Pipeline
//
//  ROLE:
//      Owns the deterministic fullscreen‑triangle rendering pipeline.
//      Compiles HLSL source files at runtime via Vortice and maintains GPU programs.
//
//  RESPONSIBILITIES:
//      - Read, compile, and maintain fullscreen pipeline HLSL source files at runtime.
//      - Bind pipeline state and draw a fullscreen textured triangle via SV_VertexID.
//      - Dispose all GPU resources deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Device/context/swap chain creation (DDC).
//      - Backbuffer/RTV creation (Core_Backbuffer, Core_RTV).
//      - Presentation (D3D11Presentation).
//      - Render‑target binding or clearing (Core_RenderTargets).
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - Deterministic Option‑B formatting.
//
//  CODE CHANGES:
//      - Fixed parameter alignment order inside Vortice Compiler.Compile calls to resolve X3506 error.
// =====================================================================================================

using System;
using System.IO;
using SASZombieAssaultTD.Engine.Diagnostics;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore
{
    public sealed class DDCFullscreenTriangle : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;

        private ID3D11VertexShader _vs;
        private ID3D11PixelShader _ps;
        private bool _disposed;

        public DDCFullscreenTriangle(ID3D11Device device, ID3D11DeviceContext context)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            CreatePipeline();
        }

        private void CreatePipeline()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var vsSourcePath = Path.Combine(baseDir, "FullscreenQuadVS.hlsl");
            var psSourcePath = Path.Combine(baseDir, "FullscreenQuadPS.hlsl");

            if (!File.Exists(vsSourcePath))
                throw new FileNotFoundException("Vertex shader source file missing from build directory.", vsSourcePath);
            if (!File.Exists(psSourcePath))
                throw new FileNotFoundException("Pixel shader source file missing from build directory.", psSourcePath);

            string vsSourceText = File.ReadAllText(vsSourcePath);
            string psSourceText = File.ReadAllText(psSourcePath);

            try
            {
                // Correct signature: Compile(shaderSource, entryPoint, sourceName, profile)
                ReadOnlyMemory<byte> vsBytecode = Compiler.Compile(vsSourceText, "main", "FullscreenQuadVS.hlsl", "vs_5_0");
                _vs = _device.CreateVertexShader(vsBytecode.Span);

                ReadOnlyMemory<byte> psBytecode = Compiler.Compile(psSourceText, "main", "FullscreenQuadPS.hlsl", "ps_5_0");
                _ps = _device.CreatePixelShader(psBytecode.Span);
            }
            catch (Exception ex)
            {
                DLogger.Log($"[HLSL RUNTIME COMPILE EXCEPTION] {ex.Message}");
                throw;
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_FullscreenQuad: pipeline compiled dynamically at runtime (Zero-Buffer Config).");
        }

        public void Draw(ID3D11ShaderResourceView textureView)
        {
            if (_disposed)
                return;

            _context.IASetInputLayout(null);
            _context.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);
            _context.IASetVertexBuffers(0, 0, null, null, null);

            _context.VSSetShader(_vs);
            _context.PSSetShader(_ps);
            _context.PSSetShaderResource(0, textureView);

            _context.Draw(3, 0);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _vs?.Dispose();
            _ps?.Dispose();

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_FullscreenQuad: disposed.");
        }
    }
}

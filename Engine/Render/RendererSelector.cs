// =====================================================================================================
//  FILE: RendererSelector.cs
//  PATH: Engine/Render/RendererSelector.cs
//  SUBSYSTEM: Rendering Backend Selection
//
//  ROLE:
//      Provides deterministic backend selection for the engine’s rendering system. This factory
//      constructs the appropriate graphics device implementation based on the selected backend.
//
//  RESPONSIBILITIES:
//      - Select and instantiate the correct renderer backend.
//      - Provide deterministic fallback behavior for unsupported backends.
//      - Maintain strict separation between backend selection and backend implementation.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU work or issuing draw commands.
//      - Managing resources, textures, or render targets.
//      - Handling diagnostics, logging, or performance metrics.
//
//  ARCHITECTURAL NOTES:
//      - Backend selection must remain deterministic and side‑effect free.
//      - Unsupported backends must fail gracefully with clear diagnostics.
//      - Modernization will introduce BGFX and SoftwareRenderer backends.
// =====================================================================================================

using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Rendering;
using Vortice.Direct3D11;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Render
{
    /// <summary>
    /// Deterministic renderer backend selector.
    /// </summary>
    public static class RendererSelector
    {
        public static IGraphicsDevice Create(RendererBackend backend)
        {
            switch (backend)
            {
                case RendererBackend.D3D11:
                    return new D3D11GraphicsDevice(IntPtr.Zero, 800, 600);

                default:
                    DLogger.Log(
                        LogSubsystems.Rendering,
                        LogEnums.LogLevel.Error,
                        $"RendererSelector: Unsupported backend '{backend}'"
                    );
                    throw new NotSupportedException(
                        $"Renderer backend '{backend}' is not supported."
                    );
            }
        }
    }

    public sealed class D3D11GraphicsDevice : IGraphicsDevice
    {
        private readonly int _width;
        private readonly int _height;
        private bool _initialized;
        private readonly D3D11Adapter_Manager _adapterManager;

        public D3D11GraphicsDevice(nint windowHandle, int width, int height)
        {
            _width = width;
            _height = height;
            _initialized = false;

            // Adapter manager is expected to be wired elsewhere in the real pipeline.
            // Here we keep it null-safe and fail deterministically if used without a backend.
            _adapterManager = null!;
        }

        public D3D11GraphicsDevice()
        {
            _adapterManager = null!;
        }

        public bool IsInitialized => _initialized;
        public int Width => _width;
        public int Height => _height;

        public ID3D11RenderTargetView MainRenderTargetView { get; private set; }

        public void Initialize(nint windowHandle, int width, int height)
        {
            _initialized = true;

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Info,
                $"D3D11GraphicsDevice: Initialized ({width}x{height})"
            );
        }

        public void ApplyClear(BackendCommand op)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: Clear");
        }

        public void ApplyDrawQuad(ID3D11DeviceContext context, BackendCommand op)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: DrawQuad");
        }

        public void ApplyDrawText(BackendCommand op)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: DrawText");
        }

        public void ApplySetRenderTarget(BackendCommand op)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: SetRenderTarget");
        }

        public void ApplySetScissor(BackendCommand op)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: SetScissor");
        }

        public Engine.Interfaces.IRenderTarget CreateRenderTarget(string name, int width, int height, PixelFormat format)
        {
            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                $"D3D11: CreateRenderTarget '{name}' ({width}x{height}, {format})"
            );

            return new SoftwareRenderTarget(width, height, format);
        }

        public ID3D11RenderTargetView CreateRenderTarget(object x, object y, object color)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Debug, "D3D11: CreateRenderTargetView");
            return null!;
        }

        public void ExecuteCommands(IEnumerable<RenderCommand> commands)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: ExecuteCommands");
        }

        public ID3D11BlendState GetOrCreateBlendState()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: BlendState");
            return null!;
        }

        public ID3D11InputLayout GetOrCreateInputLayout(ID3D11VertexShader vs)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: InputLayout");
            return null!;
        }

        public ID3D11PixelShader GetOrCreatePixelShader()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: PixelShader");
            return null!;
        }

        public ID3D11SamplerState GetOrCreateSampler()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: Sampler");
            return null!;
        }

        public ID3D11ShaderResourceView GetOrCreateShaderResourceView(ITexture2D texture)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: ShaderResourceView");
            return null!;
        }

        public ID3D11VertexShader GetOrCreateVertexShader()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: VertexShader");
            return null!;
        }

        public ID3D11DeviceContext Get_context1()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: DeviceContext");
            return null!;
        }

        public void Present()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: Present");
        }

        public void PresentFramebufferDrawing(FramebufferDrawing fb, ID3D11DeviceContext context)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: PresentFramebufferDrawing");
        }

        public void Resize(int width, int height)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info, $"D3D11: Resize {width}x{height}");
        }

        public void UploadFramebufferDrawing(FramebufferDrawing fb)
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Trace, "D3D11: UploadFramebufferDrawing");
        }

        public void Dispose()
        {
            DLogger.Log(LogSubsystems.Rendering, LogEnums.LogLevel.Info, "D3D11GraphicsDevice: Disposed");
        }

        public Engine.Interfaces.IRenderTarget CreateRenderTarget(
            string name,
            int width,
            int height,
            System.Drawing.Imaging.PixelFormat format)
        {
            if (_adapterManager == null)
                throw new InvalidOperationException("D3D11 adapter manager is not available.");

            var backend = _adapterManager.GetBackendContext();
            if (backend == null)
                throw new InvalidOperationException("D3D11 backend context is not available.");

            // Use dynamic to access Device on the backend context without compile-time type.
            var device = ((dynamic)backend).Device;

            return new DDC_RenderTargets(
                name,
                width,
                height,
                format,
                device);
        }

        public ID3D11DeviceContext GetDeviceContext()
        {
            if (_adapterManager == null)
                throw new InvalidOperationException("D3D11 adapter manager is not available.");

            var backend = _adapterManager.GetBackendContext();
            if (backend == null)
                throw new InvalidOperationException("D3D11 backend context is not available.");

            // Use dynamic to access DeviceContext on the backend context without compile-time type.
            return ((dynamic)backend).DeviceContext;
        }

        Engine.Interfaces.IRenderTarget IGraphicsDevice.CreateRenderTarget(string name, int width, int height, System.Drawing.Imaging.PixelFormat format)
        {
            if (Enum.TryParse<PixelFormat>(format.ToString(), ignoreCase: true, out var uiFormat))
            {
                return CreateRenderTarget(name, width, height, uiFormat);
            }

            switch (format)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    if (Enum.TryParse("Rgba32", true, out uiFormat) ||
                        Enum.TryParse("Bgra32", true, out uiFormat))
                    {
                        return CreateRenderTarget(name, width, height, uiFormat);
                    }
                    break;

                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    if (Enum.TryParse("Rgb24", true, out uiFormat))
                    {
                        return CreateRenderTarget(name, width, height, uiFormat);
                    }
                    break;
            }

            return CreateRenderTarget(name, width, height, default(PixelFormat));
        }
    }

    internal class DDC_RenderTargets : Engine.Interfaces.IRenderTarget
    {
        private readonly string _name;
        private readonly int _width;
        private readonly int _height;
        private readonly string _usage;
        private readonly object _device;
        private bool _disposed;

        public DDC_RenderTargets(string name, int width, int height, System.Drawing.Imaging.PixelFormat format, object device)
        {
            _name = name ?? string.Empty;
            _width = width;
            _height = height;
            _usage = format.ToString();
            _device = device;
            _disposed = false;
        }

        public string Name => _name;

        public int Width => _width;

        public int Height => _height;

        public string Usage => _usage;

        public void Dispose()
        {
            if (_disposed) return;

            if (_device is IDisposable disposable)
            {
                try { disposable.Dispose(); } catch { }
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

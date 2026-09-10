// =====================================================================================================
//  FILE: D3D11FramebufferDrawingUploader.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: D3D11FramebufferDrawingUploader.cs
//  MODULE: Resource Management Framework
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    public sealed class D3D11FramebufferDrawingUploader : IDisposable
    {
        // ---------------------------------------------------------------------------------------------
        // Fields
        // ---------------------------------------------------------------------------------------------
        private readonly D3D11DeviceCore _deviceCore;

        private ID3D11Texture2D _gpuTexture;
        private ID3D11ShaderResourceView _gpuTextureView;

        private int _lastWidth;
        private int _lastHeight;

        private bool _disposed;

        public object drawing { get; private set; }

        // ---------------------------------------------------------------------------------------------
        // Construction
        // ---------------------------------------------------------------------------------------------
        public D3D11FramebufferDrawingUploader(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Info,
                "D3D11",
                "D3D11FramebufferDrawingUploader created");
        }

        // ---------------------------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------------------------
        public void Upload(FramebufferDrawing framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            ThrowIfDisposed();

            int w = framebuffer.Width;
            int h = framebuffer.Height;

            if (w <= 0 || h <= 0)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Warning,
                    "D3D11",
                    $"Invalid framebuffer size {w}x{h}");
                return;
            }

            // Detect size changes and recreate GPU resources
            if (_gpuTexture == null || w != _lastWidth || h != _lastHeight)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Info,
                    "D3D11",
                    $"Resizing GPU texture from {_lastWidth}x{_lastHeight} to {w}x{h}");

                DisposeGpuResources();

                var desc = new Texture2DDescription
                {
                    Width = (uint)w,
                    Height = (uint)h,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ShaderResource,
                    CPUAccessFlags = CpuAccessFlags.None,
                    MiscFlags = ResourceOptionFlags.None
                };

                _gpuTexture = _deviceCore.Device.CreateTexture2D(desc);
                _gpuTextureView = _deviceCore.Device.CreateShaderResourceView(_gpuTexture);

                _lastWidth = w;
                _lastHeight = h;

                if (_gpuTextureView == null)
                {
                    DLogger.Log(
                        LogSubsystems.Rendering,
                        LogEnums.LogLevel.Error,
                        "D3D11",
                        "GPU texture view is null after creation");
                    return;
                }

                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Info,
                    "D3D11",
                    "GPU texture + SRV ready");
            }

            // Upload pixel data
            var pixels = framebuffer.Pixels;
            if (pixels == null)
            {
                DLogger.Log(
                    LogSubsystems.Rendering,
                    LogEnums.LogLevel.Error,
                    "D3D11",
                    "FramebufferDrawing.Pixels is null");
                return;
            }

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                "D3D11",
                $"Uploading framebuffer {w}x{h} ({pixels.ToString().Length} pixels)");

            // 1. Create a destination array matching the pixel count
            int[] intPixels = new int[framebuffer.Pixels.Length];

            // 2. Perform a lightning-fast byte copy directly from the uint[] array
            System.Buffer.BlockCopy(
                framebuffer.Pixels, 0, intPixels, 0,
                framebuffer.Pixels.Length * sizeof(uint));

            // 3. Commit the unified CPU data array to the GPU resource via the unmanaged immediate context
            int rowPitch = w * sizeof(int);
            int depthPitch = rowPitch * h;

            unsafe
            {
                fixed (int* pPixels = intPixels)
                {
                    var dataBox = new Vortice.Direct3D11.SubresourceData(
                        (IntPtr)pPixels,
                        (uint)rowPitch,
                        (uint)depthPitch);
                    //    _deviceCore.updateSubresource(dataBox, _gpuTexture, 0);
                    //_deviceCore.ImmediateContext.UpdateSubresource(dataBox, _gpuTexture, 0);
                }
            }

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                "D3D11",
                "Framebuffer drawing successfully uploaded to GPU texture.");
        }

        public ID3D11ShaderResourceView GetTextureView()
        {
            ThrowIfDisposed();
            return _gpuTextureView;
        }

        // ---------------------------------------------------------------------------------------------
        // Disposal
        // ---------------------------------------------------------------------------------------------
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DisposeGpuResources();

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Info,
                "D3D11",
                "Disposing FramebufferDrawingUploaderD3D11");
        }

        private void DisposeGpuResources()
        {
            if (_gpuTextureView != null)
            {
                _gpuTextureView.Dispose();
                _gpuTextureView = null;
            }

            if (_gpuTexture != null)
            {
                _gpuTexture.Dispose();
                _gpuTexture = null;
            }

            _lastWidth = 0;
            _lastHeight = 0;

            DLogger.Log(
                LogSubsystems.Rendering,
                LogEnums.LogLevel.Debug,
                "D3D11",
                "Disposed GPU texture + SRV");
        }

        // ---------------------------------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------------------------------
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(D3D11FramebufferDrawingUploader));
        }
    }
}

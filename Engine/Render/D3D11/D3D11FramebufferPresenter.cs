// ====================================================================================================
//  FILE: D3D11FramebufferDrawingPresenter.cs
//  PATH: Engine/Render/D3D11
//  SUBSYSTEM: Rendering.D3D11
// ====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Graphics.Software;
using Vortice.Direct3D11;
using Vortice.DXGI;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    public sealed class D3D11FramebufferDrawingPresenter : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;

        private ID3D11Texture2D _framebufferTexture;
        private ID3D11ShaderResourceView _framebufferSrv;

        private int _width;
        private int _height;
        private bool _disposed;

        public ID3D11ShaderResourceView ShaderResourceView => _framebufferSrv;

        public D3D11FramebufferDrawingPresenter(
            ID3D11Device device,
            ID3D11DeviceContext context)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            DLogger.Log(LogSubsystems.D3D11, "D3D11FramebufferDrawingPresenter: constructed.");
        }

        public void EnsureSize(FramebufferDrawing framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            if (framebuffer.Width <= 0 || framebuffer.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(framebuffer), "FramebufferDrawing dimensions must be positive.");

            if (_framebufferTexture != null &&
                framebuffer.Width == _width &&
                framebuffer.Height == _height)
            {
                return;
            }

            DisposeTextureResources();

            _width = framebuffer.Width;
            _height = framebuffer.Height;

            DLogger.Log(LogSubsystems.D3D11,
                $"D3D11FramebufferDrawingPresenter: creating texture {_width}x{_height}.");

            var desc = new Texture2DDescription
            {
                Width = (uint)_width,
                Height = (uint)_height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            _framebufferTexture = _device.CreateTexture2D(desc);
            _framebufferSrv = _device.CreateShaderResourceView(_framebufferTexture);

            DLogger.Log(LogSubsystems.D3D11,
                "D3D11FramebufferDrawingPresenter: texture and SRV created.");
        }

        public void Upload(FramebufferDrawing framebuffer)
        {
            if (framebuffer == null)
                throw new ArgumentNullException(nameof(framebuffer));

            if (_disposed)
                throw new ObjectDisposedException(nameof(D3D11FramebufferDrawingPresenter));

            if (_framebufferTexture == null || _framebufferSrv == null)
            {
                EnsureSize(framebuffer);
            }

            if (framebuffer.Pixels == null || framebuffer.Pixels.Length == 0)
            {
                DLogger.Log(LogSubsystems.D3D11,
                    "D3D11FramebufferDrawingPresenter.Upload: framebuffer has no pixel data.");
                return;
            }

            if (framebuffer.Width != _width || framebuffer.Height != _height)
            {
                EnsureSize(framebuffer);
            }

            int rowPitch = _width * 4;
            int depthPitch = rowPitch * _height;

            unsafe
            {
                // Pin the byte[] and pass its address as an IntPtr to UpdateSubresource.
                // This avoids an invalid implicit conversion from byte[] to uint*.
                fixed (byte* pPixels = framebuffer.Pixels)
                {
                    var dataBox = new SubresourceData((IntPtr)pPixels, (uint)rowPitch, (uint)depthPitch);
                    _context.UpdateSubresource(dataBox, _framebufferTexture, 0);
                }
            }

            DLogger.Log(LogSubsystems.D3D11,
                "D3D11FramebufferDrawingPresenter.Upload: framebuffer data uploaded to GPU.");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            DisposeTextureResources();

            DLogger.Log(LogSubsystems.D3D11,
                "D3D11FramebufferDrawingPresenter: disposed.");
        }

        private void DisposeTextureResources()
        {
            _framebufferSrv?.Dispose();
            _framebufferSrv = null;

            _framebufferTexture?.Dispose();
            _framebufferTexture = null;

            _width = 0;
            _height = 0;
        }
    }
}

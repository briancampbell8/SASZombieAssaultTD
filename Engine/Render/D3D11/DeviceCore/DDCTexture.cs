// =====================================================================================================
//  FILE: DDC_Texture.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_Texture.cs
//  SUBSYSTEM: D3D11 Backend – Texture & Shader Resource View Program
//
//  ROLE:
//      Owns deterministic creation and disposal of 2D textures and their corresponding SRVs.
//      Provides a clean, standalone GPU program for texture upload and SRV creation.
//
//  RESPONSIBILITIES:
//      - Create ID3D11Texture2D from raw pixel data.
//      - Create ID3D11ShaderResourceView for sampling in pixel shaders.
//      - Expose texture + SRV to higher‑level GPU programs.
//      - Dispose resources deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Sampler creation (Core_Sampler).
//      - Quad rendering (Core_FullscreenQuad, Core_Quad).
//      - Backbuffer/RTV creation.
//      - Presentation.
//      - Device/context creation.
//
//  ARCHITECTURAL NOTES:
//      - Standalone GPU program.
//      - No partials.
//      - No block comments.
//      - Deterministic Option‑B formatting.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore_
{
    internal sealed class DDC_Texture : IDisposable
    {
        private readonly ID3D11Device _device;

        private ID3D11Texture2D _texture;
        private ID3D11ShaderResourceView _srv;
        private bool _disposed;

        public ID3D11Texture2D Texture => _texture;
        public ID3D11ShaderResourceView ShaderResourceView => _srv;

        public DDC_Texture(ID3D11Device device, int width, int height, byte[] pixelData)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            if (pixelData == null || pixelData.Length == 0)
                throw new ArgumentNullException(nameof(pixelData));

            CreateTexture(width, height, pixelData);
        }

        private unsafe void CreateTexture(int width, int height, byte[] pixelData)
        {
            _texture?.Dispose();
            _srv?.Dispose();

            // FIXED CS0266, CS0117: Cast dimensions to uint, mapped CPUAccessFlags and MiscFlags properties
            var texDesc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Immutable,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            // FIXED CS1503: Pin pixelData block memory to securely obtain unmanaged nint pointer address
            fixed (byte* pData = pixelData)
            {
                var dataBox = new SubresourceData((IntPtr)pData, (uint)(width * 4));

                _texture = _device.CreateTexture2D(texDesc, new[] { dataBox });
            }

            if (_texture == null)
                throw new InvalidOperationException("Core_Texture: failed to create texture.");

            _srv = _device.CreateShaderResourceView(_texture);

            if (_srv == null)
                throw new InvalidOperationException("Core_Texture: failed to create SRV.");

            DLogger.Log($"Core_Texture: texture created ({width}x{height}).");
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _srv?.Dispose();
            _srv = null;

            _texture?.Dispose();
            _texture = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_Texture: disposed.");
        }
    }
}

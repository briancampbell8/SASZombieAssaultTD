//==========================================================================================
// FILE: DGDOps_Samplers.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Samplers.cs
// SUBSYSTEM: DGD / Sampler State Management
//
// ROLE:
//     Provides deterministic creation of ID3D11SamplerState objects.
//     Supplies common sampler configurations for textures, sprites, UI, and text.
//
// RESPONSIBILITIES:
//     - Create linear, point, and anisotropic samplers.
//     - Wrap D3D11DeviceCore.Device for sampler creation.
//     - Expose reusable sampler instances for DGDOps subsystems.
//
// NON-RESPONSIBILITIES:
//     - Binding samplers to the pipeline.
//     - Managing shader resource views.
//     - Creating textures or render targets.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;
using Vortice.Mathematics;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic sampler-state creation subsystem.
    /// </summary>
    public sealed class DGDOps_Samplers
    {
        private readonly D3D11DeviceCore _deviceCore;

        private ID3D11SamplerState _linearSampler;
        private ID3D11SamplerState _pointSampler;
        private ID3D11SamplerState _anisotropicSampler;

        public DGDOps_Samplers(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        // -------------------------------------------------------------------------
        //  PUBLIC: Linear Sampler (Min/Mag/Mip Linear)
        // -------------------------------------------------------------------------
        public ID3D11SamplerState GetLinearSampler()
        {
            if (_linearSampler != null)
                return _linearSampler;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new SamplerDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                BorderColor = new Color4(0f, 0f, 0f, 0f),
                ComparisonFunc = ComparisonFunction.Never,
                MinLOD = 0f,
                MaxLOD = float.MaxValue,
                MipLODBias = 0f,
                MaxAnisotropy = 1
            };

            _linearSampler = device.CreateSamplerState(desc);
            return _linearSampler;
        }

        // -------------------------------------------------------------------------
        //  PUBLIC: Point Sampler (Nearest-neighbor)
        // -------------------------------------------------------------------------
        public ID3D11SamplerState GetPointSampler()
        {
            if (_pointSampler != null)
                return _pointSampler;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new SamplerDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                BorderColor = new Color4(0f, 0f, 0f, 0f),
                ComparisonFunc = ComparisonFunction.Never,
                MinLOD = 0f,
                MaxLOD = float.MaxValue,
                MipLODBias = 0f,
                MaxAnisotropy = 1
            };

            _pointSampler = device.CreateSamplerState(desc);
            return _pointSampler;
        }

        // -------------------------------------------------------------------------
        //  PUBLIC: Anisotropic Sampler (High-quality texture sampling)
        // -------------------------------------------------------------------------
        public ID3D11SamplerState GetAnisotropicSampler()
        {
            if (_anisotropicSampler != null)
                return _anisotropicSampler;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            var desc = new SamplerDescription
            {
                Filter = Filter.Anisotropic,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = new Color4(0f, 0f, 0f, 0f),
                ComparisonFunc = ComparisonFunction.Never,
                MinLOD = 0f,
                MaxLOD = float.MaxValue,
                MipLODBias = 0f,
                MaxAnisotropy = 16
            };

            _anisotropicSampler = device.CreateSamplerState(desc);
            return _anisotropicSampler;
        }
    }
}

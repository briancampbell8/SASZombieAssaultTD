// =====================================================================================================
//  FILE: DDC_Sampler.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_Sampler.cs
//  SUBSYSTEM: D3D11 Backend – Sampler State Program
//
//  ROLE:
//      Owns deterministic creation and binding of the global sampler state used by all textured
//      rendering operations.
//
//  RESPONSIBILITIES:
//      - Create a single ID3D11SamplerState instance.
//      - Bind the sampler to the pixel‑shader stage.
//      - Dispose the sampler deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Texture creation (Core_Texture).
//      - Quad rendering (Core_FullscreenQuad, Core_Quad).
//      - RTV/backbuffer creation.
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

namespace SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore_
{
    internal sealed class DDC_Sampler : IDisposable
    {
        private readonly ID3D11Device _device;
        private readonly ID3D11DeviceContext _context;

        private ID3D11SamplerState _sampler;
        private bool _disposed;

        public ID3D11SamplerState Sampler => _sampler;

        public DDC_Sampler(ID3D11Device device, ID3D11DeviceContext context)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _context = context ?? throw new ArgumentNullException(nameof(context));

            CreateSampler();
        }

        private void CreateSampler()
        {
            var desc = new SamplerDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                ComparisonFunc = ComparisonFunction.Never,
                MinLOD = 0.0f,
                MaxLOD = float.MaxValue
            };

            _sampler = _device.CreateSamplerState(desc);

            if (_sampler == null)
                throw new InvalidOperationException("Core_Sampler: failed to create sampler state.");

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_Sampler: sampler created.");
        }

        public void Bind(int slot = 0)
        {
            if (_disposed)
                return;

            _context.PSSetSampler((uint)slot, _sampler);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _sampler?.Dispose();
            _sampler = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_Sampler: disposed.");
        }
    }
}

// ============================================================================
// File:    D3D11DeviceCoreSampler.cs
// Path:    Engine/Rendering/D3D11/D3D11DeviceCoreSampler.cs
// Author:  BDC
// Purpose: Linear clamp sampler creation + binding for D3D11DeviceCore.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using System;
using System.Security.AccessControl;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Rendering.D3D11
{
    public sealed partial class D3D11DeviceCore : IDisposable
    {
        // --------------------------------------------------------------------
        // Fields
        // --------------------------------------------------------------------

        private ID3D11SamplerState _sampler;
        private object TheType;
        private object TheMember;

        // --------------------------------------------------------------------
        // Sampler Initialization
        // --------------------------------------------------------------------

        /// <summary>
        /// Creates the linear clamp sampler and binds it to PS slot 0.
        /// Call once during device initialization.
        /// </summary>
        public void InitializeSampler()
        {
            DebugLogger.LogInfo("D3D11DeviceCore: creating linear clamp sampler");

            var desc = new SamplerDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp
            };

            _sampler = Device.CreateSamplerState(desc);

            if (_sampler == null)
            {
                DebugLogger.LogError("D3D11DeviceCore: sampler creation failed");
                return;
            }

            ImmediateContext.PSSetSampler(0, _sampler);

            DebugLogger.LogInfo("D3D11DeviceCore: sampler created + bound to PS slot 0");
        }

        internal void UpdateTexture(ID3D11Texture2D gpuTexture, byte[] pixels, int w, int h)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        // --------------------------------------------------------------------
        // Disposal
        // --------------------------------------------------------------------

        private void DisposeSampler()
        {
            _sampler?.Dispose();
            _sampler = null;

            DebugLogger.LogInfo("D3D11DeviceCore: sampler disposed");
        }
    }
}

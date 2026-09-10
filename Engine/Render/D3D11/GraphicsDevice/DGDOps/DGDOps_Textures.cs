//==========================================================================================
// FILE: DGDOps_Textures.cs
// PATH: Engine/Render/D3D11/GraphicsDevice/DGDOps/DGDOps_Textures.cs
// SUBSYSTEM: DGD / Texture & Shader Resource View Management
//
// ROLE:
//     Provides deterministic creation of textures and shader resource views (SRVs).
//     Bridges engine texture abstractions to D3D11 resources.
//
// RESPONSIBILITIES:
//     - Create SRVs for engine ITexture2D instances.
//     - Wrap D3D11DeviceCore.Device for texture/SRV creation.
//     - Expose SRVs for use by shaders and drawing subsystems.
//
// NON-RESPONSIBILITIES:
//     - Managing render targets.
//     - Uploading framebuffer data.
//     - Binding SRVs to the pipeline.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using Vortice.Direct3D11;

namespace SASZombieAssaultTD.Engine.Render.D3D11.GraphicsDevice.DGDOps
{
    /// <summary>
    /// Deterministic texture and SRV subsystem.
    /// </summary>
    public sealed class DGDOps_Textures
    {
        private readonly D3D11DeviceCore _deviceCore;

        public DGDOps_Textures(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
        }

        /// <summary>
        /// Get or create a shader resource view for the given texture.
        /// </summary>
        public ID3D11ShaderResourceView GetOrCreateShaderResourceView(ITexture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            // If the texture already exposes an SRV, return it.
            var existing = texture.ShaderResourceView;
            if (existing != null)
                return existing;

            var device = _deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11 device is not initialized.");

            // If the texture can expose a native D3D11 resource, use it.
            var native = texture.NativeTexture;
            if (native is ID3D11Texture2D tex2D)
            {
                return device.CreateShaderResourceView(tex2D);
            }

            // If no native texture is available, we cannot create an SRV here.
            // Caller must handle a null return.
            return null;
        }
    }
}

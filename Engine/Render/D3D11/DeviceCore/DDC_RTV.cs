// =====================================================================================================
//  FILE: DDC_RTV.cs
//  PATH: Engine/Render/D3D11/DeviceCore/DDC_RTV.cs
//  SUBSYSTEM: D3D11 Backend – Render Target View Owner
//
//  ROLE:
//      Owns deterministic creation and disposal of the render‑target view (RTV) for the swap‑chain
//      backbuffer. This program is responsible ONLY for creating the RTV from the backbuffer.
//
//  RESPONSIBILITIES:
//      - Create ID3D11RenderTargetView from the backbuffer texture.
//      - Expose the RTV to higher‑level GPU programs.
//      - Dispose and recreate the RTV deterministically.
//
//  NON-RESPONSIBILITIES:
//      - Backbuffer creation (Core_Backbuffer).
//      - Binding or clearing render targets (Core_RenderTargets).
//      - Swap‑chain resizing (Core_Resize).
//      - Presentation (D3D11Presentation).
//      - Device/context creation (DDC).
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
    internal sealed class DDC_RTV : IDisposable
    {
        private readonly ID3D11Device _device;
        private ID3D11RenderTargetView _rtv;
        private bool _disposed;

        public ID3D11RenderTargetView RenderTargetView => _rtv;

        public DDC_RTV(ID3D11Device device, ID3D11Texture2D backbuffer)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));

            if (backbuffer == null)
                throw new ArgumentNullException(nameof(backbuffer));

            CreateRtv(backbuffer);
        }

        private void CreateRtv(ID3D11Texture2D backbuffer)
        {
            _rtv?.Dispose();
            _rtv = _device.CreateRenderTargetView(backbuffer);

            if (_rtv == null)
                throw new InvalidOperationException("Core_RTV: failed to create render target view.");

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_RTV: RTV created.");
        }

        public void Recreate(ID3D11Texture2D backbuffer)
        {
            if (_disposed)
                return;

            if (backbuffer == null)
                throw new ArgumentNullException(nameof(backbuffer));

            CreateRtv(backbuffer);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _rtv?.Dispose();
            _rtv = null;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "Core_RTV: disposed.");
        }
    }
}

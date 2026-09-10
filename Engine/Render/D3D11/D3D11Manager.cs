// =====================================================================================================
//  FILE: D3D11Manager.cs
//  PATH: Engine/Render/D3D11/D3D11Manager.cs
//  SUBSYSTEM: D3D11 Rendering Backend – Orchestrator
//
//  ROLE:
//      Deterministic top-level orchestrator for the D3D11 rendering backend.
//      Coordinates initialization, per-frame sequencing, pipeline application,
//      render-pass execution, and presentation.
//
//  RESPONSIBILITIES:
//      Initialize → RenderFrame → Shutdown lifecycle.
//      Bind and coordinate standalone D3D11 programs.
//      Enforce deterministic ordering of backend operations.
//      Delegate GPU work to Core_* programs.
//      Provide a clean engine-facing API.
//
//  NON-RESPONSIBILITIES:
//      Creating or owning GPU objects.
//      Performing draw calls.
//      Managing gameplay or UI logic.
//      Making pipeline-state decisions.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Graphics.Software;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    public sealed class D3D11Manager
    {
        private readonly CoreRenderTargets _renderTargets;
        private readonly Core_FullscreenQuad _fullscreenQuad;
        private readonly D3D11FramebufferDrawingPresenter _presenter;
        private readonly D3D11Presentation _presentation;

        public bool IsInitialized { get; private set; }

        public D3D11Manager(
            CoreRenderTargets renderTargets,
            Core_FullscreenQuad fullscreenQuad,
            D3D11FramebufferDrawingPresenter presenter,
            D3D11Presentation presentation)
        {
            _renderTargets = renderTargets ?? throw new ArgumentNullException(nameof(renderTargets));
            _fullscreenQuad = fullscreenQuad ?? throw new ArgumentNullException(nameof(fullscreenQuad));
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _presentation = presentation ?? throw new ArgumentNullException(nameof(presentation));
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            _fullscreenQuad.Initialize();
            IsInitialized = true;
        }

        public void RenderFrame(FramebufferDrawing framebuffer)
        {
            if (!IsInitialized)
                return;

            _renderTargets.Bind();
            _renderTargets.Clear(0f, 0f, 0f, 1f);

            _presenter.EnsureSize(framebuffer);
            _presenter.Upload(framebuffer);

            var srv = _presenter.ShaderResourceView;
            if (srv != null)
                _fullscreenQuad.Draw(srv);

            _presentation.Present();
        }

        public void Shutdown()
        {
            if (!IsInitialized)
                return;

            _fullscreenQuad.Dispose();

            _presenter.Dispose();

            IsInitialized = false;
        }
    }
}

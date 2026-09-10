//==========================================================================================
// FILE: AdapterManager_FrameLifecycle.cs
// PATH: Engine/Render/Adapter/AdapterManager_FrameLifecycle.cs
// SUBSYSTEM: Rendering / D3D11 Frame Lifecycle Management
//
// ROLE:
// Provides deterministic frame lifecycle sequencing including begin, clear,
// end, and present operations. Delegates GPU work to AdapterOps_Frame and
// swap-chain subsystems.
//
// RESPONSIBILITIES:
//  - BeginFrame routing.
//  - Clear operations via FrameOps.
//  - EndFrame routing.
//  - Present routing via SwapChainOps.
//  - Maintain stable frame sequencing.
//
// NON-RESPONSIBILITIES:
//  - Initialization workflow.
//  - Info aggregation.
//  - GPU operations or rendering logic.
//==========================================================================================

using System;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter.AdapterOps
{
    public sealed class AdapterManager_FrameLifecycle
    {
        private readonly D3D11Adapter_Manager _manager;

        public AdapterManager_FrameLifecycle(D3D11Adapter_Manager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public void BeginFrame()
        {
            // Deterministic frame begin
            _manager.FrameOps?.BeginFrame();

            // Deterministic clear
            _manager.FrameOps?.ClearFrameBuffers();
        }

        public void EndFrame()
        {
            // Deterministic frame end
            _manager.FrameOps?.EndFrame();
        }

        public void Present()
        {
            // Present via frame subsystem
            _manager.FrameOps?.PresentFrame();
        }
    }
}

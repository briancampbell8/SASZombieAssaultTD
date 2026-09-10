// =====================================================================================================
//  FILE: D3D11Adapter_Frame.cs
//  PATH: Engine/Render/D3D11/Adapter/D3D11Adapter_Frame.cs
//  SUBSYSTEM: Rendering / Frame Adapter
//
//  ROLE:
//      Deterministic frame-lifecycle adapter for the D3D11 rendering pipeline. Provides a minimal,
//      diagnostic-enabled boundary for BeginFrame/EndFrame sequencing without owning GPU resources.
//
//  RESPONSIBILITIES:
//      - Expose a strict, minimal frame lifecycle surface (Initialize → BeginFrame → EndFrame → Shutdown).
//      - Emit diagnostic markers for frame entry/exit to aid trace analysis.
//      - Remain agnostic of device, swap-chain, and render commands.
//
//  NON-RESPONSIBILITIES:
//      - Creating or managing the D3D11 device or swap chain.
//      - Executing render commands or update logic.
//      - Managing engine subsystems, assets, or game state.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Interfaces;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter
{
    /// <summary>
    /// Deterministic frame adapter for the D3D11 rendering pipeline. Provides a minimal, diagnostic
    /// frame lifecycle surface without owning GPU resources or performing rendering work directly.
    /// </summary>
    public sealed class D3D11AdapterFrame : ID3D11Subsystem
    {
        private bool _frameActive;

        // -------------------------------------------------------------------------------------------------
        // INITIALIZE
        // -------------------------------------------------------------------------------------------------
        public void Initialize()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline,
                "[D3D11AdapterFrame] Initialize: ENTER");

            _frameActive = false;

            DLogger.Log(LogSubsystems.ResourcesPipeline,
                "[D3D11AdapterFrame] Initialize: Completed deterministic frame adapter startup.");
        }

        // -------------------------------------------------------------------------------------------------
        // BEGIN FRAME
        // -------------------------------------------------------------------------------------------------
        public void BeginFrame()
        {
            if (_frameActive)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline,
                    "[D3D11AdapterFrame] BeginFrame: Frame already active.");
                return;
            }

            _frameActive = true;

            DLogger.Log(LogSubsystems.ResourcesPipeline,
                "[D3D11AdapterFrame] BeginFrame: ENTER");
        }

        // -------------------------------------------------------------------------------------------------
        // END FRAME
        // -------------------------------------------------------------------------------------------------
        public void EndFrame()
        {
            if (!_frameActive)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline,
                    "[D3D11AdapterFrame] EndFrame: No active frame.");
                return;
            }

            DLogger.Log(LogSubsystems.ResourcesPipeline,
                "[D3D11AdapterFrame] EndFrame: EXIT");

            _frameActive = false;
        }

        // -------------------------------------------------------------------------------------------------
        // SHUTDOWN
        // -------------------------------------------------------------------------------------------------
        public void Shutdown()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline,
                "[D3D11AdapterFrame] Shutdown: ENTER");

            _frameActive = false;

            DLogger.Log(LogSubsystems.ResourcesPipeline,
                "[D3D11AdapterFrame] Shutdown: Completed deterministic frame adapter shutdown.");
        }

        // -------------------------------------------------------------------------------------------------
        // RESET (OPTIONAL)
        // -------------------------------------------------------------------------------------------------
        public void Reset()
        {
            _frameActive = false;
        }
    }
}

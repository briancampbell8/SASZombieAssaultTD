/*===========================================================================================================
    File: RenderPhase.cs
    Project: SASZombieAssaultTD – Engine Modernization (Deterministic Render Pipeline)
    Author: BDC
    Created: 2026-07-18
    Description:
        RenderPhase defines the deterministic sequencing stages of the engine’s rendering pipeline.
        Each phase represents a strict temporal boundary within a single frame, ensuring that all
        rendering operations occur in a predictable, ordered, and fully deterministic manner.

        The phases are intentionally minimal and tightly scoped:
            • BeginFrame – Initial setup operations, clearing buffers, establishing viewport state.
            • Draw       – All subsystem draw operations (UI, World, HUD, Debug) occur here.
            • EndFrame   – Finalizer operations including swap-chain Present() and post-draw cleanup.

        RenderPhase is used in conjunction with RenderMode and RenderFeatures to determine which
        rendering capabilities are permitted at any given moment. This prevents subsystems from
        executing operations outside their designated frame window, eliminating pipeline drift and
        ensuring strict determinism.

    Engine Determinism Notes:
        • RenderPhase acts as a secondary switch/case routing key inside RenderContextD3D11.
        • BeginFrame is the only phase allowed to perform buffer clears.
        • Draw is the only phase allowed to issue geometry or texture draw calls.
        • EndFrame is the only phase allowed to perform Present() operations.
        • RenderPhase must remain globally accessible and isolated in its own file.

    Dependencies:
        • Used by: RenderCapabilities.cs, RenderContextD3D11.cs
        • Namespace: SASZombieAssaultTD.Engine.Render

    Revision History:
        • 2026-07-18 – Initial deterministic version created by BDC.
===========================================================================================================*/

namespace SASZombieAssaultTD.Engine.Render
{

}

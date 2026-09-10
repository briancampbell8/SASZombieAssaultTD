// =====================================================================================================
//  FILE: D3D11Adapter_RenderBridge.cs
//  PATH: Engine/Render/Adapter/D3D11Adapter_RenderBridge.cs
//  SUBSYSTEM: Render Adapter
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, execution entry, and deterministic shutdown operations.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface for program orchestration.
//      - Enforce the structural sequencing contract: Initialize → Run Loop Execution → Shutdown.
//      - Serve as the base contract for any future top-level engine-hosted program modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing deep frame-level update calculation rules or rendering commands directly.
//      - Managing active systems registration pools, engine assets, or game states.
//      - Handling discrete hardware device allocation boundaries.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces the legacy GameRoot partial lifecycle methods.
//      - GameRootMain implements this interface and delegates to its subsystems:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.UI.Rendering;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter
{
    public sealed class D3D11Adapter_RenderBridge
    {
        internal ID3D11Adapter Adapter { get; }
        internal D3D11Adapter_Core RenderContext { get; }
        internal IDrawingContext DrawingContext { get; }

        internal D3D11Adapter_RenderBridge(
            ID3D11Adapter adapter,
            D3D11Adapter_Core renderContext,
            IDrawingContext drawingContext)
        {
            Adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            RenderContext = renderContext ?? throw new ArgumentNullException(nameof(renderContext));
            DrawingContext = drawingContext ?? throw new ArgumentNullException(nameof(drawingContext));
        }

        internal (ID3D11Adapter, D3D11Adapter_Core, IDrawingContext) GetInterfaces()
        {
            return (Adapter, RenderContext, DrawingContext);
        }
    }
}

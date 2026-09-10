// =====================================================================================================
//  FILE: IProgramLifecycle.cs
//  PATH: Engine/Platform/IProgramLifecycle.cs
//  SUBSYSTEM: Platform Abstraction Layer
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program.
//      This interface is implemented by engine hosts (e.g., GameRootMain) to provide a clean,
//      engine-facing API for startup, per-frame updates, rendering, and shutdown.
//
//  RESPONSIBILITIES:
//      - Provide a strict, minimal lifecycle surface.
//      - Enforce the canonical startup → update → render → shutdown sequence.
//      - Serve as the base contract for any future engine-hosted program.
//
//  NON-RESPONSIBILITIES:
//      - Implementing gameplay logic.
//      - Managing systems, assets, or state machines.
//      - Rendering or updating subsystems directly.
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

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface IProgramLifecycle
    {
        void Startup();
        void Update(float deltaTime);
        void Render();
        void Shutdown();
    }
}

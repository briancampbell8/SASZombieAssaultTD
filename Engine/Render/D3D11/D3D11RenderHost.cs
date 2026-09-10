// =====================================================================================================
//  FILE: D3D11RenderHost.cs
//  PATH: Engine/Render/D3D11/D3D11RenderHost.cs
//  SUBSYSTEM: Engine / Render Host
//
//  ROLE:
//      Defines the minimal deterministic lifecycle contract for any engine-hosted program. Implemented
//      by GameRootMain to provide a clean, engine-facing boundary for initialization, execution, update,
//      render dispatch, ticking, and shutdown.
//
//  RESPONSIBILITIES:
//      - Provide a strict lifecycle surface: Initialize → Run → Update → Render → Shutdown.
//      - Allow engine hosts (e.g., GameRootMain) to expose deterministic lifecycle entry points.
//      - Serve as the base contract for any future top-level engine program modules.
//      - Support both GPU-context rendering and generic object-based render forwarding.
//
//  NON-RESPONSIBILITIES:
//      - Implementing update or render logic internally (delegated to subsystems).
//      - Managing system registration, asset loading, or state-machine orchestration.
//      - Handling GPU device creation, swap-chain management, or windowing.
//
//  ARCHITECTURAL NOTES:
//      - This interface replaces legacy partial lifecycle methods.
//      - GameRootMain implements this interface and delegates lifecycle operations to:
//          • GameRootInitialization
//          • GameRootUpdateLoop
//          • GameRootStateController
//          • GameRootSystemRegistration
//      - All engine-hosted programs MUST implement this interface without exception.
//      - Includes legacy compatibility signatures (Render(object), Tick(object,...)) for transitional
//        subsystem support, though the GPU-only pipeline uses Render(D3D11Adapter_Core).
// =====================================================================================================

using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    /// <summary>
    /// Deterministic lifecycle contract for engine-hosted programs.
    /// Implemented by GameRootMain and any future top-level engine modules.
    /// </summary>
    internal interface ID3D11RenderHost
    {
        // Strict lifecycle surface -------------------------------------------------------------

        /// <summary>
        /// Perform one-time initialization of the host program.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Enter the main execution loop. This should set the engine running flag
        /// and begin dispatching update/render ticks.
        /// </summary>
        void Run();

        /// <summary>
        /// Perform a single logical update tick.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last update, in seconds.</param>
        void Update(float deltaTime);

        /// <summary>
        /// Perform a single render dispatch using the GPU adapter core.
        /// </summary>
        /// <param name="adapterCore">GPU adapter core used for rendering.</param>
        void Render(D3D11Adapter_Core adapterCore);

        /// <summary>
        /// Perform orderly shutdown of the host program.
        /// </summary>
        void Shutdown();

        // Legacy compatibility signatures ------------------------------------------------------

        /// <summary>
        /// Legacy render forwarding surface for subsystems that still operate on
        /// generic render targets or non-GPU contexts.
        /// </summary>
        /// <param name="target">Arbitrary render target object.</param>
        void Render(object target);

        /// <summary>
        /// Legacy tick surface for subsystems that still expect object-based tick
        /// forwarding. Kept for transitional compatibility.
        /// </summary>
        /// <param name="state">Arbitrary state object.</param>
        /// <param name="deltaTime">Elapsed time since last tick, in seconds.</param>
        void Tick(object state, float deltaTime);
    }

    // Concrete D3D11-specific host marker for future extension if needed.
    // GameRootMain can implement ID3D11RenderHost directly; this class exists
    // only as a convenient type anchor for D3D11-specific host behaviors.
    internal sealed class D3D11RenderHost : ID3D11RenderHost
    {
        public void Initialize()
        {
            // Lifecycle entry point – implemented by GameRootMain in practice.
        }

        public void Run()
        {
            // Main loop entry – implemented by GameRootMain in practice.
        }

        public void Update(float deltaTime)
        {
            // Per-frame update – delegated to GameRootUpdateLoop / state controller.
        }

        public void Render(D3D11Adapter_Core adapterCore)
        {
            // GPU-context render dispatch – delegated to render subsystems.
        }

        public void Shutdown()
        {
            // Shutdown – delegated to GameRootStateController / system teardown.
        }

        public void Render(object target)
        {
            // Legacy render forwarding – transitional support for non-GPU pipelines.
        }

        public void Tick(object state, float deltaTime)
        {
            // Legacy tick forwarding – transitional support for object-based subsystems.
        }
    }
}

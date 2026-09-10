// =====================================================================================================
//  FILE: IProgram.cs
//  PATH: Engine/Platform/IProgram.cs
//  SUBSYSTEM: Platform Abstraction Layer
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

namespace SASZombieAssaultTD.Engine.Platform
{
    /// <summary>
    /// Defines the minimal deterministic lifecycle contract for any engine-hosted program. Implemented by GameRootMain
    /// to provide a clean engine-facing host boundary.
    /// </summary>
    public interface IProgram
    {
        void Dispose();

        /// <summary>
        /// Called exactly once at engine startup. Used to initialize all engine-level systems and sub-delegate modules.
        /// </summary>
        void Initialize();

        void Render(object value);

        void Render(D3D11Adapter_Core context);

        /// <summary>
        /// Enters the authoritative execution context loop. Responsible for driving inner timing loops, clock updates,
        /// and rendering boundaries continuously until a shutdown state is tripped.
        /// </summary>
        void Run();

        /// <summary>
        /// Called exactly once when the engine program receives a termination request. Cascades destruction signals
        /// across running subcomponents.
        /// </summary>
        void Shutdown();

        void Tick(object gameTime, ElapsedGameTime elapsedGameTime);

        void Update(float deltaTime);
    }

    public class ElapsedGameTime
    {
        public float TotalTime;
        public float Time;
        public float DeltaTime;
    }
}

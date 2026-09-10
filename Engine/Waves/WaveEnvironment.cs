// =====================================================================================================
//  FILE: WaveEnvironment.cs
//  PATH: Engine/Waves/WaveEnvironment.cs
//  SUBSYSTEM: Waves Subsystem
//
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

namespace SASZombieAssaultTD.Engine.Waves
{
    public class WaveEnvironment
    {
        public virtual void Initialize() { }
        public virtual void Run() { }
        public virtual void Shutdown() { }

        public virtual void Initialize_Internal() { }
        public virtual void Run_Internal() { }
        public virtual void Shutdown_Internal() { }
        public struct WaveState
        {
            public enum State
            {
                Uninitialized,
                Initialized,
                Running,
                Shutdown,
            }
            public State state = State.Uninitialized;

            public WaveState() => state = State.Uninitialized;
            public void Transition(State newState)
            {
                state = newState;
            }
        }
    }
}

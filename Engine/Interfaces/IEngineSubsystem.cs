// =====================================================================================================
//  FILE: IEngineSubsystem.cs
//  PATH: Engine/Interfaces/IEngineSubsystem.cs
//  SUBSYSTEM: Core Engine Interface
//
//  ROLE:
//      Defines the common lifecycle contract implemented by all engine subsystems. This interface
//      ensures deterministic update ordering, consistent reset behavior, and clean integration with
//      SystemRegistry.
//
//  RESPONSIBILITIES:
//      - Expose Update() for per-frame subsystem logic.
//      - Expose Clear() for subsystem reset or shutdown.
//      - Provide a minimal, stable API that all subsystems adhere to.
//
//  NON-RESPONSIBILITIES:
//      - Implement subsystem-specific logic (delegated to concrete subsystem classes).
//      - Define input, rendering, audio, or gameplay behavior.
//      - Perform OS-level polling or device interactions.
//
//  NOTES:
//      This interface is intentionally minimal. It exists solely to unify subsystem lifecycle
//      management across the engine.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Interfaces
{
    /// <summary>
    /// Common lifecycle interface for all engine subsystems managed by SystemRegistry.
    /// </summary>
    public interface IEngineSubsystem
    {
        /// <summary>
        /// Called once per frame to update subsystem state.
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// Clears or resets subsystem state.
        /// </summary>
        void Clear();
    }
}

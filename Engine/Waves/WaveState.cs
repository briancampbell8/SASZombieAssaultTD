// =====================================================================================================
//  FILE: WaveState.cs
//  PATH: Engine/Waves/WaveState.cs
//  SUBSYSTEM: Waves
//
//  ROLE:
//      Defines the global discrete runtime state milestones for the wave orchestration subsystems.
//
//  RESPONSIBILITIES:
//      - Enumerate all possible operational phases of a wave timeline tracking execution flow.
//      - Serve as a pure, lightweight type contract accessible by decoupled engine modules.
//
//  NON-RESPONSIBILITIES:
//      - Storing active timer data or tracking internal state mutations.
//      - Executing conditional state transitions or evaluation rules.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Waves
{
    /// <summary>
    /// Explicit enumeration defining the precise execution lifecycle phases of the wave system engine.
    /// </summary>
    public enum WaveState
    {
        /// <summary>
        /// Default state before any game initialization occurs.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Initial gameplay buffer stage waiting for user interaction or trigger rules to start.
        /// </summary>
        WaitingToStart,

        /// <summary>
        /// Intermediate state handling setup arrays and preparatory spawner allocations.
        /// </summary>
        Starting,

        /// <summary>
        /// Active round execution timeline where enemy entities are actively processing.
        /// </summary>
        InProgress,

        /// <summary>
        /// Down-time countdown step between completed active rounds.
        /// </summary>
        InterWave,

        /// <summary>
        /// Victory phase reached after completing the final wave sequence threshold.
        /// </summary>
        Complete,

        /// <summary>
        /// Explicit cancellation or debug halt phase forcing execution loops to sleep.
        /// </summary>
        Stopped
    }
}

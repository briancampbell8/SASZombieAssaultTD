// ====================================================================================================
//  FILE: IAnimationState.cs
//  PATH: Engine/Animation/Core/State/IAnimationState.cs
//  MODULE: Animation Core State
//
//  ROLE:
//      Encapsulate core engine behavior for the IAnimationState module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Animation.Core.Clips;

namespace SASZombieAssaultTD.Engine.Animation.Core.State
{
    /// <summary>
    /// P11-17-02: Interface for animation states with explicit method contracts. Defines Enter, Exit, and Update
    /// methods with no assumptions or side effects.
    /// </summary>
    public interface IAnimationState
    {
        /// <summary>
        /// P11-17-02: Unique name identifier for the state. Provides deterministic state identification without
        /// assumptions.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// P11-17-02: Animation clip associated with this state. Provides deterministic clip association for
        /// state-based animation.
        /// </summary>
        AnimationClip? Clip { get; }

        /// <summary>
        /// P11-17-02: Called when entering the state. Explicit initialization method with no side effects beyond state
        /// setup.
        /// </summary>
        void Enter();

        /// <summary>
        /// P11-17-02: Called when exiting the state. Explicit cleanup method with no side effects beyond state cleanup.
        /// </summary>
        void Exit();

        /// <summary>
        /// P11-17-02: Called during state update with deterministic timing. Updates state logic without external side
        /// effects.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
        /// <param name="timeInState">Total time elapsed in current state in seconds.</param>
        void Update(float deltaTime, float timeInState);

        /// <summary>
        /// P11-17-02: Checks for state transitions with deterministic conditions. Returns next state if transition
        /// conditions are met, null otherwise. No side effects - only evaluates transition conditions.
        /// </summary>
        /// <returns>Next state to transition to, or null if no transition.</returns>
        IAnimationState? CheckTransitions();

        /// <summary>
        /// P11-17-02: Gets state-specific parameters for debugging and inspection. Returns deterministic parameter
        /// values without side effects.
        /// </summary>
        /// <returns>Dictionary of state parameters.</returns>
        Dictionary<string, object> GetParameters();

        /// <summary>
        /// P11-17-02: Validates state configuration and dependencies. Returns validation result without side effects.
        /// </summary>
        /// <returns>True if state is valid, false otherwise.</returns>
        bool IsValid();

        /// <summary>
        /// P11-17-02: Gets debug information about the state. Returns deterministic debug information without side
        /// effects.
        /// </summary>
        /// <returns>Debug information string.</returns>
        string GetDebugInfo();
    }
}

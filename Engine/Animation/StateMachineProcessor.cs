// =====================================================================================================
//  FILE: StateMachineProcessor.cs
//  PATH: Engine/Animation/StateMachineProcessor.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Provides deterministic update logic for animation state machines. This module evaluates
//      state transitions, updates active states, and ensures animation-driven behavior remains
//      consistent across frames.
//
//  RESPONSIBILITIES:
//      - Update AnimationStateMachineComponent instances each frame.
//      - Evaluate transitions and trigger state changes deterministically.
//      - Maintain strict subsystem boundaries: no rendering, no ECS world ownership,
//        no animation controller logic, no gameplay callbacks.
//      - Log state machine activity for debugging and profiling.
//
//  NON-RESPONSIBILITIES:
//      - Animation controller playback logic.
//      - Rendering or sprite updates.
//      - Gameplay callbacks triggered by animation events.
//      - ECS ECSEntityCore lifecycle management.
//      - Deterministic sequencing (handled by ECSRuntimeCore).
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally thin and stateless.
//      - All state machine updates must route through this processor.
//      - Ensures animation state machine behavior remains isolated and deterministic.
//
//  CHANGE LOG:
//      • 08-15-2026 — File created during Animation subsystem modernization (BDC).
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Provides deterministic update logic for animation state machines.
    /// </summary>
    internal sealed class StateMachineProcessor
    {
        // =====================================================================================================
        //  PUBLIC API
        // =====================================================================================================
        private readonly ECSRuntimeCore runtimeCore;
        /// <summary>
        /// Updates all AnimationStateMachineComponent instances found on the provided ECSEntityCore.
        /// </summary>
        public void UpdateStateMachines(ECSEntityCore entity, float deltaTime)
        {

            // ECSRuntimeCore is the only valid component access point now
            var stateMachine = runtimeCore.GetComponent<AnimationMachineComponent>(entity);
            if (stateMachine == null || !stateMachine.IsRunning)
                return;

            try
            {
                stateMachine.Update(deltaTime);

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug,
                    $"StateMachineProcessor: Entity {entity.Id} updated state machine (State={stateMachine.CurrentStateName})");
            }
            catch (System.Exception ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error,
                    $"StateMachineProcessor: Error updating state machine for Entity {entity.Id}: {ex.Message}");
            }
        }


    }
}

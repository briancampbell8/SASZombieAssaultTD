// =====================================================================================================
//  FILE: TransitionResolver.cs
//  PATH: Engine/Animation/TransitionResolver.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Provides deterministic evaluation and resolution of animation state transitions.
//      This module inspects transition conditions, determines whether a transition should
//      occur, and returns the resolved next state for the animation state machine.
//
//  RESPONSIBILITIES:
//      - Evaluate transition conditions for AnimationStateMachineComponent.
//      - Determine whether a transition should occur based on parameters and rules.
//      - Maintain strict subsystem boundaries: no rendering, no ECS world ownership,
//        no controller update logic, no gameplay callbacks.
//      - Log transition evaluation activity for debugging and profiling.
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
//      - All transition evaluation must route through this resolver.
//      - Ensures animation transition behavior remains isolated and deterministic.
//
//  CHANGE LOG:
//      • 08-15-2026 — File created during Animation subsystem modernization (BDC).
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Components;
using SASZombieAssaultTD.Engine.Animation.Core.Transitions;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Provides deterministic evaluation of animation transitions.
    /// </summary>
    internal sealed class TransitionResolver
    {
        // =====================================================================================================
        //  PUBLIC API
        // =====================================================================================================

        public delegate bool TransitionCondition(AnimationMachineComponent stateMachine);
        public delegate string? TransitionResolverDelegate(AnimationMachineComponent stateMachine, string currentState);

        public string? ResolveTransition(AnimationMachineComponent stateMachine, string currentState) => ResolveTransition(stateMachine);

        public string? ResolveTransition(AnimationMachineComponent stateMachine)
        {
            if (stateMachine == null || !stateMachine.IsRunning)
                return null;

            var currentState = stateMachine.CurrentStateName;

            // 1. Cast the generic object to a non-generic IDictionary interface
            if (stateMachine.Transitions is not System.Collections.IDictionary transitionsDict)
                return null;

            // 2. Safely perform key checking and extract the collection of transitions
            if (!transitionsDict.Contains(currentState))
                return null;

            var transitions = transitionsDict[currentState] as System.Collections.IEnumerable;
            if (transitions == null)
                return null;

            // 3. Process the state transitions sequentially
            foreach (var obj in transitions)
            {
                // Safely extract individual transition entities from the list loop
                if (obj is not AnimationTransition transition)
                    continue;

                try
                {
                    if (EvaluateCondition(stateMachine, transition))
                    {
                        DLogger.Log(LogSubsystems.Animation, LogLevel.Debug,
                            $"TransitionResolver: Transition triggered from '{currentState}' → '{transition.TargetState}'");

                        return (string)transition.TargetState;
                    }
                }
                catch (System.Exception ex)
                {
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Error,
                        $"TransitionResolver: Error evaluating transition for state '{currentState}': {ex.Message}");
                }
            }

            return null;
        }


        // =====================================================================================================
        //  INTERNAL HELPERS
        // =====================================================================================================

        /// <summary>
        /// Evaluates a single transition condition.
        /// </summary>
        private bool EvaluateCondition(AnimationMachineComponent stateMachine, AnimationTransition transition)
        {
            // If no condition is defined, transition is unconditional
            if (transition.Condition == null)
                return true;

            try
            {
                // Safely handles both direct boolean fields or generic objects by checking truths
                if (transition.Condition is System.Delegate del)
                {
                    return (bool)del.DynamicInvoke(stateMachine)!;
                }

                // If it's a built-in custom class or primitive type wrapper, handle it or return true
                return true;
            }
            catch (System.Exception ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error,
                    $"TransitionResolver: Condition evaluation failed for transition to '{transition.TargetState}': {ex.Message}");
                return false;
            }
        }
    }
}

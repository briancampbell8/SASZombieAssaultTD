// =====================================================================================================
//  FILE: ControllerProcessor.cs
//  PATH: Engine/Animation/ControllerProcessor.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Provides deterministic update logic for AnimationControllerComponent instances.
//      This module updates playback time, evaluates clip transitions, and ensures animation
//      controllers remain consistent across frames.
//
//  RESPONSIBILITIES:
//      - Update AnimationControllerComponent playback state.
//      - Evaluate clip transitions and detect clip changes.
//      - Track event-crossing conditions for animation events.
//      - Maintain strict subsystem boundaries: no rendering, no ECS world ownership,
//        no state machine logic, no gameplay callbacks.
//      - Log controller update activity for debugging and profiling.
//
//  NON-RESPONSIBILITIES:
//      - Animation state machine evaluation.
//      - Rendering or sprite updates.
//      - Gameplay callbacks triggered by animation events.
//      - ECS ECSEntityCore lifecycle management.
//      - Deterministic sequencing (handled by ECSRuntimeCore).
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally thin and stateless.
//      - All animation controller updates must route through this processor.
//      - Ensures animation controller behavior remains isolated and deterministic.
//
//  CHANGE LOG:
//      • 08-15-2026 — File created during Animation subsystem modernization (BDC).
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Provides deterministic update logic for animation controllers.
    /// </summary>
    internal sealed class ControllerProcessor
    {
        private readonly ECSComponents _components;

        // =====================================================================================================
        //  PUBLIC API
        // =====================================================================================================

        /// <summary>
        /// Updates the AnimationControllerComponent on the given ECSEntityCore, if present.
        /// </summary>
        public void UpdateController(ECSEntityCore ECSEntityCore, float deltaTime)
        {
            var controller = _components.GetComponent<AnimationControllerComponent>(ECSEntityCore.Id);
            if (controller == null)
                return;

            var previousTime = controller.PlaybackTime;
            var previousClip = controller.CurrentClip;

            try
            {
                controller.Update(deltaTime);

                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                    $"ControllerProcessor: ECSEntityCore {ECSEntityCore.Id} updated controller (Clip='{controller.CurrentClip}', Time={controller.PlaybackTime:F3})");
            }
            catch (System.Exception ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Error,
                    $"ControllerProcessor: Error updating controller for ECSEntityCore {ECSEntityCore.Id}: {ex.Message}");
            }
        }

        // =====================================================================================================
        //  EVENT CROSSING HELPERS
        // =====================================================================================================

        /// <summary>
        /// Determines whether an animation event timestamp was crossed during this frame.
        /// </summary>
        public bool EventCrossed(AnimationControllerComponent controller, AnimationEvent animationEvent,
                                 float previousTime, float currentTime)
        {
            return controller.Speed >= 0
                ? previousTime < animationEvent.Timestamp && currentTime >= animationEvent.Timestamp
                : previousTime > animationEvent.Timestamp && currentTime <= animationEvent.Timestamp;
        }
    }
}

// =====================================================================================================
//  FILE: RenderableUpdater.cs
//  PATH: Engine/Animation/RenderableUpdater.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Provides deterministic synchronization between AnimationControllerComponent instances
//      and ECS renderable components (e.g., SpriteComponent). Ensures visual output matches
//      the current animation state each frame.
//
//  RESPONSIBILITIES:
//      - Update renderable components based on animation controller state.
//      - Apply sprite index changes, transform offsets, color tint, and animation time.
//      - Maintain strict subsystem boundaries: no sequencing, no ECS world ownership,
//        no controller update logic, no state machine logic.
//      - Log renderable update activity for debugging and profiling.
//
//  NON-RESPONSIBILITIES:
//      - Animation controller playback logic.
//      - Animation state machine evaluation.
//      - Gameplay callbacks triggered by animation events.
//      - ECS ECSEntityCore lifecycle management.
//      - Deterministic sequencing (handled by ECSRuntimeCore).
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally thin and stateless.
//      - All renderable updates must route through this processor.
//      - Ensures animation → rendering integration remains isolated and deterministic.
//
//  CHANGE LOG:
//      • 08-15-2026 — File created during Animation subsystem modernization (BDC).
// =====================================================================================================

using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Provides deterministic synchronization between animation controllers and renderable components.
    /// </summary>
    public sealed class RenderableUpdater
    {
        private readonly ECSRuntimeCore _runtime;

        public RenderableUpdater(ECSRuntimeCore runtime)
        {
            _runtime = runtime ?? throw new System.ArgumentNullException(nameof(runtime));
        }

        // =====================================================================================================
        //  PUBLIC API
        // =====================================================================================================

        public void UpdateRenderable(ECSEntityCore entity)
        {
            // SpriteComponent required
            var renderable = _runtime.Components.GetComponent<SpriteComponent>(entity.Id);
            if (renderable == null)
                return;

            // AnimationControllerComponent required
            var controller = _runtime.Components.GetComponent<AnimationControllerComponent>(entity.Id);
            if (controller == null)
                return;

            var clip = controller.GetCurrentClip();
            if (clip == null)
                return;

            try
            {
                // Sync sprite index (property, not method)
                renderable.SpriteIndex = (int)controller.GetCurrentSpriteIndex;

                // Sync transform offset (object → tuple)
                if (controller.CurrentTransformOffset is (float ox, float oy))
                {
                    renderable.TransformOffset = (ox, oy);
                }

                // Sync color tint (already a float tuple)
                var tint = controller.CurrentColorTint;
                renderable.ColorTint = (tint.R, tint.G, tint.B, tint.A);

                // Sync animation time
                renderable.AnimationTime = controller.PlaybackTime;

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug,
                    $"RenderableUpdater: Entity {entity.Id} updated renderable " +
                    $"(SpriteIndex={renderable.SpriteIndex}, Time={renderable.AnimationTime:F3})");
            }
            catch (System.Exception ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error,
                    $"RenderableUpdater: Error updating renderable for Entity {entity.Id}: {ex.Message}");
            }
        }
    }
}

// =====================================================================================================
//  FILE: UnifiedSystemCorePass.cs
//  PATH: Engine/ECS/ECSSystem/UnifiedSystemCorePass.cs
//  SUBSYSTEM: ECS – Unified Animation & Render Sync Pass
//
//  ROLE:
//      Provides a unified per‑frame animation update pass that processes:
//      • AnimationControllerComponent playback
//      • AnimationMachineComponent state machine updates
//      • Renderable (SpriteComponent) synchronization
//
//  RESPONSIBILITIES:
//      - Iterate deterministically over all ECS entities
//      - Update animation playback time
//      - Update animation state machines
//      - Synchronize renderable components with animation state
//      - Maintain per‑frame statistics for debugging/profiling
//
//  NON‑RESPONSIBILITIES:
//      - Animation event firing (handled by AnimationSystem)
//      - Animation state machine transitions (handled by AnimationMachineComponent)
//      - Rendering (handled by RenderSystem)
//      - ECS lifecycle management
//
//  ARCHITECTURAL NOTES:
//      - This pass is intentionally thin and stateless.
//      - All component access must route through ECSRuntimeCore.Components.
//      - ECSEntityCore never exposes TryGetComponent or Update logic.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Animation.Components;      // AnimationMachineComponent
using SASZombieAssaultTD.Engine.Components;                // SpriteComponent
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS.ECSSystem
{
    internal sealed class UnifiedSystemCorePass
    {
        private readonly ECSRuntimeCore _runtime;

        private int _entitiesProcessed;
        private int _clipsUpdated;
        private int _statesUpdated;
        private float _totalAnimationTime;

        public UnifiedSystemCorePass(ECSRuntimeCore runtime)
        {
            _runtime = runtime ?? throw new System.ArgumentNullException(nameof(runtime));
        }

        // =====================================================================================================
        //  MAIN PASS
        // =====================================================================================================
        public void UnifiedAnimationPass(float deltaTime)
        {
            foreach (var entity in _runtime.Entities.All)
            {
                _entitiesProcessed++;

                // -----------------------------------------------------------------------------------------
                // Animation Controller Update
                // -----------------------------------------------------------------------------------------
                if (_runtime.TryGetComponent(entity, out Animation.AnimationControllerComponent controller))
                {
                    var previousTime = controller.PlaybackTime;
                    var previousClip = controller.CurrentClip;

                    controller.UpdatePlaybackTime(deltaTime);

                    _clipsUpdated++;
                    _totalAnimationTime += deltaTime;

                    if (previousClip != controller.CurrentClip)
                    {
                        DLogger.Log(LogSubsystems.Animation, LogLevel.Debug,
                            $"UnifiedPass: Entity {entity.Id} changed clip '{previousClip}' → '{controller.CurrentClip}'");
                    }
                }

                // -----------------------------------------------------------------------------------------
                // Animation State Machine Update
                // -----------------------------------------------------------------------------------------
                if (_runtime.TryGetComponent(entity, out AnimationMachineComponent stateMachine))
                {
                    if (stateMachine.IsRunning)
                    {
                        stateMachine.Update(deltaTime);
                        _statesUpdated++;
                    }
                }

                // -----------------------------------------------------------------------------------------
                // Renderable Sync
                // -----------------------------------------------------------------------------------------
                if (_runtime.TryGetComponent(entity, out SpriteComponent renderable) &&
                    _runtime.TryGetComponent(entity, out Animation.AnimationControllerComponent controller2))
                {
                    var clip = controller2.GetCurrentClip();
                    if (clip != null)
                    {
                        renderable.SpriteIndex = (int)controller2.GetCurrentSpriteIndex;
                        renderable.AnimationClip = clip;

                        if (controller2.CurrentTransformOffset is (float ox, float oy))
                            renderable.TransformOffset = (ox, oy);

                        renderable.ColorTint = controller2.CurrentColorTint;
                        renderable.AnimationTime = controller2.PlaybackTime;
                    }
                }
            }
        }
    }
}

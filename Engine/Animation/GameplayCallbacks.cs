// =====================================================================================================
//  FILE: GameplayCallbacks.cs
//  PATH: Engine/Animation/GameplayCallbacks.cs
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Provides deterministic gameplay-side callback handlers for animation events.
//      This module is invoked by higher-level animation systems (e.g., EventDispatcher,
//      ControllerProcessor, StateMachineProcessor) whenever an animation event requires
//      gameplay integration.
//
//  RESPONSIBILITIES:
//      - Handle animation-driven gameplay actions (weapon fire, footsteps, sound triggers,
//        effect spawning, area damage, etc.).
//      - Provide a clean, isolated API for gameplay callbacks triggered by animation events.
//      - Maintain strict subsystem boundaries: no sequencing, no ECS world ownership,
//        no rendering, no state machine logic.
//      - Log all gameplay callback activity via DLogger for debugging and profiling.
//
//  NON-RESPONSIBILITIES:
//      - Animation state machine evaluation.
//      - Animation controller updates.
//      - Rendering or sprite updates.
//      - ECS ECSEntityCore lifecycle management.
//      - Deterministic sequencing (handled by ECSRuntimeCore).
//
//  ARCHITECTURAL NOTES:
//      - This module is intentionally thin and stateless.
//      - All gameplay logic triggered by animation events must route through this module.
//      - Ensures animation → gameplay integration remains deterministic and isolated.
//
//  CHANGE LOG:
//      • 08-15-2026 — File created during Animation subsystem modernization (BDC).
// =====================================================================================================
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Provides deterministic gameplay callback handlers for animation events.
    /// </summary>
    internal sealed class GameplayCallbacks
    {
        // =====================================================================================================
        //  PUBLIC CALLBACK API
        // =====================================================================================================
        private readonly ECSComponents component;



        public void HandleFireWeapon(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var damage = animationEvent.GetParameter<float>("Damage");
            if (damage.Equals(default(float)))
                damage = 10f;

            component.AddComponent<DamageComponent>(
                controller.Entity.Id,
                new DamageComponent { Damage = damage });


            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                $"GameplayCallbacks: ECSEntityCore {controller.Entity.Id} fired weapon for {damage} damage");
        }

        public void HandleFootstep(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var soundName = animationEvent.GetParameter<string>("Sound") ?? "footstep";

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                $"GameplayCallbacks: ECSEntityCore {controller.Entity.Id} footstep sound '{soundName}'");
        }

        public void HandlePlaySound(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var soundName = animationEvent.GetParameter<string>("SoundName") ?? "unknown";

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                $"GameplayCallbacks: ECSEntityCore {controller.Entity.Id} playing sound '{soundName}'");
        }

        public void HandleSpawnEffect(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var effectType = animationEvent.GetParameter<string>("EffectType") ?? "default";

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                $"GameplayCallbacks: ECSEntityCore {controller.Entity.Id} spawning effect '{effectType}'");
        }

        public void HandleDamageArea(AnimationControllerComponent controller, AnimationEvent animationEvent)
        {
            var damage = animationEvent.GetParameter<float>("Damage");
            if (damage.Equals(default(float)))
                damage = 25f;

            component.AddComponent<DamageComponent>(
                controller.Entity.Id,
                new DamageComponent { Damage = damage });

            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug,
                $"GameplayCallbacks: ECSEntityCore {controller.Entity.Id} created area damage: {damage}");
        }
    }
}

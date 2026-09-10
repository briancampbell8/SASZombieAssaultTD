// =====================================================================================================
//  FILE: SystemHooks.cs
//  PATH: Engine/ECS/ESCSystem/SystemHooks.cs
//  SUBSYSTEM: ECS ESCSystem Hooks
//
//  ROLE:
//      Provides the unified deterministic hook surface for all ECSSystem lifecycle events.
//      This module defines the complete set of overridable lifecycle callbacks used by
//      SystemCore and all derived ECSSystem modules.
//
//  RESPONSIBILITIES:
//      - Define the authoritative lifecycle hook contract for ECSSystem modules.
//      - Provide a stable, deterministic callback surface for system initialization,
//        update phases, rendering, destruction, and reset operations.
//      - Ensure all ECSSystem implementations share a consistent hook structure.
//      - Serve as the dedicated hook container for the ESCSystem subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Executing lifecycle logic (handled by SystemCore).
//      - Typed system specialization (handled by SystemTyped).
//      - System instantiation or construction (handled by SystemConstructor / SystemFactory).
//      - Component processing, ECSEntityCore management, or world orchestration.
//
//  ARCHITECTURAL NOTES:
//      - This module replaces all legacy scattered hook definitions across the engine.
//      - All ECSSystem modules must reference and follow the hook structure defined here.
//      - Hook ordering and naming are deterministic and must remain stable for engine runtime.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.ECS.ESCSystem
{
    internal abstract class SystemHooks
    {
        protected virtual void OnInitialize() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnFixedUpdate(float fixedDeltaTime) { }
        protected virtual void OnLateUpdate(float deltaTime) { }
        protected virtual void OnRender() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnReset() { }
        // ---------------------------------------------------------------------------------------------
        // HOOKS — SUBSYSTEM-SPECIFIC BEHAVIOR
        // ---------------------------------------------------------------------------------------------

        protected virtual void OnInitialize(ECSRuntimeCore world)
        { }

        protected virtual void OnUpdate(ECSRuntimeCore world, float deltaTime)
        { }

        protected virtual void OnFixedUpdate(ECSRuntimeCore world, float fixedDeltaTime)
        { }

        protected virtual void OnLateUpdate(ECSRuntimeCore world, float deltaTime)
        { }

        protected virtual void OnRender(ECSRuntimeCore world)
        { }

        protected virtual void OnDestroy(ECSRuntimeCore world)
        { }

    }
}

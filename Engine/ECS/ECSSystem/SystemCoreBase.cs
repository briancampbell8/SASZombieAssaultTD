// =====================================================================================================
//  FILE: SystemCore.cs
//  PATH: Engine/ECS/ESCSystem/SystemCore.cs
//  SUBSYSTEM: ECS ESCSystem Core
//
//  ROLE:
//      Provides the deterministic core lifecycle implementation for all ECS-driven engine systems.
//      This module defines the unified, minimal, and strictly ordered execution contract used by
//      every system hosted within the ECSSystem subsystem.
//
//  RESPONSIBILITIES:
//      - Define the deterministic lifecycle surface for all ECS systems.
//      - Enforce strict sequencing: Initialize → Update → FixedUpdate → LateUpdate → Render → Destroy.
//      - Maintain system state flags (enabled, initialized, ready).
//      - Track update metrics (last update time, update count).
//      - Provide virtual hook points for subsystem-specific behavior.
//      - Serve as the foundational base class for all ECSSystem modules.
//
//  NON-RESPONSIBILITIES:
//      - Typed system specialization (handled by SystemTyped).
//      - System instantiation or factory logic (handled by SystemFactory).
//      - Component processing, ECSEntityCore management, or world orchestration.
//      - Rendering pipeline management or low-level device operations.
//
//  ARCHITECTURAL NOTES:
//      - This module replaces all legacy fragmented system base classes.
//      - All ECS systems MUST inherit from SystemCore without exception.
//      - SystemCore provides deterministic behavior guarantees required by the engine runtime.
// =====================================================================================================
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS.ESCSystem
{
    internal abstract class SystemCoreBase : ISystemCore
    {
        // Removed the invalid static 'ECSCompCoreHelpers' field. 
        // Use static utility methods globally via 'ECSCompCoreHelpers.Method()' instead.

        public abstract List<string> _systemsField { get; }
        public abstract bool IsEnabled { get; }
        public abstract bool IsInitialized { get; }
        public abstract bool IsReady { get; }
        public abstract float LastUpdateTime { get; }
        public abstract uint UpdateCount { get; }

        public abstract void Add();
        public abstract void AddSystem(object system);
        public abstract void Disable();
        public abstract void Enable();
        public abstract void Toggle();
        public abstract object GetSystems();
        public abstract void RemoveSystem(object system);
        public abstract void Reset();
        public abstract void SetSystems(object systems);
        public abstract void UnifiedAnimationPass(float deltaTime);
        public abstract T GetSystem<T>();

        // Changed 'override' to 'virtual' and added the required 'world' parameters
        public virtual void Initialize(ECSRuntimeCore world) { }
        public virtual void Destroy(ECSRuntimeCore world) { }
        public virtual void Update(ECSRuntimeCore world, float deltaTime) { }
        public virtual void FixedUpdate(ECSRuntimeCore world, float fixedDeltaTime) { }
        public virtual void LateUpdate(ECSRuntimeCore world, float deltaTime) { }
        public virtual void Render(ECSRuntimeCore world) { }
    }
}

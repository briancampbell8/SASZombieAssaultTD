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
    internal interface ISystemCore
    {
        List<string> _systemsField { get; }
        bool IsEnabled { get; }
        bool IsInitialized { get; }
        bool IsReady { get; }
        float LastUpdateTime { get; }
        uint UpdateCount { get; }

        void Add();
        void AddSystem(object system);
        void Destroy(ECSRuntimeCore world);
        void Disable();
        void Enable();
        void FixedUpdate(ECSRuntimeCore world, float fixedDeltaTime);
        T GetSystem<T>();
        object GetSystems();
        void Initialize(ECSRuntimeCore world);
        void LateUpdate(ECSRuntimeCore world, float deltaTime);
        void RemoveSystem(object system);
        void Render(ECSRuntimeCore world);
        void Reset();
        void SetSystems(object systems);
        void Toggle();
        string ToString();
        void UnifiedAnimationPass(float deltaTime);
        void Update(ECSRuntimeCore world, float deltaTime);
    }
}

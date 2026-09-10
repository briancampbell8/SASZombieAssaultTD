// =====================================================================================================
//  FILE: SystemState.cs
//  PATH: Engine/ECS/ESCSystem/SystemState.cs
//  SUBSYSTEM: ECS ESCSystem State
//
//  ROLE:
//      Provides deterministic state tracking for ECSSystem modules.
//      This module defines the unified state container used by SystemCore and all derived systems,
//      ensuring consistent representation of initialization, readiness, enablement, and update metrics.
//
//  RESPONSIBILITIES:
//      - Maintain deterministic state flags for ECSSystem modules.
//      - Track initialization, enablement, and readiness conditions.
//      - Store update metrics including last update time and update count.
//      - Provide a stable, engine-wide state representation for system lifecycle operations.
//      - Serve as the authoritative state container for the ESCSystem subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Executing lifecycle logic (handled by SystemCore).
//      - Providing lifecycle hooks (handled by SystemHooks).
//      - Typed system specialization (handled by SystemTyped).
//      - System construction or instantiation (handled by SystemConstructor / SystemFactory).
//
//  ARCHITECTURAL NOTES:
//      - This module replaces all legacy scattered state fields across system implementations.
//      - All ECSSystem modules must reference and maintain state through SystemState.
//      - Deterministic state tracking ensures stable runtime behavior across the engine.
// =====================================================================================================

using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Engine.ECS.ESCSystem
{
    internal sealed class SystemState
    {
        public bool IsEnabled { get; set; }
        public bool IsInitialized { get; set; }

        public bool IsReady => IsEnabled && IsInitialized;

        public float LastUpdateTime { get; set; }
        public uint UpdateCount { get; set; }

        public SystemPriority Priority { get; set; }

        public SystemState(SystemPriority priority = SystemPriority.Normal)
        {
            IsEnabled = true;
            IsInitialized = false;
            Priority = priority;
            LastUpdateTime = 0f;
            UpdateCount = 0;
        }
    }
}

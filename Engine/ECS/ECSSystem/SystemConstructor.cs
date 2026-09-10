// =====================================================================================================
//  FILE: SystemConstructor.cs
//  PATH: Engine/ECS/ESCSystem/SystemConstructor.cs
//  SUBSYSTEM: ECS ESCSystem Constructor
//
//  ROLE:
//      Provides deterministic construction utilities for ECSSystem modules.
//      This module defines the unified creation surface used by system initialization pipelines,
//      ensuring consistent priority assignment, state initialization, and structural compliance
//      across all ECSSystem-derived types.
//
//  RESPONSIBILITIES:
//      - Provide deterministic construction routines for ECSSystem modules.
//      - Enforce structural correctness for system instantiation.
//      - Validate system types prior to construction.
//      - Apply initial priority and state configuration rules.
//      - Serve as the authoritative constructor utility for the ESCSystem subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Typed system specialization (handled by SystemTyped).
//      - System lifecycle execution (handled by SystemCore).
//      - System factory orchestration (handled by SystemFactory).
//      - Component processing, ECSEntityCore management, or world orchestration.
//
//  ARCHITECTURAL NOTES:
//      - This module replaces all legacy ad-hoc system construction logic.
//      - All ECSSystem modules must be constructed through SystemConstructor or SystemFactory.
//      - Deterministic construction ensures stable runtime behavior across all engine systems.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.ECS.ECSEnums;

namespace SASZombieAssaultTD.Engine.ECS.ESCSystem
{
    internal static class SystemConstructor
    {
        public static SystemCore Create(Type systemType, SystemPriority priority = SystemPriority.Normal)
        {
            if (systemType == null)
                throw new ArgumentNullException(nameof(systemType));

            if (!typeof(SystemCore).IsAssignableFrom(systemType))
                throw new ArgumentException($"Type '{systemType.Name}' does not inherit from SystemCore.");

            var instance = (SystemCore)Activator.CreateInstance(systemType, priority);
            return instance;
        }

        public static T Create<T>(SystemPriority priority = SystemPriority.Normal) where T : SystemCore
        {
            var instance = (T)Activator.CreateInstance(typeof(T), priority);
            return instance;
        }
    }
}

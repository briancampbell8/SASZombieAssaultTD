// =====================================================================================================
//  FILE: ECSRuntimeEventss.cs
//  PATH: Engine/ECS/ECSRuntime/ECSRuntimeEventss.cs
//  SUBSYSTEM: ECS ECSRuntime Events
//
//  ROLE:
//      Provides deterministic event subscription, unsubscription, and dispatch services for the
//      ECSRuntime subsystem. Acts as the modular replacement for the legacy monolithic EventManager,
//      supplying stable, predictable event‑routing behavior across all ECS systems.
//
//  RESPONSIBILITIES:
//      - Maintain per‑event‑type handler lists.
//      - Provide Subscribe<T>() and Unsubscribe<T>() behavior.
//      - Provide deterministic Dispatch<T>() behavior.
//      - Ensure stable ordering guarantees for event delivery.
//      - Operate as a dedicated ECSRuntime subsystem owned by ECSRuntimeCore.
//
//  NON-RESPONSIBILITIES:
//      - Managing ECS ECSEntityCore lifecycle or component storage.
//      - Executing system logic or update‑loop sequencing.
//      - Performing spatial or collision queries.
//      - Managing system registration or priority ordering.
//
//  ARCHITECTURAL NOTES:
//      - Extracted from the legacy ECSRuntimeCore monolithic event bus.
//      - Designed for strict modularity under the new ECSRuntime architecture.
//      - All event routing is type‑safe and deterministic.
//      - ECSRuntimeCore exposes this subsystem via the Events property.
// =====================================================================================================


using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.ECS
{
    internal sealed class ECSRuntimeEventss
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);

            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();

            _handlers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var list))
                list.Remove(handler);
        }

        public void Dispatch<T>(T evt)
        {
            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var list))
            {
                foreach (var h in list)
                    ((Action<T>)h)(evt);
            }
        }
    }
}

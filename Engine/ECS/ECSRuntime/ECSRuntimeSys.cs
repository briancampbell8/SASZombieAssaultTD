// =====================================================================================================
//  FILE: ECSRuntimeSys.cs
//  PATH: Engine/ECS/ECSRuntime/ECSRuntimeSys.cs
//  SUBSYSTEM: ECS ECSRuntime
//
//  ROLE:
//      Deterministic system‑management subsystem for ECSRuntimeCore.
//      Stores, registers, removes, and retrieves IECSSystem instances.
// =====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.ECS
{
    internal sealed class ECSRuntimeSys
    {
        private readonly List<IECSSystem> _systems = new();
        private readonly Dictionary<Type, IECSSystem> _lookup = new();

        public int Count => _systems.Count;
        public IReadOnlyCollection<IECSSystem> Active => _systems;
        public IEnumerable<IECSSystem> ByPriority => _systems.OrderBy(s => s.Priority);

        public ECSRuntimeSys(ECSRuntimeCore runtime)
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Systems", 1, "Init", "ECSRuntimeSys subsystem initialized.");
        }

        public void Add(object system)
        {
            if (system is not IECSSystem ecsSystem)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Systems", 2, "Add", "Attempted to add non‑IECSSystem.");
                return;
            }

            _systems.Add(ecsSystem);
            _lookup[ecsSystem.GetType()] = ecsSystem;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Systems", 3, "Add", $"System added: {ecsSystem.GetType().Name}");
        }

        public void Remove(object system)
        {
            if (system is not IECSSystem ecsSystem)
                return;

            _systems.Remove(ecsSystem);
            _lookup.Remove(ecsSystem.GetType());

            DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Systems", 4, "Remove", $"System removed: {ecsSystem.GetType().Name}");
        }

        public void Remove<T>() where T : IECSSystem
        {
            if (_lookup.TryGetValue(typeof(T), out var system))
            {
                _systems.Remove(system);
                _lookup.Remove(typeof(T));

                DLogger.Log(LogSubsystems.ResourcesPipeline, "ECSRuntime.Systems", 5, "RemoveType", $"System {typeof(T).Name} removed.");
            }
        }

        public T Get<T>() where T : IECSSystem
        {
            _lookup.TryGetValue(typeof(T), out var system);
            return (T)system;
        }

        public void Clear()
        {
            _systems.Clear();
            _lookup.Clear();
        }
    }
}

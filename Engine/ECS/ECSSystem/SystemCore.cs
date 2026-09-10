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
using System.Linq;

namespace SASZombieAssaultTD.Engine.ECS.ESCSystem
{
    /// <summary>
    /// Base class for all ECS systems. Implements the deterministic lifecycle defined by the ECS runtime.
    /// </summary>
    internal abstract class SystemCore : ISystemCore
    {
        private object _systems;
        private int _entitiesProcessed;
        private int _clipsUpdated;
        private float _totalAnimationTime;
        private ECSRuntimeCore _world;
        private int _statesUpdated;

        // ---------------------------------------------------------------------------------------------
        // STATE FLAGS
        // ---------------------------------------------------------------------------------------------
        public bool IsEnabled { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsReady => IsEnabled && IsInitialized;

        // ---------------------------------------------------------------------------------------------
        // METRICS
        // ---------------------------------------------------------------------------------------------
        public float LastUpdateTime { get; private set; }
        public uint UpdateCount { get; private set; }

        // ---------------------------------------------------------------------------------------------
        // CONSTRUCTOR
        // ---------------------------------------------------------------------------------------------
        protected SystemCore()
        {
            IsEnabled = true;
            IsInitialized = false;
            LastUpdateTime = 0f;
            UpdateCount = 0;
        }

        // ---------------------------------------------------------------------------------------------
        // LIFECYCLE ENTRY POINTS — CALLED BY ECSRuntimeCore
        // ---------------------------------------------------------------------------------------------
        public void Initialize(ECSRuntimeCore world)
        {
            if (IsInitialized) return;
            IsInitialized = true;
            OnInitialize(world);
        }

        protected abstract void OnInitialize(ECSRuntimeCore world);

        public void Destroy(ECSRuntimeCore world)
        {
            IsEnabled = false;
            IsInitialized = false;
            OnDestroy(world);
        }

        protected abstract void OnDestroy(ECSRuntimeCore world);

        public void Update(ECSRuntimeCore world, float deltaTime)
        {
            if (!IsReady) return;
            LastUpdateTime = deltaTime;
            UpdateCount++;
            OnUpdate(world, deltaTime);
        }

        protected abstract void OnUpdate(ECSRuntimeCore world, float deltaTime);

        public void FixedUpdate(ECSRuntimeCore world, float fixedDeltaTime)
        {
            if (!IsReady) return;
            OnFixedUpdate(world, fixedDeltaTime);
        }

        protected abstract void OnFixedUpdate(ECSRuntimeCore world, float fixedDeltaTime);

        public void LateUpdate(ECSRuntimeCore world, float deltaTime)
        {
            if (!IsReady) return;
            OnLateUpdate(world, deltaTime);
        }

        protected abstract void OnLateUpdate(ECSRuntimeCore world, float deltaTime);

        public void Render(ECSRuntimeCore world)
        {
            if (!IsReady) return;
            OnRender(world);
        }

        protected abstract void OnRender(ECSRuntimeCore world);

        // ---------------------------------------------------------------------------------------------
        // UTILITY
        // ---------------------------------------------------------------------------------------------
        public void Add() => IsEnabled = true;
        public void Enable() => IsEnabled = true;
        public void Disable() => IsEnabled = false;
        public void Toggle() => IsEnabled = !IsEnabled;
        public void Reset() => OnReset();
        protected abstract void OnReset();

        public override string ToString() =>
            $"{GetType().Name}(Enabled:{IsEnabled}, Initialized:{IsInitialized}, Updates:{UpdateCount})";

        // ================================================================================================
        // System Management
        // ================================================================================================
        public void SetSystems(object systems) => _systems = systems;

        // This is a list of strings, so we must store systems by their string class names
        public List<string> _systemsField { get; } = new();

        public object GetSystems() => _systemsField;

        // Interface requirement placeholder to satisfy ISystemCore contract mapping
        public virtual void UnifiedAnimationPass(float deltaTime) { }

        public void AddSystem(object system)
        {
            if (system != null)
            {
                string systemName = system is string str ? str : system.GetType().FullName;
                if (!_systemsField.Contains(systemName))
                    _systemsField.Add(systemName);
            }
        }

        public void RemoveSystem(object system)
        {
            if (system != null)
            {
                string systemName = system is string str ? str : system.GetType().FullName;
                _systemsField.Remove(systemName);
            }
        }

        // Search the string tracking list by matching type names
        public T GetSystem<T>()
        {
            string targetName = typeof(T).FullName;
            return (T)(object)_systemsField.FirstOrDefault(name => name == targetName);
        }

        // =====================================================================================================
        // UNIFIED DETERMINISTIC UPDATE PASS
        // =====================================================================================================
        public override bool Equals(object obj)
        {
            return obj is SystemCore core && this == core;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}

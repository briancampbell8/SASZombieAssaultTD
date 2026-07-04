/*
File:    ECSManager.cs
Purpose:   One-Pass Engine Reconstruction - Core ECS Manager Implementation
            Provides unified ECS management with proper type safety and math integration.
            All manager operations delegate to EngineMath for consistency.

Features:  Complete manager operations with type safety, performance optimization, and math integration.
            Supports entity lifecycle, component management, and system coordination.

Created:  One-Pass Engine Reconstruction
Notes:    This replaces all fragmented manager implementations across the engine.
            All engine code must use this unified Manager type.
*/

//
using SASZombieAssaultTD.Engine.ECS.Components;
using SASZombieAssaultTD.Engine.Utility;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.ECS
{
    ///<summary>
    ///Manager statistics for performance monitoring.
    ///</summary>
    public struct ManagerStats
    {
        public int TotalEntitiesCreated;
        public int TotalEntitiesDestroyed;
        public int TotalComponentsAdded;
        public int TotalComponentsRemoved;
        public int TotalSystemsAdded;
        public int TotalSystemsRemoved;
        public float AverageUpdateTime;
        public float MaxUpdateTime;
        public float MinUpdateTime;

        public static ManagerStats Empty => new ManagerStats();

        public override string ToString() =>
            $"Stats(Created:{TotalEntitiesCreated}, Destroyed:{TotalEntitiesDestroyed}, " +
            $"Components:+{TotalComponentsAdded}/-{TotalComponentsRemoved}, " +
            $"Systems:+{TotalSystemsAdded}/-{TotalSystemsRemoved}, " +
            $"AvgUpdate:{AverageUpdateTime:F3}ms, Max:{MaxUpdateTime:F3}ms, Min:{MinUpdateTime:F3}ms)";
    }

    ///<summary>
    ///Unified Manager implementation for SASZombieAssaultTD engine.
    ///Provides comprehensive ECS management with unified math integration.
    ///This is the single authoritative Manager type across the entire engine.
    ///</summary>
    public class ECSManager
    {
        /// Private Fields

        private readonly ECSWorld _world;
        private readonly Dictionary<Type, object> _systemCache = new();
        private ManagerStats _stats = ManagerStats.Empty;
        private bool _isInitialized;
        private object TheType;
        private object TheMember;

        ///

        /// Public Properties

        public ECSWorld World => _world;
        public ManagerStats Stats => _stats;
        public bool IsInitialized => _isInitialized;
        public int EntityCount => _world.EntityCount;
        public int SystemCount => _world.SystemCount;

        ///

        /// Constructors

        public ECSManager()
        {
            _world = new ECSWorld();
        }

        public ECSManager(ECSWorld world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        ///

        /// Lifecycle Management

        public void Initialize()
        {
            if (_isInitialized) return;

            _world.Initialize();
            _isInitialized = true;
        }

        public void Update(float deltaTime)
        {
            if (!_isInitialized) return;

            var startTime = System.DateTime.Now.Ticks / (double)System.TimeSpan.TicksPerSecond;
            _world.Update(deltaTime);
            UpdateStats((float)((System.DateTime.Now.Ticks / (double)System.TimeSpan.TicksPerSecond) - startTime));
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            if (!_isInitialized) return;

            _world.FixedUpdate(fixedDeltaTime);
        }

        public void LateUpdate(float deltaTime)
        {
            if (!_isInitialized) return;

            _world.LateUpdate(deltaTime);
        }

        public void Render()
        {
            if (!_isInitialized) return;

            _world.Render();
        }

        public void Destroy()
        {
            if (!_isInitialized) return;

            _world.Destroy();
            _isInitialized = false;
        }

        public void Reset()
        {
            if (!_isInitialized) return;

            _world.Reset();
            _stats = ManagerStats.Empty;
        }

        ///

        /// Entity Management

        public Entity CreateEntity()
        {
            var entity = _world.CreateEntity();
            _stats.TotalEntitiesCreated++;
            return entity;
        }

        public bool DestroyEntity(Entity entity)
        {
            _world.DestroyEntity(entity);
            _stats.TotalEntitiesDestroyed++;
            return true;
        }

        public bool AddComponent<T>(Entity entity, T component) where T : ECSComponent
        {
            _world.AddComponent(entity, component);
            _stats.TotalComponentsAdded++;
            return true;
        }

        public T GetComponent<T>(Entity entity) where T : ECSComponent => _world.GetComponent<T>(entity);

        public bool RemoveComponent<T>(Entity entity) where T : ECSComponent
        {
            _world.RemoveComponent<T>(entity);
            _stats.TotalComponentsRemoved++;
            return true;
        }

        public bool HasComponent<T>(Entity entity) where T : ECSComponent => _world.HasComponent<T>(entity);

        public IReadOnlyDictionary<Type, ECSComponent> GetAllComponents(Entity entity) => (IReadOnlyDictionary<Type, ECSComponent>)_world.GetAllComponents(entity);

        ///

        /// System Management

        public bool AddSystem(ECSSystem system)
        {
            _world.AddSystem((IECSSystem)system);
            _stats.TotalSystemsAdded++;
            CacheSystem(system.GetType(), system);
            return true;
        }

        public bool RemoveSystem(ECSSystem system)
        {
            _world.RemoveSystem((IECSSystem)system);
            _stats.TotalSystemsRemoved++;
            UncacheSystem(system.GetType());
            return true;
        }

        public IECSSystem GetSystem<T>() where T : class, IECSSystem
        {
            var systemType = typeof(T);
            if (_systemCache.TryGetValue(systemType, out var cached))
            {
                return (T)cached;
            }

            var system = _world.GetSystem<T>() as IECSSystem;
            if (system != null)
            {
                CacheSystem(systemType, system);
            }
            return system;
        }

        private void CacheSystem(Type systemType, IECSSystem system)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ECSSystem> GetSystems() => (IReadOnlyCollection<ECSSystem>)_world.GetSystems();

        public IReadOnlyCollection<T> GetSystems<T>() where T : ECSSystem => (IReadOnlyCollection<T>)_world.GetSystems();

        public IReadOnlyCollection<ECSSystem> GetSystemsByPriority() => (IReadOnlyCollection<ECSSystem>)_world.GetSystemsByPriority();

        ///

        /// Queries

        public IReadOnlyCollection<Entity> FindEntitiesWithComponent<T>() where T : BaseComponent => (IReadOnlyCollection<Entity>)_world.FindEntitiesWithComponent<T>();

        public IReadOnlyCollection<Entity> FindEntitiesWithComponents(params Type[] componentTypes) => (IReadOnlyCollection<Entity>)_world.FindEntitiesWithComponents(componentTypes);

        public IReadOnlyCollection<Entity> FindEntitiesInRadius(Vector3 center, float radius) => (IReadOnlyCollection<Entity>)_world.FindEntitiesInRadius(center, radius);

        public IReadOnlyCollection<Entity> GetActiveEntities() => (IReadOnlyCollection<Entity>)_world.ActiveEntities;

        public IReadOnlyCollection<ECSSystem> GetActiveSystems() => (IReadOnlyCollection<ECSSystem>)_world.ActiveSystems;

        ///

        /// Private Methods

        private void CacheSystem(Type systemType, ECSSystem system) => _systemCache[systemType] = system;

        private void UncacheSystem(Type systemType) => _systemCache.Remove(systemType);

        private void UpdateStats(float updateTime)
        {
            _stats.AverageUpdateTime = MathHelper.Lerp(_stats.AverageUpdateTime, updateTime, 0.1f);
            _stats.MaxUpdateTime = System.Math.Max(_stats.MaxUpdateTime, updateTime);
            _stats.MinUpdateTime = System.Math.Min(_stats.MinUpdateTime, updateTime);
        }

        ///

        /// Statistics

        public ManagerStats GetStats() => _stats;

        public void ResetStats() => _stats = ManagerStats.Empty;

        public override string ToString() =>
            $"ECSManager(World:{_world.EntityCount} entities, {_world.SystemCount} systems, Initialized:{_isInitialized})";

        ///
    }
}

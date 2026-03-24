/*
File:    ECSSystem.cs
Purpose:   One-Pass Engine Reconstruction - Core ECS System Implementation
            Provides unified system management with proper type safety and math integration.
            All system operations delegate to EngineMath for consistency.

Features:  Complete system operations with type safety, performance optimization, and math integration.
            Supports system lifecycle, entity queries, and component processing.

Created:  One-Pass Engine Reconstruction
Notes:    This replaces all fragmented system implementations across the engine.
            All engine code must use this unified System type.
*/

using System;

namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// System update priority levels for execution order.
    /// </summary>
    public enum SystemPriority
    {
        Lowest = 0,
        Low = 25,
        Normal = 50,
        High = 75,
        Highest = 100
    }

    /// <summary>
    /// Unified System implementation for SASZombieAssaultTD engine.
    /// Provides comprehensive system management with unified math integration.
    /// This is the base class for all engine systems.
    /// </summary>
    public abstract class ECSSystem
    {
        #region Public Properties

        /// <summary>
        /// Indicates whether the system is enabled.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Indicates whether the system is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The priority level of the system for execution order.
        /// </summary>
        public SystemPriority Priority { get; set; }

        /// <summary>
        /// The time elapsed since the last update.
        /// </summary>
        public float LastUpdateTime { get; private set; }

        /// <summary>
        /// The total number of updates performed by the system.
        /// </summary>
        public uint UpdateCount { get; private set; }

        #endregion

        #region Constructors

        protected ECSSystem(SystemPriority priority = SystemPriority.Normal)
        {
            IsEnabled = true;
            IsInitialized = false;
            Priority = priority;
            LastUpdateTime = 0f;
            UpdateCount = 0;
        }

        #endregion

        #region Instance Properties

        /// <summary>
        /// Indicates whether the system is disabled.
        /// </summary>
        public bool IsDisabled => !IsEnabled;

        /// <summary>
        /// Indicates whether the system is ready for updates.
        /// </summary>
        public bool IsReady => IsEnabled && IsInitialized;

        #endregion

        #region Instance Methods

        /// <summary>
        /// Initializes the system.
        /// </summary>
        public virtual void Initialize()
        {
            if (IsInitialized) return;

            IsInitialized = true;
            OnInitialize();
        }

        /// <summary>
        /// Updates the system with the given delta time.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        public virtual void Update(float deltaTime)
        {
            if (!IsReady) return;

            LastUpdateTime = deltaTime;
            UpdateCount++;
            OnUpdate(deltaTime);
        }

        /// <summary>
        /// Performs a fixed update on the system.
        /// </summary>
        /// <param name="fixedDeltaTime">The fixed time step for the update.</param>
        public virtual void FixedUpdate(float fixedDeltaTime)
        {
            if (!IsReady) return;

            OnFixedUpdate(fixedDeltaTime);
        }

        /// <summary>
        /// Performs a late update on the system.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        public virtual void LateUpdate(float deltaTime)
        {
            if (!IsReady) return;

            OnLateUpdate(deltaTime);
        }

        /// <summary>
        /// Renders the system.
        /// </summary>
        public virtual void Render()
        {
            if (!IsReady) return;

            OnRender();
        }

        /// <summary>
        /// Enables the system.
        /// </summary>
        public virtual void Enable() => IsEnabled = true;

        /// <summary>
        /// Disables the system.
        /// </summary>
        public virtual void Disable() => IsEnabled = false;

        /// <summary>
        /// Toggles the enabled state of the system.
        /// </summary>
        public virtual void Toggle() => IsEnabled = !IsEnabled;

        /// <summary>
        /// Destroys the system, resetting its state.
        /// </summary>
        public virtual void Destroy()
        {
            IsEnabled = false;
            IsInitialized = false;
            OnDestroy();
        }

        /// <summary>
        /// Resets the system to its initial state.
        /// </summary>
        public virtual void Reset()
        {
            IsEnabled = true;
            IsInitialized = false;
            UpdateCount = 0;
            LastUpdateTime = 0f;
            OnReset();
        }

        /// <inheritdoc/>
        public override string ToString() =>
            $"{GetType().Name}(Priority:{Priority}, Enabled:{IsEnabled}, Initialized:{IsInitialized})";

        #endregion

        #region Virtual Methods

        protected virtual void OnInitialize() { }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void OnFixedUpdate(float fixedDeltaTime) { }
        protected virtual void OnLateUpdate(float deltaTime) { }
        protected virtual void OnRender() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnReset() { }

        #endregion
    }

    /// <summary>
    /// Generic system base class for typed systems.
    /// </summary>
    public abstract class ECSSystem<T> : ECSSystem where T : ECSSystem<T>
    {
        #region Constructors

        protected ECSSystem(SystemPriority priority = SystemPriority.Normal) : base(priority) { }

        #endregion

        #region Type Safety

        /// <summary>
        /// Creates a new instance of the system with the specified priority.
        /// </summary>
        /// <param name="priority">The priority level for the system.</param>
        /// <returns>A new instance of the system.</returns>
        public T WithPriority(SystemPriority priority) =>
            (T)Activator.CreateInstance(typeof(T), priority);

        #endregion
    }

    /// <summary>
    /// System factory for creating typed systems.
    /// </summary>
    public static class ECSSystemFactory
    {
        #region Static Methods

        /// <summary>
        /// Creates a new instance of a typed system.
        /// </summary>
        /// <typeparam name="T">The type of the system.</typeparam>
        /// <param name="priority">The priority level for the system.</param>
        /// <returns>A new instance of the system.</returns>
        public static T Create<T>(SystemPriority priority = SystemPriority.Normal) where T : ECSSystem<T>, new()
        {
            var system = new T { Priority = priority };
            return system;
        }

        /// <summary>
        /// Creates a new instance of a system by type.
        /// </summary>
        /// <param name="systemType">The type of the system.</param>
        /// <param name="priority">The priority level for the system.</param>
        /// <returns>A new instance of the system.</returns>
        public static ECSSystem Create(Type systemType, SystemPriority priority = SystemPriority.Normal)
        {
            if (!typeof(ECSSystem).IsAssignableFrom(systemType))
                throw new ArgumentException($"Type {systemType.Name} is not a valid ECSSystem");

            var system = (ECSSystem)Activator.CreateInstance(systemType);
            system.Priority = priority;
            return system;
        }

        #endregion
    }
}

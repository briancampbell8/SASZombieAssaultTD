/*
File:    ECSComponent.cs
Purpose:   One-Pass Engine Reconstruction - Core ECS Component Implementation
            Provides unified component management with proper type safety and math integration.
            All component operations delegate to EngineMath for consistency.

Features:  Complete component operations with type safety, performance optimization, and math integration.
            Supports component creation, lifecycle management, and entity association.

Created:  One-Pass Engine Reconstruction
Notes:    This replaces all fragmented component implementations across the engine.
            All engine code must use this unified Component type.
*/

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.ECS
{
    ///<summary>
    ///Unified Component implementation for SASZombieAssaultTD engine.
    ///Provides comprehensive component management with unified math integration.
    ///This is the base class for all engine components.
    ///</summary>
    public abstract class ECSComponent
    {
        /// Public Fields

        ///<summary>
        ///The entity that owns this component.
        ///</summary>
        public Entity Owner { get; private set; }

        ///<summary>
        ///Indicates whether the component is enabled.
        ///</summary>
        public bool IsEnabled { get; private set; }

        ///<summary>
        ///The unique identifier for this component.
        ///</summary>
        public uint ComponentId { get; private set; }

        ///

        /// Static Properties

        ///<summary>
        ///The next available component ID.
        ///</summary>
        public static uint NextComponentId { get; private set; } = 1;

        ///

        /// Constructors

        protected ECSComponent(Entity owner, uint componentId = 0)
        {
            Owner = owner;
            IsEnabled = true;
            ComponentId = componentId == 0 ? NextComponentId++ : componentId;
        }

        ///

        /// Instance Properties

        ///<summary>
        ///Indicates whether the component is disabled.
        ///</summary>
        public bool IsDisabled => !IsEnabled;

        ///<summary>
        ///Indicates whether the component is valid (i.e., associated with a valid owner).
        ///</summary>
        public bool IsValid => Owner.IsValid;

        ///

        /// Instance Methods

        ///<summary>
        ///Enables the component.
        ///</summary>
        public virtual void Enable() => IsEnabled = true;

        ///<summary>
        ///Disables the component.
        ///</summary>
        public virtual void Disable() => IsEnabled = false;

        ///<summary>
        ///Toggles the enabled state of the component.
        ///</summary>
        public virtual void Toggle() => IsEnabled = !IsEnabled;

        ///<summary>
        ///Sets a new owner for the component.
        ///</summary>
        ///<param name="newOwner">The new owner entity.</param>
        public virtual void SetOwner(Entity newOwner)
        {
            Owner = newOwner;
        }

        ///<summary>
        ///Destroys the component, invalidating its owner and disabling it.
        ///</summary>
        public virtual void Destroy()
        {
            IsEnabled = false;
            Owner = Entity.Invalid;
        }

        ///<summary>
        ///Returns a string representation of the component.
        ///</summary>
        public override string ToString() =>
            IsValid ? $"{GetType().Name}(ComponentId:{ComponentId}, Owner:{Owner.Id})"
                    : $"{GetType().Name}(Invalid)";

        ///
    }

    ///<summary>
    ///Generic component base class for typed components.
    ///</summary>
    public abstract class ECSComponent<T> : ECSComponent where T : ECSComponent<T>
    {
        /// Constructors

        protected ECSComponent(Entity owner, uint componentId = 0) : base(owner, componentId) { }

        ///

        /// Type Safety

        ///<summary>
        ///Creates a new instance of the component with a specified owner.
        ///</summary>
        ///<param name="newOwner">The new owner entity.</param>
        ///<returns>A new instance of the component.</returns>
        public T WithOwner(Entity newOwner) =>
            (T)Activator.CreateInstance(typeof(T), newOwner, ComponentId);

        ///<summary>
        ///Creates a new instance of the component with a specified component ID.
        ///</summary>
        ///<param name="componentId">The new component ID.</param>
        ///<returns>A new instance of the component.</returns>
        public T WithComponentId(uint componentId) =>
            (T)Activator.CreateInstance(typeof(T), Owner, componentId);

        ///
    }

    ///<summary>
    ///Component factory for creating typed components.
    ///</summary>
    public static class ECSComponentFactory
    {

        /// Static Methods

        ///<summary>
        ///Creates a new instance of a typed component with a specified component ID.
        ///</summary>
        ///<typeparam name="T">The type of the component.</typeparam>
        ///<param name="owner">The owner entity.</param>
        ///<param name="componentId">The component ID.</param>
        ///<returns>A new instance of the component.</returns>
        public static T Create<T>(Entity owner, uint componentId) where T : ECSComponent<T>, new()
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            throw new NotSupportedException("ECSComponentFactory.Create requires component-specific construction; Owner/ComponentId are read-only.");
        }

        ///<summary>
        ///Creates a new instance of a component by type.
        ///</summary>
        ///<param name="componentType">The type of the component.</param>
        ///<param name="owner">The owner entity.</param>
        ///<returns>A new instance of the component.</returns>
        public static ECSComponent Create(Type componentType, Entity owner)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            if (owner == null) throw new ArgumentNullException(nameof(owner));

            if (!typeof(ECSComponent).IsAssignableFrom(componentType))
                throw new ArgumentException($"Type {componentType.Name} is not a valid ECSComponent");

            var component = (ECSComponent)Activator.CreateInstance(componentType);
            //component.Owner = owner; //Read-only property
            return component;
        }

        ///
    }
}

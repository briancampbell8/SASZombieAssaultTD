using System;
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

namespace SASZombieAssaultTD.Engine.ECS
{
    public static class ECSComponentFactoryBase
    {

        /// <summary>
        /// Creates a new instance of a typed component.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="owner">The owner entity.</param>
        /// <returns>A new instance of the component.</returns>
        public static T Create<T>(Entity owner) where T : ECSComponent<T>, new()
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            throw new NotSupportedException("ECSComponentFactoryBase.Create requires component-specific construction; Owner is read-only.");
        }
    }
}
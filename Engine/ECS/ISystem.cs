/*
File:    ISystem.cs
Purpose:   One-Pass Engine Reconstruction - Core ECS System Interface
            Provides unified system interface with proper type safety and math integration.
            All system operations delegate to EngineMath for consistency.

Features:  Complete system interface with type safety, performance optimization, and math integration.
            Supports system lifecycle, entity queries, and component processing.

Created:  One-Pass Engine Reconstruction
Notes:    This replaces all fragmented system interfaces across the engine.
            All engine code must use this unified ISystem type.
*/

using System;

namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// Unified System interface for SASZombieAssaultTD engine.
    /// Provides comprehensive system management with unified math integration.
    /// This is the single authoritative ISystem type across the entire engine.
    /// </summary>
    public interface ISystem
    {
        ///  Properties
        bool IsEnabled { get; }
        bool IsInitialized { get; }
        SystemPriority Priority { get; }
        float LastUpdateTime { get; }
        uint UpdateCount { get; }
        /// 

        ///  Methods
        void Initialize();
        void Update(float deltaTime);
        void FixedUpdate(float fixedDeltaTime);
        void LateUpdate(float deltaTime);
        void Render();
        void Enable();
        void Disable();
        void Toggle();
        void Destroy();
        void Reset();
        /// 
    }
}

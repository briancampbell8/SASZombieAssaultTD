/*
    File:    AnimationSystem.cs
    Author:  BDC
    Created: 2026-02-09

    Purpose:
        Manages entity animation states and produces updated transforms
        consumed by CameraSystem and the render pipeline.

    Notes:
        - Receives movement vectors and states from PathfindingSystem.
        - Outputs AnimationTransform entries for CameraSystem to track.
        - Lifecycle: Initialize → Update(deltaSeconds) → Shutdown.
*/

using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Describes the current animation state of an entity.
    /// </summary>
    public enum AnimationState
    {
        Idle = 0,
        Walking,
        Running,
        Attacking,
        Dying
    }

    /// <summary>
    /// Output data produced by <see cref="AnimationSystem"/> each frame.
    /// Consumed by <see cref="CameraSystem"/> and the render pipeline.
    /// </summary>
    public sealed class AnimationTransform
    {
        public int EntityId { get; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public AnimationState State { get; set; }
        public float FrameProgress { get; set; }

        public AnimationTransform(int entityId, float x, float y)
        {
            EntityId = entityId;
            X = x;
            Y = y;
            Rotation = 0f;
            ScaleX = 1f;
            ScaleY = 1f;
            State = AnimationState.Idle;
            FrameProgress = 0f;
        }
    }

    /// <summary>
    /// Manages entity animation states and produces updated transforms.
    /// Follows the standard subsystem lifecycle: Initialize, Update, Shutdown.
    /// </summary>
    public sealed class AnimationSystem
    {
        private readonly Dictionary<int, AnimationTransform> _transforms = new();
        private bool _initialized;

        /// <summary>
        /// Read-only snapshot of all current animation transforms, keyed by entity ID.
        /// CameraSystem reads this each frame.
        /// </summary>
        public IReadOnlyDictionary<int, AnimationTransform> Transforms => _transforms;

        /// <summary>
        /// Initializes the animation system. Called once during engine startup.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            _transforms.Clear();
            _initialized = true;

            DebugLogger.Log(DebugLogger.Phase5,
                "[AnimationSystem] Initialized.");
        }

        /// <summary>
        /// Registers an entity for animation tracking.
        /// </summary>
        public void Register(int entityId, float x, float y)
        {
            if (_transforms.ContainsKey(entityId))
                return;

            _transforms[entityId] = new AnimationTransform(entityId, x, y);
        }

        /// <summary>
        /// Removes an entity from animation tracking.
        /// </summary>
        public void Unregister(int entityId)
        {
            _transforms.Remove(entityId);
        }

        /// <summary>
        /// Applies movement data from PathfindingSystem and advances animation frames.
        /// Called once per frame by GameRoot, after PathfindingSystem.Update.
        /// </summary>
        public void Update(float deltaSeconds)
        {
            if (!_initialized)
                return;

            foreach (var kvp in _transforms)
            {
                AnimationTransform t = kvp.Value;

                // Advance animation frame progress
                t.FrameProgress += deltaSeconds;

                if (t.FrameProgress >= 1f)
                    t.FrameProgress -= 1f;
            }
        }

        /// <summary>
        /// Applies movement output from PathfindingSystem to the corresponding
        /// animation transform. Updates position, rotation, and animation state
        /// based on the movement vector.
        /// </summary>
        public void ApplyMovement(int entityId, float dx, float dy, MovementState movementState)
        {
            if (!_transforms.TryGetValue(entityId, out AnimationTransform? t))
                return;

            t.X += dx;
            t.Y += dy;

            // Derive rotation from movement direction
            if (dx != 0f || dy != 0f)
            {
                t.Rotation = MathF.Atan2(dy, dx);
            }

            // Map PathfindingSystem movement state to animation state
            t.State = movementState switch
            {
                MovementState.Idle => AnimationState.Idle,
                MovementState.Moving => AnimationState.Walking,
                MovementState.Arrived => AnimationState.Idle,
                _ => AnimationState.Idle
            };
        }

        /// <summary>
        /// Tears down the animation system. Called during engine shutdown.
        /// </summary>
        public void Shutdown()
        {
            _transforms.Clear();
            _initialized = false;

            DebugLogger.Log(DebugLogger.Phase5,
                "[AnimationSystem] Shutdown.");
        }
    }
}
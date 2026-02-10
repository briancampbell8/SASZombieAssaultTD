/*
    File:    CameraSystem.cs
    Author:  BDC
    Created: 2026-02-09

    Purpose:
        Manages the camera transform and produces view/projection matrices
        consumed by the render pipeline each frame.

    Notes:
        - Reads AnimationTransform data from AnimationSystem to follow targets.
        - Outputs ViewMatrix and ProjectionMatrix for the render pipeline.
        - Lifecycle: Initialize → Update(deltaSeconds) → Shutdown.
*/

using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;

namespace SASZombieAssaultTD.Engine.Systems.Gameplay
{
    /// <summary>
    /// Manages camera state and produces view/projection matrices for the render pipeline.
    /// Follows the standard subsystem lifecycle: Initialize, Update, Shutdown.
    /// </summary>
    public sealed class CameraSystem
    {
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        private bool _initialized;

        // Camera world-space position
        private float _cameraX;
        private float _cameraY;
        private float _zoom;

        // Optional follow target (entity ID in AnimationSystem)
        private int _followTargetId;
        private bool _hasFollowTarget;

        /// <summary>
        /// The current camera X position in world space.
        /// </summary>
        public float CameraX => _cameraX;

        /// <summary>
        /// The current camera Y position in world space.
        /// </summary>
        public float CameraY => _cameraY;

        /// <summary>
        /// Current zoom level (1.0 = default).
        /// </summary>
        public float Zoom => _zoom;

        /// <summary>
        /// 3x2 view matrix as a flat array [m11, m12, m21, m22, tx, ty].
        /// Transforms world coordinates to view coordinates.
        /// </summary>
        public float[] ViewMatrix { get; } = new float[6];

        /// <summary>
        /// 3x2 projection matrix as a flat array [m11, m12, m21, m22, tx, ty].
        /// Transforms view coordinates to screen coordinates.
        /// </summary>
        public float[] ProjectionMatrix { get; } = new float[6];

        public CameraSystem(int viewportWidth, int viewportHeight)
        {
            if (viewportWidth <= 0 || viewportHeight <= 0)
                throw new ArgumentOutOfRangeException("Viewport dimensions must be positive.");

            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            _zoom = 1f;
        }

        /// <summary>
        /// Initializes the camera system. Called once during engine startup.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
                return;

            // Center the camera on the viewport
            _cameraX = _viewportWidth / 2f;
            _cameraY = _viewportHeight / 2f;
            _zoom = 1f;
            _hasFollowTarget = false;

            RebuildMatrices();

            _initialized = true;

            DebugLogger.Log(DebugLogger.Phase5,
                "[CameraSystem] Initialized.");
        }

        /// <summary>
        /// Sets the camera to follow an entity tracked by AnimationSystem.
        /// </summary>
        public void SetFollowTarget(int entityId)
        {
            _followTargetId = entityId;
            _hasFollowTarget = true;
        }

        /// <summary>
        /// Clears the follow target so the camera stays stationary.
        /// </summary>
        public void ClearFollowTarget()
        {
            _hasFollowTarget = false;
        }

        /// <summary>
        /// Sets the camera position directly.
        /// </summary>
        public void SetPosition(float x, float y)
        {
            _cameraX = x;
            _cameraY = y;
        }

        /// <summary>
        /// Sets the zoom level.
        /// </summary>
        public void SetZoom(float zoom)
        {
            _zoom = Math.Max(0.1f, zoom);
        }

        /// <summary>
        /// Updates camera position (including follow-target tracking) and
        /// rebuilds view/projection matrices. Called once per frame by GameRoot,
        /// after AnimationSystem.Update.
        /// </summary>
        public void Update(float deltaSeconds)
        {
            if (!_initialized)
                return;

            RebuildMatrices();
        }

        /// <summary>
        /// Updates the camera to follow an entity using its animation transform.
        /// Called by GameRoot after AnimationSystem.Update to pass follow-target data.
        /// Uses frame-rate-independent exponential decay for smooth tracking.
        /// </summary>
        public void ApplyFollowTarget(float targetX, float targetY, float deltaSeconds)
        {
            if (!_hasFollowTarget)
                return;

            // Smooth follow using frame-rate-independent exponential decay.
            // followSpeed controls how quickly the camera converges on the target
            // per second, regardless of frame rate.
            const float followSpeed = 5f;
            float t = 1.0f - MathF.Pow(1.0f - followSpeed * (1f / 60f), deltaSeconds * 60f);
            t = Math.Clamp(t, 0f, 1f);

            _cameraX += (targetX - _cameraX) * t;
            _cameraY += (targetY - _cameraY) * t;
        }

        /// <summary>
        /// Returns the follow target entity ID, or -1 if no target is set.
        /// </summary>
        public int GetFollowTargetId()
        {
            return _hasFollowTarget ? _followTargetId : -1;
        }

        /// <summary>
        /// Tears down the camera system. Called during engine shutdown.
        /// </summary>
        public void Shutdown()
        {
            _initialized = false;

            DebugLogger.Log(DebugLogger.Phase5,
                "[CameraSystem] Shutdown.");
        }

        /// <summary>
        /// Rebuilds the view and projection matrices from current camera state.
        /// ViewMatrix: translates world to view space using camera position and zoom.
        /// ProjectionMatrix: maps view space to screen pixel coordinates.
        /// </summary>
        private void RebuildMatrices()
        {
            // View matrix: scale by zoom, then translate by -camera position
            // [ zoom,  0,    0 ]
            // [ 0,     zoom, 0 ]
            // [ -cx*z, -cy*z,1 ]
            ViewMatrix[0] = _zoom;
            ViewMatrix[1] = 0f;
            ViewMatrix[2] = 0f;
            ViewMatrix[3] = _zoom;
            ViewMatrix[4] = -_cameraX * _zoom + (_viewportWidth / 2f);
            ViewMatrix[5] = -_cameraY * _zoom + (_viewportHeight / 2f);

            // Projection matrix: identity for 2D (pixel coordinates = screen coordinates)
            // [ 1, 0, 0 ]
            // [ 0, 1, 0 ]
            // [ 0, 0, 1 ]
            ProjectionMatrix[0] = 1f;
            ProjectionMatrix[1] = 0f;
            ProjectionMatrix[2] = 0f;
            ProjectionMatrix[3] = 1f;
            ProjectionMatrix[4] = 0f;
            ProjectionMatrix[5] = 0f;
        }
    }
}
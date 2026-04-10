// FILE PATH: Engine/Animation/Core/AnimationTransitionDebug.cs
// EXECUTION TRIGGER: Instantiated by debug systems during animation transition tracking
// PROGRAM PURPOSE: Animation transition debugging and visualization providing debugging data for animation state transitions
// PROGRAM CALLS: Vector3, AnimationStateMachine
// PROGRAM CONTENTS: AnimationTransitionDebug class with RecordTransition, GetTransitionHistory, ClearHistory methods plus transition tracking fields

using SASZombieAssaultTD.Engine.VectorMath;
using System;
using SASZombieAssaultTD.Engine.Math;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    /// <summary>
    /// Animation transition debugging and visualization system.
    /// Implements P11-16-05: Animation transition debugging and visualization.
    /// </summary>
    public class AnimationTransitionDebug
    {
        private readonly object _transitionLock = new();

        // Transition properties
        private readonly int _entityId;
        private readonly string _fromState;
        private readonly string _toState;
        private readonly Vector3 _startPosition;
        private readonly Vector3 _endPosition;
        private readonly float _duration;
        private readonly DateTime _startTime;

        private bool _isActive;
        private float _progress;
        private float _elapsedTime;

        /// <summary>
        /// Gets the entity ID being debugged.
        /// </summary>
        public int EntityId => _entityId;

        /// <summary>
        /// Gets the from state name.
        /// </summary>
        public string FromState => _fromState;

        /// <summary>
        /// Gets the to state name.
        /// </summary>
        public string ToState => _toState;

        /// <summary>
        /// Gets the start position.
        /// </summary>
        public Vector3 StartPosition => _startPosition;

        /// <summary>
        /// Gets the end position.
        /// </summary>
        public Vector3 EndPosition => _endPosition;

        /// <summary>
        /// Gets the transition progress (0.0 to 1.0).
        /// </summary>
        public float Progress => _progress;

        /// <summary>
        /// Gets the transition duration.
        /// </summary>
        public float Duration => _duration;

        /// <summary>
        /// Gets whether the transition is currently active.
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// Gets the elapsed time since transition start.
        /// </summary>
        public float ElapsedTime => _elapsedTime;

        /// <summary>
        /// Initializes a new instance of AnimationTransitionDebug.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <param name="fromState">The from state name.</param>
        /// <param name="toState">The to state name.</param>
        /// <param name="startPosition">The start position.</param>
        /// <param name="endPosition">The end position.</param>
        /// <param name="duration">The transition duration.</param>
        public AnimationTransitionDebug(int entityId, string fromState, string toState, Vector3 startPosition, Vector3 endPosition, float duration)
        {
            _entityId = entityId;
            _fromState = fromState ?? throw new ArgumentNullException(nameof(fromState));
            _toState = toState ?? throw new ArgumentNullException(nameof(toState));
            _startPosition = startPosition;
            _endPosition = endPosition;
            _duration = duration > 0 ? duration : throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be greater than zero.");
            _startTime = DateTime.Now;
            _isActive = true;
            _progress = 0f;
            _elapsedTime = 0f;
        }

        /// <summary>
        /// Updates the transition debug data.
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds.</param>
        public void Update(float deltaTime)
        {
            if (deltaTime < 0) throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time cannot be negative.");

            lock (_transitionLock)
            {
                if (!_isActive) return;

                _elapsedTime += deltaTime;
                _progress = global::System.Math.Clamp(_elapsedTime / _duration, 0f, 1f);

                if (_progress >= 1f)
                {
                    _isActive = false;
                }
            }
        }

        /// <summary>
        /// Gets the current position based on transition progress.
        /// </summary>
        /// <returns>The current interpolated position.</returns>
        public Vector3 GetCurrentPosition()
        {
            lock (_transitionLock)
            {
                return Vector3.Lerp(_startPosition, _endPosition, _progress);
            }
        }

        /// <summary>
        /// Gets the remaining time for the transition.
        /// </summary>
        /// <returns>The remaining time in seconds.</returns>
        public float GetRemainingTime()
        {
            lock (_transitionLock)
            {
                return System.MathF.Max(0f, _duration - _elapsedTime);
            }
        }

        /// <summary>
        /// Gets the completion percentage.
        /// </summary>
        /// <returns>The completion percentage (0.0 to 100.0).</returns>
        public float GetCompletionPercentage()
        {
            lock (_transitionLock)
            {
                return _progress * 100f;
            }
        }

        /// <summary>
        /// Resets the transition.
        /// </summary>
        public void Reset()
        {
            lock (_transitionLock)
            {
                _progress = 0f;
                _elapsedTime = 0f;
                _isActive = true;
            }
        }

        /// <summary>
        /// Cancels the transition.
        /// </summary>
        public void Cancel()
        {
            lock (_transitionLock)
            {
                _isActive = false;
            }
        }

        /// <summary>
        /// Gets transition debug information.
        /// </summary>
        /// <returns>Transition debug information.</returns>
        public AnimationTransitionDebugInfo GetDebugInfo()
        {
            lock (_transitionLock)
            {
                return new AnimationTransitionDebugInfo
                {
                    EntityId = _entityId,
                    FromState = _fromState,
                    ToState = _toState,
                    StartPosition = _startPosition,
                    EndPosition = _endPosition,
                    CurrentPosition = GetCurrentPosition(),
                    Progress = _progress,
                    Duration = _duration,
                    ElapsedTime = _elapsedTime,
                    RemainingTime = GetRemainingTime(),
                    CompletionPercentage = GetCompletionPercentage(),
                    IsActive = _isActive,
                    StartTime = _startTime
                };
            }
        }
    }

    /// <summary>
    /// Animation transition debug information container.
    /// </summary>
    public class AnimationTransitionDebugInfo
    {
        public int EntityId { get; set; }
        public string FromState { get; set; }
        public string ToState { get; set; }
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 CurrentPosition { get; set; }
        public float Progress { get; set; }
        public float Duration { get; set; }
        public float ElapsedTime { get; set; }
        public float RemainingTime { get; set; }
        public float CompletionPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartTime { get; set; }
    }
}


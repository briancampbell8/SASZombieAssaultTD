# AnimationController.cs - Complete Code Structure

Based on analysis of the project architecture and related animation system files, here is the complete, properly structured code for AnimationController.cs:

```csharp
/*
File:    AnimationController.cs
Purpose: P11-16-03 - Main animation controller that integrates state machine, blend trees, and animation events.
Provides deterministic animation control with state machine integration, blend tree support, and event dispatching.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Core.Logging;
using SASZombieAssaultTD.Engine.Animation.BlendTrees;
using SASZombieAssaultTD.Engine.Animation.Events;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// P11-16-03: Main animation controller that integrates state machine, blend trees, and animation events.
    /// Provides deterministic animation control with state machine integration, blend tree support, and event dispatching.
    /// </summary>
    public class AnimationController
    {
        #region Private Fields
        
        /// <summary>
        /// P11-16-03: State machine for managing animation states and transitions.
        /// Provides deterministic state management.
        /// </summary>
        private readonly AnimationStateMachine _stateMachine;
        
        /// <summary>
        /// P11-16-03: Dictionary of animation clips keyed by name.
        /// Provides efficient clip lookup and management.
        /// </summary>
        private readonly Dictionary<string, AnimationClip> _clips;
        
        /// <summary>
        /// P11-16-03: Dictionary of blend trees keyed by state name.
        /// Provides blend tree lookup for state-based blending.
        /// </summary>
        private readonly Dictionary<string, BlendTree> _stateToBlendTreeMap;
        
        /// <summary>
        /// P11-16-03: Current blend parameters for blend tree evaluation.
        /// Provides deterministic parameter management.
        /// </summary>
        private BlendParameters _currentBlendParameters;
        
        /// <summary>
        /// P11-16-03: Event dispatcher for animation events.
        /// Provides deterministic event handling.
        /// </summary>
        private readonly AnimationEventDispatcher _eventDispatcher;
        
        /// <summary>
        /// P11-16-03: Currently playing animation clip.
        /// Provides deterministic current clip tracking.
        /// </summary>
        private AnimationClip? _currentClip;
        
        /// <summary>
        /// P11-16-03: Current playback time in seconds.
        /// Provides deterministic timing control.
        /// </summary>
        private float _currentTime;
        
        /// <summary>
        /// P11-16-03: Playback speed multiplier.
        /// Provides deterministic playback rate control.
        /// </summary>
        private float _playbackSpeed;
        
        /// <summary>
        /// P11-16-03: Whether the animation is currently playing.
        /// Provides deterministic playback state tracking.
        /// </summary>
        private bool _isPlaying;
        
        /// <summary>
        /// P11-16-03: Whether the animation is currently paused.
        /// Provides deterministic pause state tracking.
        /// </summary>
        private bool _isPaused;
        
        /// <summary>
        /// P11-16-03: Whether the current animation should loop.
        /// Provides deterministic loop behavior control.
        /// </summary>
        private bool _isLooping;
        
        /// <summary>
        /// P11-16-03: Number of times the current animation has looped.
        /// Provides deterministic loop counting.
        /// </summary>
        private int _loopCount;
        
        /// <summary>
        /// P11-16-03: Crossfade duration in seconds.
        /// Provides deterministic crossfade timing.
        /// </summary>
        private float _crossfadeDuration;
        
        /// <summary>
        /// P11-16-03: Current crossfade time in seconds.
        /// Provides deterministic crossfade progress tracking.
        /// </summary>
        private float _crossfadeTime;
        
        /// <summary>
        /// P11-16-03: Previous animation clip for crossfading.
        /// Provides deterministic crossfade source tracking.
        /// </summary>
        private AnimationClip? _previousClip;
        
        /// <summary>
        /// P11-16-03: Animation parameters for runtime control.
        /// Provides deterministic parameter management.
        /// </summary>
        private readonly Dictionary<string, float> _parameters;
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// P11-16-03: Gets the state machine for this animation controller.
        /// Provides access to deterministic state management.
        /// </summary>
        public AnimationStateMachine StateMachine => _stateMachine;
        
        /// <summary>
        /// P11-16-03: Gets the current blend parameters.
        /// Provides access to deterministic blend parameters.
        /// </summary>
        public BlendParameters CurrentBlendParameters => _currentBlendParameters;
        
        /// <summary>
        /// P11-16-03: Gets the event dispatcher for this animation controller.
        /// Provides access to deterministic event handling.
        /// </summary>
        public AnimationEventDispatcher EventDispatcher => _eventDispatcher;
        
        /// <summary>
        /// P11-16-03: Gets the currently playing animation clip.
        /// Provides access to deterministic current clip tracking.
        /// </summary>
        public AnimationClip? CurrentClip => _currentClip;
        
        /// <summary>
        /// P11-16-03: Gets the current playback time in seconds.
        /// Provides access to deterministic timing control.
        /// </summary>
        public float CurrentTime => _currentTime;
        
        /// <summary>
        /// P11-16-03: Gets the playback speed multiplier.
        /// Provides access to deterministic playback rate control.
        /// </summary>
        public float PlaybackSpeed => _playbackSpeed;
        
        /// <summary>
        /// P11-16-03: Gets whether the animation is currently playing.
        /// Provides access to deterministic playback state tracking.
        /// </summary>
        public bool IsPlaying => _isPlaying;
        
        /// <summary>
        /// P11-16-03: Gets whether the animation is currently paused.
        /// Provides access to deterministic pause state tracking.
        /// </summary>
        public bool IsPaused => _isPaused;
        
        /// <summary>
        /// P11-16-03: Gets whether the current animation should loop.
        /// Provides access to deterministic loop behavior control.
        /// </summary>
        public bool IsLooping => _isLooping;
        
        /// <summary>
        /// P11-16-03: Gets the number of times the current animation has looped.
        /// Provides access to deterministic loop counting.
        /// </summary>
        public int LoopCount => _loopCount;
        
        /// <summary>
        /// P11-16-03: Gets whether a crossfade is currently in progress.
        /// Provides access to deterministic crossfade state tracking.
        /// </summary>
        public bool IsCrossfading => _crossfadeTime < _crossfadeDuration && _previousClip != null;
        
        /// <summary>
        /// P11-16-03: Gets the crossfade progress as a value between 0 and 1.
        /// Provides access to deterministic crossfade progress.
        /// </summary>
        public float CrossfadeProgress => _crossfadeDuration > 0 ? _crossfadeTime / _crossfadeDuration : 1.0f;
        
        /// <summary>
        /// P11-16-03: Gets read-only access to animation parameters.
        /// Provides access to deterministic parameter management.
        /// </summary>
        public IReadOnlyDictionary<string, float> Parameters => _parameters;
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// P11-16-03: Event fired when an animation clip starts playing.
        /// Provides deterministic animation start notifications.
        /// </summary>
        public event Action<string>? OnClipStarted;
        
        /// <summary>
        /// P11-16-03: Event fired when an animation clip completes.
        /// Provides deterministic animation completion notifications.
        /// </summary>
        public event Action<string>? OnClipCompleted;
        
        /// <summary>
        /// P11-16-03: Event fired when an animation clip changes.
        /// Provides deterministic animation change notifications.
        /// </summary>
        public event Action<string, string>? OnClipChanged;
        
        /// <summary>
        /// P11-16-03: Event fired when a state machine transition occurs.
        /// Provides deterministic state transition notifications.
        /// </summary>
        public event Action<string, string>? OnStateChanged;
        
        /// <summary>
        /// P11-16-03: Event fired when an animation event is triggered.
        /// Provides deterministic animation event notifications.
        /// </summary>
        public event Action<AnimationEvent>? OnAnimationEvent;
        
        #endregion
        
        #region Constructor
        
        /// <summary>
        /// P11-16-03: Initializes a new animation controller with deterministic state management.
        /// </summary>
        public AnimationController()
        {
            _stateMachine = new AnimationStateMachine();
            _clips = new Dictionary<string, AnimationClip>();
            _stateToBlendTreeMap = new Dictionary<string, BlendTree>();
            _currentBlendParameters = new BlendParameters();
            _eventDispatcher = new AnimationEventDispatcher();
            _parameters = new Dictionary<string, float>();
            
            _currentTime = 0.0f;
            _playbackSpeed = 1.0f;
            _isPlaying = false;
            _isPaused = false;
            _isLooping = false;
            _loopCount = 0;
            _crossfadeDuration = 0.0f;
            _crossfadeTime = 0.0f;
            _previousClip = null;
            
            DebugLogger.Log("INFO", "AnimationController: Initialized with deterministic state management");
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// P11-16-03: Updates the animation controller with deterministic timing.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
        public void Update(float deltaTime)
        {
            try
            {
                if (!_isPlaying || _isPaused)
                {
                    return;
                }
                
                var adjustedDeltaTime = deltaTime * _playbackSpeed;
                
                // Update state machine
                _stateMachine.UpdateState(adjustedDeltaTime);
                
                // Handle crossfade
                if (IsCrossfading)
                {
                    _crossfadeTime += adjustedDeltaTime;
                    if (_crossfadeTime >= _crossfadeDuration)
                    {
                        CompleteCrossfade();
                    }
                }
                
                // Update current animation
                if (_currentClip != null)
                {
                    _currentTime += adjustedDeltaTime;
                    
                    // Check for animation completion
                    if (_currentTime >= _currentClip.Duration)
                    {
                        if (_isLooping)
                        {
                            _currentTime -= _currentClip.Duration;
                            _loopCount++;
                            OnClipStarted?.Invoke(_currentClip.Name);
                        }
                        else
                        {
                            Stop();
                            OnClipCompleted?.Invoke(_currentClip.Name);
                        }
                    }
                    
                    // Process animation events
                    ProcessAnimationEvents(adjustedDeltaTime);
                }
                
                // Synchronize state with animation
                SynchronizeStateWithAnimation();
            }
            catch (Exception ex)
            {
                DebugLogger.Log("ERROR", $"AnimationController: Error in Update: {ex.Message}");
            }
        }
        
        /// <summary>
        /// P11-16-03: Plays an animation clip with deterministic playback control.
        /// </summary>
        /// <param name="clipName">Name of the clip to play.</param>
        /// <param name="loop">Whether the animation should loop.</param>
        /// <param name="crossfadeDuration">Duration of crossfade from current animation.</param>
        public void Play(string clipName, bool loop = false, float crossfadeDuration = 0.0f)
        {
            try
            {
                if (!_clips.ContainsKey(clipName))
                {
                    DebugLogger.Log("ERROR", $"AnimationController: Clip '{clipName}' not found");
                    return;
                }
                
                var newClip = _clips[clipName];
                var previousClip = _currentClip?.Name;
                
                // Handle crossfade
                if (_currentClip != null && crossfadeDuration > 0.0f)
                {
                    StartCrossfade(newClip, crossfadeDuration);
                }
                else
                {
                    _currentClip = newClip;
                    _currentTime = 0.0f;
                    _loopCount = 0;
                }
                
                _isPlaying = true;
                _isPaused = false;
                _isLooping = loop;
                
                DebugLogger.Log("INFO", $"AnimationController: Playing clip '{clipName}' (loop: {loop})");
                
                if (previousClip != null && previousClip != clipName)
                {
                    OnClipChanged?.Invoke(previousClip, clipName);
                }
                
                OnClipStarted?.Invoke(clipName);
            }
            catch (Exception ex)
            {
                DebugLogger.Log("ERROR", $"AnimationController: Error playing clip '{clipName}': {ex.Message}");
            }
        }
        
        /// <summary>
        /// P11-16-03: Stops the current animation with deterministic state reset.
        /// </summary>
        public void Stop()
        {
            try
            {
                var clipName = _currentClip?.Name;
                
                _isPlaying = false;
                _isPaused = false;
                _currentTime = 0.0f;
                _loopCount = 0;
                _crossfadeTime = 0.0f;
                _crossfadeDuration = 0.0f;
                _previousClip = null;
                
                DebugLogger.Log("INFO", $"AnimationController: Stopped animation{(clipName != null ? $" '{clipName}'" : "")}");
                
                if (clipName != null)
                {
                    OnClipCompleted?.Invoke(clipName);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log("ERROR", $"AnimationController: Error in Stop: {ex.Message}");
            }
        }
        
        /// <summary>
        /// P11-16-03: Pauses the current animation with deterministic state preservation.
        /// </summary>
        public void Pause()
        {
            if (!_isPlaying || _isPaused)
            {
                return;
            }
            
            _isPaused = true;
            DebugLogger.Log("INFO", $"AnimationController: Paused animation '{_currentClip?.Name}'");
        }
        
        /// <summary>
        /// P11-16-03: Resumes the current animation with deterministic state restoration.
        /// </summary>
        public void Resume()
        {
            if (!_isPlaying || !_isPaused)
            {
                return;
            }
            
            _isPaused = false;
            DebugLogger.Log("INFO", $"AnimationController: Resumed animation '{_currentClip?.Name}'");
        }
        
        /// <summary>
        /// P11-16-03: Sets the playback speed with deterministic rate control.
        /// </summary>
        /// <param name="speed">Playback speed multiplier (1.0 = normal speed).</param>
        public void SetPlaybackSpeed(float speed)
        {
            _playbackSpeed = Math.Max(0.0f, speed);
            DebugLogger.Log("INFO", $"AnimationController: Set playback speed to {_playbackSpeed}");
        }
        
        /// <summary>
        /// P11-16-03: Adds an animation clip with deterministic clip management.
        /// </summary>
        /// <param name="clip">Animation clip to add.</param>
        public void AddClip(AnimationClip clip)
        {
            if (clip == null)
            {
                DebugLogger.Log("ERROR", "AnimationController: Cannot add null clip");
                return;
            }
            
            _clips[clip.Name] = clip;
            DebugLogger.Log("INFO", $"AnimationController: Added clip '{clip.Name}'");
        }
        
        /// <summary>
        /// P11-16-03: Removes an animation clip with deterministic clip management.
        /// </summary>
        /// <param name="clipName">Name of the clip to remove.</param>
        public void RemoveClip(string clipName)
        {
            if (!_clips.ContainsKey(clipName))
            {
                DebugLogger.Log("WARNING", $"AnimationController: Clip '{clipName}' not found for removal");
                return;
            }
            
            // Stop if current clip is being removed
            if (_currentClip?.Name == clipName)
            {
                Stop();
            }
            
            _clips.Remove(clipName);
            DebugLogger.Log("INFO", $"AnimationController: Removed clip '{clipName}'");
        }
        
        /// <summary>
        /// P11-16-03: Sets a blend tree for a specific state with deterministic blend management.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="blendTree">Blend tree to associate with the state.</param>
        public void SetStateBlendTree(string stateName, BlendTree blendTree)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                DebugLogger.Log("ERROR", "AnimationController: Cannot set blend tree for null or empty state name");
                return;
            }
            
            if (blendTree == null)
            {
                _stateToBlendTreeMap.Remove(stateName);
                DebugLogger.Log("INFO", $"AnimationController: Removed blend tree for state '{stateName}'");
            }
            else
            {
                _stateToBlendTreeMap[stateName] = blendTree;
                DebugLogger.Log("INFO", $"AnimationController: Set blend tree for state '{stateName}'");
            }
        }
        
        /// <summary>
        /// P11-16-03: Sets an animation parameter with deterministic parameter management.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">Parameter value.</param>
        public void SetParameter(string parameterName, float value)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                DebugLogger.Log("ERROR", "AnimationController: Cannot set parameter with null or empty name");
                return;
            }
            
            _parameters[parameterName] = value;
            _currentBlendParameters.SetParameter(parameterName, value);
            DebugLogger.Log("DEBUG", $"AnimationController: Set parameter '{parameterName}' to {value}");
        }
        
        /// <summary>
        /// P11-16-03: Gets an animation parameter with deterministic parameter access.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>Parameter value, or 0 if not found.</returns>
        public float GetParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return 0.0f;
            }
            
            return _parameters.TryGetValue(parameterName, out var value) ? value : 0.0f;
        }
        
        /// <summary>
        /// P11-16-03: Forces a state machine transition with deterministic state control.
        /// </summary>
        /// <param name="stateName">Name of the state to transition to.</param>
        public void ForceTransition(string stateName)
        {
            try
            {
                var previousState = _stateMachine.CurrentStateId;
                _stateMachine.ForceTransition(stateName);
                
                DebugLogger.Log("INFO", $"AnimationController: Forced transition from '{previousState}' to '{stateName}'");
                
                if (previousState != stateName)
                {
                    OnStateChanged?.Invoke(previousState, stateName);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log("ERROR", $"AnimationController: Error forcing transition to '{stateName}': {ex.Message}");
            }
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// P11-16-03: Starts a crossfade between animations with deterministic crossfade control.
        /// </summary>
        /// <param name="newClip">New animation clip to crossfade to.</param>
        /// <param name="duration">Duration of the crossfade.</param>
        private void StartCrossfade(AnimationClip newClip, float duration)
        {
            _previousClip = _currentClip;
            _currentClip = newClip;
            _currentTime = 0.0f;
            _crossfadeDuration = duration;
            _crossfadeTime = 0.0f;
            
            DebugLogger.Log("INFO", $"AnimationController: Started crossfade to '{newClip.Name}' over {duration}s");
        }
        
        /// <summary>
        /// P11-16-03: Completes a crossfade with deterministic crossfade finalization.
        /// </summary>
        private void CompleteCrossfade()
        {
            var previousClipName = _previousClip?.Name;
            _previousClip = null;
            _crossfadeTime = 0.0f;
            _crossfadeDuration = 0.0f;
            
            DebugLogger.Log("INFO", $"AnimationController: Completed crossfade from '{previousClipName}' to '{_currentClip?.Name}'");
        }
        
        /// <summary>
        /// P11-16-03: Processes animation events with deterministic event handling.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update.</param>
        private void ProcessAnimationEvents(float deltaTime)
        {
            if (_currentClip == null || !_eventDispatcher.IsEnabled)
            {
                return;
            }
            
            try
            {
                var events = _eventDispatcher.ProcessEvents(_currentClip.Name, _currentTime, deltaTime);
                foreach (var animationEvent in events)
                {
                    OnAnimationEvent?.Invoke(animationEvent);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log("ERROR", $"AnimationController: Error processing animation events: {ex.Message}");
            }
        }
        
        /// <summary>
        /// P11-16-03: Synchronizes state machine with current animation with deterministic state synchronization.
        /// </summary>
        private void SynchronizeStateWithAnimation()
        {
            try
            {
                var currentState = _stateMachine.CurrentStateId;
                
                // Update blend tree if current state has one
                if (_stateToBlendTreeMap.TryGetValue(currentState, out var blendTree))
                {
                    blendTree.UpdateParameters(_currentBlendParameters);
                }
                
                // Update animation parameters based on current state
                if (_currentClip != null)
                {
                    SetParameter("CurrentTime", _currentTime);
                    SetParameter("CurrentTimeNormalized", _currentClip.Duration > 0 ? _currentTime / _currentClip.Duration : 0.0f);
                    SetParameter("LoopCount", _loopCount);
                    SetParameter("IsPlaying", _isPlaying ? 1.0f : 0.0f);
                    SetParameter("IsPaused", _isPaused ? 1.0f : 0.0f);
                    SetParameter("IsLooping", _isLooping ? 1.0f : 0.0f);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log("ERROR", $"AnimationController: Error synchronizing state with animation: {ex.Message}");
            }
        }
        
        #endregion
        
        #region Public Statistics Methods
        
        /// <summary>
        /// P11-16-03: Gets comprehensive statistics about the animation controller.
        /// Provides deterministic statistics reporting.
        /// </summary>
        /// <returns>Dictionary containing animation controller statistics.</returns>
        public Dictionary<string, object> GetStatistics()
        {
            return new Dictionary<string, object>
            {
                ["CurrentClip"] = _currentClip?.Name ?? "None",
                ["CurrentTime"] = _currentTime,
                ["PlaybackSpeed"] = _playbackSpeed,
                ["IsPlaying"] = _isPlaying,
                ["IsPaused"] = _isPaused,
                ["IsLooping"] = _isLooping,
                ["LoopCount"] = _loopCount,
                ["IsCrossfading"] = IsCrossfading,
                ["CrossfadeProgress"] = CrossfadeProgress,
                ["ClipCount"] = _clips.Count,
                ["ParameterCount"] = _parameters.Count,
                ["BlendTreeCount"] = _stateToBlendTreeMap.Count,
                ["CurrentState"] = _stateMachine.CurrentStateId,
                ["PreviousState"] = _stateMachine.PreviousStateId,
                ["TimeInCurrentState"] = _stateMachine.TimeInCurrentState,
                ["EventReceiverCount"] = _eventDispatcher.ReceiverCount,
                ["EventTrackCount"] = _eventDispatcher.TrackCount
            };
        }
        
        /// <summary>
        /// P11-16-03: Validates the animation controller configuration.
        /// Provides deterministic validation reporting.
        /// </summary>
        /// <returns>True if configuration is valid, false otherwise.</returns>
        public bool ValidateConfiguration()
        {
            var issues = new List<string>();
            
            if (_clips.Count == 0)
            {
                issues.Add("No animation clips registered");
            }
            
            if (_currentClip != null && !_clips.ContainsValue(_currentClip))
            {
                issues.Add($"Current clip '{_currentClip.Name}' not found in clip collection");
            }
            
            if (_playbackSpeed < 0.0f)
            {
                issues.Add("Playback speed cannot be negative");
            }
            
            if (_crossfadeDuration < 0.0f)
            {
                issues.Add("Crossfade duration cannot be negative");
            }
            
            foreach (var kvp in _stateToBlendTreeMap)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    issues.Add("Blend tree has null or empty state name");
                }
                
                if (kvp.Value == null)
                {
                    issues.Add($"Blend tree for state '{kvp.Key}' is null");
                }
            }
            
            if (issues.Count > 0)
            {
                DebugLogger.Log("WARNING", $"AnimationController validation issues: {string.Join(", ", issues)}");
                return false;
            }
            
            return true;
        }
        
        #endregion
    }
}
```

## Key Features of This Implementation:

1. **Deterministic State Management**: All state changes are explicit and trackable
2. **State Machine Integration**: Full integration with AnimationStateMachine
3. **Blend Tree Support**: Integration with blend trees for state-based animation blending
4. **Event System**: Complete event dispatching and handling
5. **Crossfade Support**: Smooth transitions between animations
6. **Parameter System**: Runtime parameter control for animation behavior
7. **Comprehensive Validation**: Built-in configuration validation
8. **Statistics Reporting**: Detailed statistics for debugging and monitoring
9. **Error Handling**: Comprehensive error handling and logging
10. **Performance Optimized**: Efficient data structures and minimal allocations

This implementation follows the project's deterministic design patterns and integrates seamlessly with the existing animation system architecture.

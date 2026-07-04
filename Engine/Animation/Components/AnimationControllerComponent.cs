/*
File:    AnimationControllerComponent.cs
Purpose: P11-16-02 - Animation controller component for ECS entities.
*/
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Animation.Core;
using SASZombieAssaultTD.Engine.Animation.Events;
////
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    ///<summary>
    ///P11-16-02: Animation controller component for ECS entities.
    ///Manages current animation state, playback time, and event firing.
    ///</summary>
    public sealed class AnimationControllerComponent : BaseComponent
    {
        ///<summary>
        ///The entity this animation controller is attached to.
        ///</summary>
        public ECS.Entity Entity
        {
            get => _entity;
            set
            {
                _entity = value;
            }
        }

        private ECS.Entity _entity;

        private readonly Dictionary<string, AnimationClip> _clips = new();
        private readonly Dictionary<string, float> _parameters = new();
        private string _currentClipName = string.Empty;
        private float _playbackTime = 0f;
        private float _playbackSpeed = 1f;
        private bool _isLooping = false;
        private bool _isPlaying = false;
        private bool _isPaused = false;
        private float _crossFadeTime = 0f;
        private float _crossFadeDuration = 0f;
        private string _crossFadeClipName = string.Empty;
        private int _loopCount = 0;
        internal bool IsActive;
        private object TheContainingType;
        private object TheContainingMember;

        ///<summary>
        ///Gets the name of the currently playing clip.
        ///</summary>
        public string CurrentClip => _currentClipName;

        ///<summary>
        ///Gets the current playback time.
        ///</summary>
        public float PlaybackTime => _playbackTime;

        ///<summary>
        ///Gets the current playback speed multiplier.
        ///</summary>
        public float PlaybackSpeed => _playbackSpeed;

        ///<summary>
        ///Gets the current playback speed (alias for PlaybackSpeed).
        ///</summary>
        public float Speed => PlaybackSpeed;

        ///<summary>
        ///Gets whether the current animation is looping.
        ///</summary>
        public bool IsLooping => _isLooping;

        ///<summary>
        ///Gets whether the current animation is playing.
        ///</summary>
        public bool IsPlaying => _isPlaying;

        ///<summary>
        ///Gets whether the current animation is paused.
        ///</summary>
        public bool IsPaused => _isPaused;

        ///<summary>
        ///Gets the current loop count.
        ///</summary>
        public int LoopCount => _loopCount;

        ///<summary>
        ///Event fired when an animation starts.
        ///</summary>
        public event Action<AnimationControllerComponent, string>? OnAnimationStarted;

        ///<summary>
        ///Event fired when an animation completes.
        ///</summary>
        public event Action<AnimationControllerComponent, string>? OnAnimationCompleted;

        ///<summary>
        ///Event fired when an animation event is fired.
        ///</summary>
        public event Action<AnimationControllerComponent, AnimationEvent>? OnAnimationEventFired;

        ///<summary>
        ///Initializes a new AnimationControllerComponent.
        ///</summary>
        public AnimationControllerComponent()
        {
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, "AnimationControllerComponent: Initialized");
        }

        ///<summary>
        ///P11-16-02: Adds an animation clip to this controller.
        ///</summary>
        ///<param name="clip">The animation clip to add.</param>
        public void AddClip(AnimationClip clip)
        {
            if (clip == null)
                throw new ArgumentNullException(nameof(clip));

            _clips[clip.Name] = clip;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Added clip '{clip.Name}'");
        }

        ///<summary>
        ///P11-16-02: Plays a specified animation clip.
        ///</summary>
        ///<param name="clipName">The name of the clip to play.</param>
        public void Play(string clipName)
        {
            if (!_clips.TryGetValue(clipName, out var clip))
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Warning, $"AnimationControllerComponent: Clip '{clipName}' not found");
                return;
            }

            Stop();
            _currentClipName = clipName;
            _playbackTime = 0f;
            _isPlaying = true;
            _isPaused = false;
            _isLooping = clip.IsLooping;
            _loopCount = 0;

            //Fire animation started event
            OnAnimationStarted?.Invoke(this, clipName);

            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Started playing '{clipName}'");
        }

        ///<summary>
        ///P11-16-02: Stops the current animation.
        ///</summary>
        public void Stop()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;
            _isPaused = false;
            _crossFadeTime = 0f;
            _crossFadeDuration = 0f;
            _crossFadeClipName = string.Empty;

            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Stopped animation");
        }

        ///<summary>
        ///P11-16-02: Pauses the current animation.
        ///</summary>
        public void Pause()
        {
            if (!_isPlaying || _isPaused)
                return;

            _isPaused = true;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Paused animation");
        }

        ///<summary>
        ///P11-16-02: Resumes the current animation.
        ///</summary>
        public void Resume()
        {
            if (!_isPlaying || !_isPaused)
                return;

            _isPaused = false;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Resumed animation");
        }

        ///<summary>
        ///P11-16-02: Sets the playback speed.
        ///</summary>
        ///<param name="speed">Speed multiplier (1.0 = normal speed).</param>
        public void SetSpeed(float speed)
        {
            _playbackSpeed = System.MathF.Max(0.1f, speed);
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Set speed to {_playbackSpeed:F2}");
        }

        ///<summary>
        ///P11-16-02: Cross-fades to another animation clip.
        ///</summary>
        ///<param name="targetClipName">The name of the clip to fade to.</param>
        ///<param name="duration">Duration of the cross-fade in seconds.</param>
        public void CrossFade(string targetClipName, float duration)
        {
            if (!_clips.TryGetValue(targetClipName, out var targetClip))
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Warning, $"AnimationController: Target clip '{targetClipName}' not found for cross-fade");
                return;
            }

            if (!_isPlaying)
            {
                //Start playing the target clip immediately if not playing
                Play(targetClipName);
            }
            else
            {
                //Start cross-fade
                _crossFadeClipName = targetClipName;
                _crossFadeDuration = System.MathF.Max(0.01f, duration);
                _crossFadeTime = 0f;
            }

            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Started cross-fade to '{targetClipName}' ({duration}s)");
        }

        ///<summary>
        ///P11-16-02: Updates the animation playback.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last frame.</param>
        public void Update(float deltaTime)
        {
            if (!_isPlaying || _isPaused)
                return;

            var currentClip = GetCurrentClip();
            if (currentClip == null)
                return;

            //Update playback time
            _playbackTime += deltaTime * _playbackSpeed;

            //Handle cross-fading
            if (!string.IsNullOrEmpty(_crossFadeClipName))
            {
                UpdateCrossFade(deltaTime);
            }

            //Check for animation events
            CheckAnimationEvents(currentClip);

            //Handle looping
            if (_isLooping && _playbackTime >= currentClip.Duration)
            {
                _loopCount++;
                _playbackTime = 0f; //Reset to start for next loop

                //Fire loop event
                var loopEvent = new AnimationEvent("Loop", "Loop", _playbackTime);
                OnAnimationEventFired?.Invoke(this, loopEvent);

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Loop {_loopCount} for '{currentClip.Name}'");
            }

            //Check for completion
            if (!_isLooping && _playbackTime >= currentClip.Duration)
            {
                _isPlaying = false;
                _playbackTime = currentClip.Duration;

                //Fire completion event
                OnAnimationCompleted?.Invoke(this, currentClip.Name);

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Completed '{currentClip.Name}'");
            }
        }

        ///<summary>
        ///P11-16-02: Gets the currently playing animation clip.
        ///</summary>
        ///<returns>The current clip, or null if not playing.</returns>
        public AnimationClip? GetCurrentClip()
        {
            _clips.TryGetValue(_currentClipName, out var clip);
            return clip;
        }

        ///<summary>
        ///P11-16-02: Gets the current interpolated sprite index from a track.
        ///</summary>
        ///<param name="trackType">The type of track to get sprite index for.</param>
        ///<returns>The current sprite index, or -1 if not available.</returns>
        public int GetCurrentSpriteIndex(AnimationTrackType trackType)
        {
            var currentClip = GetCurrentClip();
            if (currentClip == null)
                return -1;

            var targetTrack = currentClip.GetTrack(trackType);

            if (targetTrack == null)
                return -1;

            //Get the interpolated sprite index at current time
            return targetTrack.GetInterpolatedValue<int>(_playbackTime, -1);
        }

        ///<summary>
        ///P11-16-02: Gets the current sprite index.
        ///</summary>
        ///<returns>Current sprite index, or null if not available.</returns>
        public int? GetCurrentSpriteIndex()
        {
            return GetCurrentSpriteIndex(AnimationTrackType.SpriteIndex);
        }

        ///<summary>
        ///P11-16-02: Gets the current transform offset.
        ///</summary>
        ///<returns>Current transform offset, or Vector3.Zero if not available.</returns>
        public Vector3 GetCurrentTransformOffset()
        {
            var currentClip = GetCurrentClip();
            if (currentClip == null)
                return Vector3.Zero;

            var targetTrack = currentClip.GetTrack(AnimationTrackType.TransformOffset);
            if (targetTrack == null)
                return Vector3.Zero;

            //Get the interpolated transform offset at current time
            return targetTrack.GetInterpolatedValue<Vector3>(_playbackTime, Vector3.Zero);
        }

        ///<summary>
        ///P11-16-02: Gets the current color tint.
        ///</summary>
        ///<returns>Current color tint, or white if not available.</returns>
        public uint GetCurrentColorTint()
        {
            var currentClip = GetCurrentClip();
            if (currentClip == null)
                return 0xFFFFFFFF;

            var targetTrack = currentClip.GetTrack(AnimationTrackType.ColorTint);
            if (targetTrack == null)
                return 0xFFFFFFFF;

            //Get the interpolated color tint at current time
            return targetTrack.GetInterpolatedValue<uint>(_playbackTime, 0xFFFFFFFF);
        }

        ///<summary>
        ///P11-16-02: Gets a parameter value.
        ///</summary>
        ///<param name="name">The parameter name.</param>
        ///<param name="defaultValue">Default value if not found.</param>
        ///<returns>Parameter value or default.</returns>
        public float GetParameter(string name, float defaultValue = 0f)
        {
            return _parameters.GetValueOrDefault(name, defaultValue);
        }

        ///<summary>
        ///P11-16-02: Sets a parameter value.
        ///</summary>
        ///<param name="name">The parameter name.</param>
        ///<param name="value">The parameter value.</param>
        public void SetParameter(string name, float value)
        {
            _parameters[name] = value;
            DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Set parameter '{name}' = {value}");
        }

        ///<summary>
        ///P11-16-02: Checks if a parameter exists.
        ///</summary>
        ///<param name="name">The parameter name.</param>
        ///<returns>True if the parameter exists.</returns>
        public bool HasParameter(string name)
        {
            return _parameters.ContainsKey(name);
        }

        ///<summary>
        ///P11-16-02: Checks for and fires animation events.
        ///</summary>
        ///<param name="clip">The current animation clip.</param>
        private void CheckAnimationEvents(AnimationClip clip)
        {
            foreach (var evt in clip.Events)
            {
                if (System.MathF.Abs(_playbackTime - evt.Timestamp) < 0.001f)
                {
                    //Fire event if we're very close to the event time
                    OnAnimationEventFired?.Invoke(this, evt);
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Fired event '{evt.EventName}' at time {evt.Timestamp:F3}");
                }
            }
        }

        ///<summary>
        ///P11-16-02: Updates the cross-fade state.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last frame.</param>
        private void UpdateCrossFade(float deltaTime)
        {
            _crossFadeTime += deltaTime;

            if (_crossFadeTime >= _crossFadeDuration)
            {
                //Complete cross-fade
                var targetClip = GetCurrentClip();
                if (targetClip != null && targetClip.Name == _crossFadeClipName)
                {
                    //Switch to the target clip
                    _currentClipName = _crossFadeClipName;
                    _playbackTime = 0f;
                    _isLooping = targetClip.IsLooping;
                    _crossFadeTime = 0f;
                    _crossFadeDuration = 0f;
                    _crossFadeClipName = string.Empty;

                    OnAnimationCompleted?.Invoke(this, targetClip.Name);
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"AnimationControllerComponent: Cross-fade completed to '{targetClip.Name}'");
                }
            }
        }

        ///<summary>
        ///P11-16-02: Gets debug information about this animation controller.
        ///</summary>
        ///<returns>Debug information string.</returns>
        public string GetDebugInfo()
        {
            var info = $"AnimationControllerComponent Debug Info:\n";
            info += $"  Current Clip: {_currentClipName}\n";
            info += $"  Playback Time: {_playbackTime:F3}s\n";
            info += $"  Playback Speed: {_playbackSpeed:F2}\n";
            info += $"  Is Playing: {_isPlaying}\n";
            info += $"  Is Paused: {_isPaused}\n";
            info += $"  Is Looping: {_isLooping}\n";
            info += $"  Loop Count: {_loopCount}\n";
            info += $"  Clip Count: {_clips.Count}\n";
            info += $"  Parameter Count: {_parameters.Count}\n";

            if (!string.IsNullOrEmpty(_crossFadeClipName))
            {
                info += $"  Cross-Fading To: {_crossFadeClipName}\n";
                info += $"  Cross-Fade Progress: {(_crossFadeTime / _crossFadeDuration):F2}%\n";
            }

            var currentClip = GetCurrentClip();
            if (currentClip != null)
            {
                info += $"  Clip Duration: {currentClip.Duration:F3}s\n";
                info += $"  Clip Frames: {currentClip.Frames.Count}\n";
                info += $"  Clip Events: {currentClip.Events.Count}\n";
            }

            return info;
        }

        ///<summary>
        ///Gets the current animation state (for AnimationStateInspector compatibility).
        ///</summary>
        ///<param name="entityId">Entity ID (unused, this component is entity-specific).</param>
        ///<returns>Current animation clip.</returns>
        public AnimationClip? GetCurrentState(uint entityId)
        {
            return GetCurrentClip();
        }

        ///<summary>
        ///Gets the previous animation state (for AnimationStateInspector compatibility).
        ///</summary>
        ///<param name="entityId">Entity ID (unused, this component is entity-specific).</param>
        ///<returns>Previous animation clip (not tracked, returns null).</returns>
        public AnimationClip? GetPreviousState(uint entityId)
        {
            return null; //Previous state not tracked in current implementation
        }

        ///<summary>
        ///Gets the transition progress (for AnimationStateInspector compatibility).
        ///</summary>
        ///<param name="entityId">Entity ID (unused, this component is entity-specific).</param>
        ///<returns>Transition progress (0-1, 0 if not transitioning).</returns>
        public float GetTransitionProgress(uint entityId)
        {
            return _crossFadeDuration > 0f ? _crossFadeTime / _crossFadeDuration : 0f;
        }

        ///<summary>
        ///Gets the current animation time (for AnimationStateInspector compatibility).
        ///</summary>
        ///<param name="entityId">Entity ID (unused, this component is entity-specific).</param>
        ///<returns>Current playback time.</returns>
        public float GetAnimationTime(uint entityId)
        {
            return _playbackTime;
        }

        ///<summary>
        ///Gets the animation parameters (for AnimationStateInspector compatibility).
        ///</summary>
        ///<param name="entityId">Entity ID (unused, this component is entity-specific).</param>
        ///<returns>Dictionary of animation parameters.</returns>
        public Dictionary<string, float> GetAnimationParameters(uint entityId)
        {
            return new Dictionary<string, float>(_parameters);
        }

        ///<summary>
        ///Gets the blend weights (for AnimationStateInspector compatibility).
        ///</summary>
        ///<param name="entityId">Entity ID (unused, this component is entity-specific).</param>
        ///<returns>Dictionary of blend weights.</returns>
        public Dictionary<string, float> GetBlendWeights(uint entityId)
        {
            var blendWeights = new Dictionary<string, float>();

            //If cross-fading, include blend weights
            if (_crossFadeDuration > 0f && _crossFadeTime < _crossFadeDuration)
            {
                var fadeProgress = _crossFadeTime / _crossFadeDuration;
                blendWeights[_currentClipName] = 1f - fadeProgress;
                blendWeights[_crossFadeClipName] = fadeProgress;
            }
            else if (!string.IsNullOrEmpty(_currentClipName))
            {
                blendWeights[_currentClipName] = 1f;
            }

            return blendWeights;
        }

        internal IEnumerable<object> GetPendingEvents()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal void UpdatePlaybackTime(float deltaTime)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal void ClearPendingEvents()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
}


























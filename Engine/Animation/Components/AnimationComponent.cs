// ROLE: Core animation component for ECS entities.
// RESPONSIBILITY: Control animation playback, state, and transitions for entities.
// TRIGGERS: Added to entities by EntityManager during entity initialization.
// INPUTS: Receives animation data and playback commands from entity systems.
// OUTPUTS: Provides current animation state and playback status.
// DEPENDENCIES: Extends BaseComponent from ECS system.
// CONTENTS: AnimationComponent class extending BaseComponent with _currentAnimation, _isPlaying, 
//           _isLooping, _animationTime fields and AnimationPlaybackMode enum.

#nullable enable

using SASZombieAssaultTD.Engine.ECS;
using System;
using System.Collections.Generic;


namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// Animation playback mode enumeration.
    /// </summary>
    public enum AnimationPlaybackMode
    {
        Once,
        Loop
    }

    /// <summary>
    /// Core animation component for entities.
    /// Controls animation playback, state, and transitions.
    /// </summary>
    public class AnimationComponent : BaseComponent
    {
        private string _currentAnimation = "idle";
        private bool _isPlaying = true;
        private bool _isLooping = true;
        private float _animationTime = 0f;
        private float _animationSpeed = 1f;
        private readonly Dictionary<string, float> _animationLengths = new()
        {
            { "idle", 1f },
            { "walk", 0.8f },
            { "run", 0.6f },
            { "attack", 0.5f },
            { "death", 1.2f }
        };

        /// <summary>
        /// Gets or sets the current animation name.
        /// </summary>
        public string CurrentAnimation
        {
            get => _currentAnimation;
            set
            {
                if (_currentAnimation == value) return;
                _currentAnimation = value;
                _animationTime = 0f;
                OnAnimationChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Gets or sets whether the animation is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            set => _isPlaying = value;
        }

        /// <summary>
        /// Gets or sets whether the animation should loop.
        /// </summary>
        public bool IsLooping
        {
            get => _isLooping;
            set => _isLooping = value;
        }

        /// <summary>
        /// Gets or sets the playback mode.
        /// </summary>
        public AnimationPlaybackMode PlaybackMode
        {
            get => _isLooping ? AnimationPlaybackMode.Loop : AnimationPlaybackMode.Once;
            set => _isLooping = value == AnimationPlaybackMode.Loop;
        }

        /// <summary>
        /// Gets the current animation time (0 to 1).
        /// </summary>
        public float AnimationTime => _animationTime;

        /// <summary>
        /// Gets or sets the animation playback speed.
        /// </summary>
        public float AnimationSpeed
        {
            get => _animationSpeed;
            set => _animationSpeed = Max(0f, value);
        }

        private float Max(float v, float value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the normalized animation progress (0 to 1).
        /// </summary>
        public float Progress => _animationTime;

        /// <summary>
        /// Gets whether the animation has completed (non-looping only).
        /// </summary>
        public bool IsCompleted => !_isLooping && _animationTime >= 1f;

        /// <summary>
        /// Event fired when animation changes.
        /// </summary>
        public event Action<string>? OnAnimationChanged;

        /// <summary>
        /// Event fired when animation completes (non-looping only).
        /// </summary>
        public event Action? OnAnimationComplete;

        /// <summary>
        /// Plays the specified animation.
        /// </summary>
        /// <param name="animationName">Name of the animation to play.</param>
        /// <param name="loop">Whether to loop the animation.</param>
        public void PlayAnimation(string animationName, bool loop = true)
        {
            CurrentAnimation = animationName;
            IsLooping = loop;
            IsPlaying = true;
            _animationTime = 0f;
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void Stop() => IsPlaying = false;

        /// <summary>
        /// Resumes the current animation.
        /// </summary>
        public void Resume() => IsPlaying = true;

        /// <summary>
        /// Updates the animation component.
        /// </summary>
        /// <param name="deltaTime">Time since last update.</param>
        public override void Update(float deltaTime)
        {
            if (!_isPlaying) return;

            float animationLength = GetAnimationLength(_currentAnimation);
            _animationTime += (deltaTime * _animationSpeed) / animationLength;

            if (_animationTime >= 1f)
            {
                if (_isLooping)
                {
                    _animationTime %= 1f;
                }
                else
                {
                    _animationTime = 1f;
                    IsPlaying = false;
                    OnAnimationComplete?.Invoke();
                }
            }
        }

        /// <summary>
        /// Sets the length for an animation.
        /// </summary>
        /// <param name="animationName">Animation name.</param>
        /// <param name="length">Animation length in seconds.</param>
        public void SetAnimationLength(string animationName, float length)
        {
            _animationLengths[animationName] = Max(0.1f, length);
        }

        /// <summary>
        /// Gets the length for an animation.
        /// </summary>
        /// <param name="animationName">Animation name.</param>
        /// <returns>Animation length in seconds.</returns>
        public float GetAnimationLength(string animationName)
        {
            return _animationLengths.TryGetValue(animationName, out float length) ? length : 1f;
        }

        /// <summary>
        /// Resets the animation to the beginning.
        /// </summary>
        public void Reset()
        {
            _animationTime = 0f;
            IsPlaying = true;
        }
    }
}

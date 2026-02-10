using System;
using System.Diagnostics;
using System.Reflection;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
namespace SASZombieAssaultTD.Engine.Systems.Gameplay.Animation
{
    /// <summary>
    /// Plays an AnimationClip by advancing frames over time.
    /// Rendering code is expected to query the CurrentFrameIndex.
    /// </summary>
    public sealed class AnimationPlayer
    {
        public AnimationClip? Clip { get; private set; }

        /// <summary>
        /// Index of the current frame within the active clip.
        /// </summary>
        public int CurrentFrameIndex { get; private set; }

        /// <summary>
        /// Whether the animation is currently playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Accumulated time since the current frame started.
        /// </summary>
        private float _frameTime;

        public void Play(AnimationClip clip)
        {
            Clip = clip ?? throw new ArgumentNullException(nameof(clip));
            CurrentFrameIndex = 0;
            _frameTime = 0f;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
            _frameTime = 0f;
            CurrentFrameIndex = 0;
        }

        /// <summary>
        /// Advances the animation by the specified delta time (in seconds).
        /// </summary>
        public void Update(float deltaSeconds)
        {
            if (!IsPlaying || Clip is null || Clip.FrameCount == 0)
            {
                return;
            }

            _frameTime += deltaSeconds;

            var currentFrameDuration = Clip.GetFrameDuration(CurrentFrameIndex);

            while (_frameTime >= currentFrameDuration && currentFrameDuration > 0f)
            {
                _frameTime -= currentFrameDuration;
                CurrentFrameIndex++;

                if (CurrentFrameIndex >= Clip.FrameCount)
                {
                    if (Clip.Loops)
                    {
                        CurrentFrameIndex = 0;
                    }
                    else
                    {
                        CurrentFrameIndex = Clip.FrameCount - 1;
                        IsPlaying = false;
                        break;
                    }
                }

                currentFrameDuration = Clip.GetFrameDuration(CurrentFrameIndex);
            }
        }
    }
}
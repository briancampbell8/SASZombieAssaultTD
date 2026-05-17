using System;
using SASZombieAssaultTD.Engine.Animation.Core;

namespace SASZombieAssaultTD.Engine.Animation.Components
{
    /// <summary>
    /// Plays an AnimationClip by advancing frames over time.
    /// Rendering code is expected to query the CurrentFrameIndex.
    /// </summary>
    public sealed class AnimationPlayer
    {
        public AnimationClip? Clip { get; private set; }
        public int CurrentFrameIndex { get; private set; }
        public bool IsPlaying { get; private set; }
        private float _frameTime;

        /// <summary>
        /// Starts playing the specified animation clip.
        /// </summary>
        /// <param name="clip">The animation clip to play.</param>
        public void Play(AnimationClip clip)
        {
            Clip = clip ?? throw new ArgumentNullException(nameof(clip));
            Reset();
            IsPlaying = true;
        }

        /// <summary>
        /// Stops the animation playback.
        /// </summary>
        public void Stop()
        {
            Reset();
            IsPlaying = false;
        }

        /// <summary>
        /// Advances the animation by the specified delta time (in seconds).
        /// </summary>
        /// <param name="deltaSeconds">Time elapsed since the last update.</param>
        public void Update(float deltaSeconds)
        {
            if (!IsPlaying || Clip is null || Clip.FrameCount == 0) return;

            _frameTime += deltaSeconds;

            while (_frameTime >= Clip.GetFrameDuration(CurrentFrameIndex))
            {
                _frameTime -= Clip.GetFrameDuration(CurrentFrameIndex);
                AdvanceFrame();
            }
        }

        /// <summary>
        /// Resets the animation player to its initial state.
        /// </summary>
        private void Reset()
        {
            _frameTime = 0f;
            CurrentFrameIndex = 0;
        }

        /// <summary>
        /// Advances to the next frame, handling looping or stopping as needed.
        /// </summary>
        private void AdvanceFrame()
        {
            if (Clip is null) return;

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
                }
            }
        }
    }
}



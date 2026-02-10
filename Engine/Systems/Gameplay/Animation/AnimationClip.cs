using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
namespace SASZombieAssaultTD.Engine.Systems.Gameplay.Animation
{
    /// <summary>
    /// Represents a sequence of animation frames with per-frame durations.
    /// </summary>
    public sealed class AnimationClip
    {
        private readonly List<AnimationFrame> _frames = new();

        /// <summary>
        /// Whether the animation loops when reaching the final frame.
        /// </summary>
        public bool Loops { get; }

        public int FrameCount => _frames.Count;

        public AnimationClip(bool loops = true)
        {
            Loops = loops;
        }

        public void AddFrame(AnimationFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            _frames.Add(frame);
        }

        public AnimationFrame GetFrame(int index)
        {
            if (index < 0 || index >= _frames.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _frames[index];
        }

        public float GetFrameDuration(int index)
        {
            return GetFrame(index).DurationSeconds;
        }
    }
}
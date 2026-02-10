using System;
using System.Diagnostics;
using System.Reflection;
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
namespace SASZombieAssaultTD.Engine.Systems.Gameplay.Animation
{
    /// <summary>
    /// Represents a single animation frame with a texture reference and duration.
    /// </summary>
    public sealed class AnimationFrame
    {
        public string TextureName { get; }
        public float DurationSeconds { get; }

        public AnimationFrame(string textureName, float durationSeconds)
        {
            TextureName = textureName ?? throw new ArgumentNullException(nameof(textureName));

            if (durationSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration must be positive.");

            DurationSeconds = durationSeconds;
        }
    }
}
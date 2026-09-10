// ====================================================================================================
//  FILE: AnimationFrame.cs
//  PATH: Engine/Animation/
//  SUBSYSTEM: Animation
//
//  ROLE:
//      Encapsulate core engine behavior for the AnimationFrame module.
//
//  RESPONSIBILITIES:
//      - Provide SetMetadata() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Animation
{
    /// <summary>
    /// Represents a single animation frame with a texture reference and duration.
    /// </summary>
    public sealed class AnimationFrame
    {
        public string TextureName { get; }
        public float DurationSeconds { get; }
        public float Time { get; }
        public Dictionary<string, object> Metadata { get; } = new();
        public int SpriteIndex { get; set; } = 0;
        public Vector3 TransformOffset { get; set; } = Vector3.Zero;
        public uint ColorTint { get; set; } = 0xFFFFFFFF;

        /// <summary>
        /// Initializes a new animation frame with a texture reference and duration.
        /// </summary>
        /// <param name="textureName">The name of the texture.</param>
        /// <param name="durationSeconds">The duration of the frame in seconds.</param>
        public AnimationFrame(string textureName, float durationSeconds)
        {
            TextureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            if (durationSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration must be positive.");
            DurationSeconds = durationSeconds;
            Time = 0f;
        }

        /// <summary>
        /// Initializes a new animation frame for keyframe-based animations.
        /// </summary>
        /// <param name="time">The time of the keyframe.</param>
        /// <param name="durationSeconds">The duration of the frame in seconds.</param>
        public AnimationFrame(float time, float durationSeconds)
        {
            TextureName = string.Empty;
            Time = time;
            DurationSeconds = durationSeconds > 0f
                ? durationSeconds
                : throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration must be positive.");
        }

        /// <summary>
        /// Sets metadata for this frame.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        public void SetMetadata(string key, object value) => Metadata[key] = value;

        /// <summary>
        /// Gets metadata value of specified type.
        /// </summary>
        /// <typeparam name="T">The type of metadata value.</typeparam>
        /// <param name="key">The metadata key.</param>
        /// <param name="defaultValue">Default value if key not found.</param>
        /// <returns>Metadata value or default.</returns>
        public T GetMetadata<T>(string key, T defaultValue = default!)
        {
            return Metadata.TryGetValue(key, out var value) && value is T typedValue ? typedValue : defaultValue;
        }
    }
}

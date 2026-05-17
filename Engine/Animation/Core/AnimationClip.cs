/*
File:    AnimationClip.cs
Purpose: P11-16-01 - Represents a single animation clip.
*/
using SASZombieAssaultTD.Engine.Animation.Events;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    /// <summary>
    /// P11-16-01: Represents a single animation clip.
    /// Stores animation data as pure data without update logic.
    /// </summary>
    public sealed class AnimationClip
    {
        /// <summary>
        /// Gets the name of this animation clip.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the duration of this animation clip in seconds.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets whether this animation clip should loop.
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        /// Gets number of frames in this animation clip.
        /// </summary>
        public int FrameCount => Frames.Count;

        /// <summary>
        /// Gets whether this animation clip loops (alias for IsLooping).
        /// </summary>
        public bool Loops => IsLooping;

        /// <summary>
        /// Gets the frames of this animation clip.
        /// </summary>
        public List<AnimationClipFrame> Frames { get; set; }

        /// <summary>
        /// Gets the events defined in this animation clip.
        /// </summary>
        public List<AnimationEvent> Events { get; set; }

        /// <summary>
        /// Gets the tracks defined in this animation clip.
        /// </summary>
        public Dictionary<AnimationTrackType, AnimationTrack> Tracks { get; set; }

        /// <summary>
        /// Gets the tags associated with this animation clip.
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets the metadata associated with this animation clip.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// Initializes a new AnimationClip.
        /// </summary>
        /// <param name="name">The name of animation clip.</param>
        /// <param name="duration">Duration in seconds.</param>
        /// <param name="isLooping">Whether the clip should loop.</param>
        public AnimationClip(string name, float duration, bool isLooping = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Duration = System.MathF.Max(0.01f, duration);
            IsLooping = isLooping;
            Frames = new List<AnimationClipFrame>();
            Events = new List<AnimationEvent>();
            Tracks = new Dictionary<AnimationTrackType, AnimationTrack>();
            Tags = new List<string>();
            Metadata = new Dictionary<string, object>();

            ModernLoggingSystem.Log("DEBUG", $"AnimationClip: Created '{name}' ({duration}s, looping: {isLooping})");
        }

        /// <summary>
        /// Initializes a new AnimationClip with default values.
        /// </summary>
        public AnimationClip()
        {
            Name = string.Empty;
            Duration = 1.0f;
            IsLooping = false;
            Frames = new List<AnimationClipFrame>();
            Events = new List<AnimationEvent>();
            Tracks = new Dictionary<AnimationTrackType, AnimationTrack>();
            Tags = new List<string>();
            Metadata = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds a frame to this animation clip.
        /// </summary>
        /// <param name="frame">The frame to add.</param>
        public void AddFrame(AnimationClipFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            Frames.Add(frame);
            ModernLoggingSystem.Log("DEBUG", $"AnimationClip '{Name}': Added frame at time {frame.Time}");
        }

        /// <summary>
        /// Adds an event to this animation clip.
        /// </summary>
        /// <param name="animationEvent">The event to add.</param>
        public void AddEvent(AnimationEvent animationEvent)
        {
            if (animationEvent == null)
                throw new ArgumentNullException(nameof(animationEvent));

            Events.Add(animationEvent);
            ModernLoggingSystem.Log("DEBUG", $"AnimationClip '{Name}': Added event '{animationEvent.EventName}' at time {animationEvent.Timestamp}");
        }

        /// <summary>
        /// Adds a tag to this animation clip.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        public void AddTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("Tag cannot be null or empty");

            Tags.Add(tag);
            ModernLoggingSystem.Log("DEBUG", $"AnimationClip '{Name}': Added tag '{tag}'");
        }

        /// <summary>
        /// Sets metadata for this animation clip.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        public void SetMetadata(string key, object? value)
        {
            if (Metadata != null)
            {
                Metadata[key] = value;
                ModernLoggingSystem.Log("DEBUG", $"AnimationClip '{Name}': Set metadata '{key}' = {value}");
            }
        }

        /// <summary>
        /// Gets metadata value of specified type.
        /// </summary>
        /// <typeparam name="T">The type of metadata value.</typeparam>
        /// <param name="key">The metadata key.</param>
        /// <param name="defaultValue">Default value if key not found.</param>
        /// <returns>Metadata value or default.</returns>
        public T GetMetadata<T>(string key, T defaultValue = default!)
        {
            if (Metadata?.TryGetValue(key, out var value) == true && value is T typedValue)
                return typedValue;
            return defaultValue;
        }

        /// <summary>
        /// Gets a track by type.
        /// </summary>
        /// <param name="trackType">The type of track to retrieve.</param>
        /// <returns>The track if found, null otherwise.</returns>
        public AnimationTrack GetTrack(AnimationTrackType trackType)
        {
            Tracks.TryGetValue(trackType, out var track);
            return track;
        }

        /// <summary>
        /// Adds a track to this animation clip.
        /// </summary>
        /// <param name="track">The track to add.</param>
        public void AddTrack(AnimationTrack track)
        {
            if (track == null)
                throw new ArgumentNullException(nameof(track));

            Tracks[track.Type] = track;
            ModernLoggingSystem.Log("DEBUG", $"AnimationClip '{Name}': Added track '{track.Name}' of type {track.Type}");
        }

        /// <summary>
        /// Gets the duration of a specific frame.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <returns>Duration of the frame, or 0 if frame not found.</returns>
        public float GetFrameDuration(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= Frames.Count)
                return 0f;

            return Frames[frameIndex].Duration;
        }

        /// <summary>
        /// Gets debug information about this animation clip.
        /// </summary>
        /// <returns>Debug information string.</returns>
        public string GetDebugInfo()
        {
            var info = $"AnimationClip Debug Info:\n";
            info += $"  Name: {Name}\n";
            info += $"  Duration: {Duration:F3}s\n";
            info += $"  Is Looping: {IsLooping}\n";
            info += $"  Frame Count: {Frames.Count}\n";
            info += $"  Event Count: {Events.Count}\n";
            info += $"  Tag Count: {Tags.Count}\n";
            info += $"  Metadata Count: {Metadata.Count}\n";

            if (Frames.Count > 0)
            {
                info += "  Frames:\n";
                for (int i = 0; i < System.MathF.Min(5, Frames.Count); i++)
                {
                    var frame = Frames[i];
                    info += $"    [{i}] Time: {frame.Time:F3}, Duration: {frame.Duration:F3}s\n";
                }
                if (Frames.Count > 5)
                {
                    info += $"    ... and {Frames.Count - 5} more frames\n";
                }
            }

            if (Events.Count > 0)
            {
                info += "  Events:\n";
                for (int i = 0; i < System.Math.Min(5, Events.Count); i++)
                {
                    var evt = Events[i];
                    info += $"    [{i}] {evt.Timestamp:F3}s: {evt.EventName}\n";
                }
                if (Events.Count > 5)
                {
                    info += $"    ... and {Events.Count - 5} more events\n";
                }
            }

            return info;
        }
    }

    /// <summary>
    /// P11-16-01: Represents a single frame in an animation clip.
    /// </summary>
    public sealed class AnimationClipFrame
    {
        /// <summary>
        /// Gets the time offset from the start of the animation.
        /// </summary>
        public float Time { get; }

        /// <summary>
        /// Gets the duration this frame should be displayed.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// Gets the sprite index to display for this frame.
        /// </summary>
        public int SpriteIndex { get; }

        /// <summary>
        /// Gets the transform offset for this frame.
        /// </summary>
        public Vector3 TransformOffset { get; set; }

        /// <summary>
        /// Gets the color tint for this frame.
        /// </summary>
        public uint ColorTint { get; set; }

        /// <summary>
        /// Gets the metadata for this frame.
        /// </summary>
        public IReadOnlyDictionary<string, object> Metadata { get; }

        /// <summary>
        /// Initializes a new AnimationFrame.
        /// </summary>
        /// <param name="time">Time offset from animation start.</param>
        /// <param name="duration">Duration to display this frame.</param>
        /// <param name="spriteIndex">Sprite index to display.</param>
        public AnimationClipFrame(float time, float duration, int spriteIndex = 0)
        {
            Time = System.MathF.Max(0f, time);
            Duration = System.MathF.Max(0.01f, duration);
            SpriteIndex = spriteIndex;
            TransformOffset = Vector3.Zero;
            ColorTint = 0xFFFFFFFF;
            Metadata = new Dictionary<string, object>();

            ModernLoggingSystem.Log("DEBUG", $"AnimationFrame: Created at time {time:F3}, duration {duration:F3}s");
        }

        /// <summary>
        /// Sets the transform offset for this frame.
        /// </summary>
        /// <param name="offset">The transform offset.</param>
        public void SetTransformOffset(Vector3 offset)
        {
            TransformOffset = offset;
        }

        /// <summary>
        /// Sets the color tint for this frame.
        /// </summary>
        /// <param name="color">The color tint (ARGB format).</param>
        public void SetColorTint(uint color)
        {
            ColorTint = color;
        }
    }
}

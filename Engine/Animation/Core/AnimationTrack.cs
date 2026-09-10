// ====================================================================================================
//  FILE: AnimationTrack.cs
//  PATH: Engine/Animation/Core/
//  MODULE: Animation Track
//
//  ROLE:
//      Represents per-property animation tracks in the animation system. Manages keyframes, per-frame
//      values, and interpolation logic for sprite indices, transforms, colors, and other properties.
//
//  RESPONSIBILITIES:
//      - Store ordered AnimationFrame instances for a single track.
//      - Provide keyframe-based interpolation for multiple value types.
//      - Expose debug information for diagnostics and tooling.
//      - Support custom interpolatable types via IInterpolatable<T>.
//
//  NON-RESPONSIBILITIES:
//      - Does not manage global animation state or timelines.
//      - Does not perform rendering.
//      - Does not handle asset loading or resource management.
//
//  ARCHITECTURAL NOTES:
//      - Designed as a reusable core component for the animation subsystem.
//      - Interpolation is extensible via IInterpolatable<T> implementations.
//      - Numeric and vector interpolation fall back to built-in logic when no custom type is provided.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.VectorMath;
using static SASZombieAssaultTD.Engine.Animation.AnimationEnums;

namespace SASZombieAssaultTD.Engine.Animation.Core
{
    ///<summary>
    ///P11-16-01: Represents per-property animation tracks.
    ///Manages sprite indices, transform offsets, and other per-frame properties.
    ///</summary>
    public sealed class AnimationTrack
    {
        ///<summary>
        ///Gets the name of this animation track.
        ///</summary>
        public string Name { get; }

        ///<summary>
        ///Gets the type of this animation track.
        ///</summary>
        public AnimationTrackType Type { get; }

        ///<summary>
        ///Gets the frames in this animation track.
        ///</summary>
        public List<AnimationFrame> Frames { get; } = new();

        ///<summary>
        ///Gets the metadata associated with this animation track.
        ///</summary>
        public IReadOnlyDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        ///<summary>
        ///Initializes a new AnimationTrack.
        ///</summary>
        ///<param name="name">The name of the track.</param>
        ///<param name="type">The type of the track.</param>
        public AnimationTrack(string name, AnimationTrackType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationTrack: Created '{name}' (Type: {type})");
        }

        ///<summary>
        ///Adds a frame to this animation track.
        ///</summary>
        ///<param name="frame">The frame to add.</param>
        public void AddFrame(AnimationFrame frame)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            Frames.Add(frame);
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationTrack '{Name}': Added frame at time {frame.Time}");
        }

        ///<summary>
        ///Adds a keyframe to this animation track.
        ///Stores the value in frame metadata under "KeyframeValue".
        ///</summary>
        ///<param name="time">The time of the keyframe.</param>
        ///<param name="value">The value of the keyframe.</param>
        public void AddKeyframe(float time, object value)
        {
            var frame = new AnimationFrame(time, 0.016f);
            frame.SetMetadata("KeyframeValue", value);
            AddFrame(frame);
            DLogger.Log(LogSubsystems.Animation, LogEnums.LogLevel.Debug, $"AnimationTrack '{Name}': Added keyframe at time {time:F3} with value {value}");
        }

        ///<summary>
        ///Gets the interpolated value at a specific time.
        ///Uses keyframes stored in frame metadata under "KeyframeValue".
        ///</summary>
        ///<typeparam name="T">The type of the value to retrieve.</typeparam>
        ///<param name="time">The time to evaluate.</param>
        ///<param name="defaultValue">Default value if no keyframes found.</param>
        ///<returns>Interpolated value or default.</returns>
        public T GetInterpolatedValue<T>(float time, T defaultValue = default!)
        {
            if (Frames.Count == 0)
                return defaultValue;

            // Find surrounding keyframes
            AnimationFrame? beforeFrame = null;
            AnimationFrame? afterFrame = null;

            foreach (var frame in Frames)
            {
                if (frame.Time <= time && (beforeFrame == null || frame.Time > beforeFrame.Time))
                    beforeFrame = frame;

                if (frame.Time >= time && (afterFrame == null || frame.Time < afterFrame.Time))
                    afterFrame = frame;
            }

            // Exact keyframe match: return its value
            if (beforeFrame != null && MathF.Abs(beforeFrame.Time - time) < 0.001f)
                return beforeFrame.GetMetadata("KeyframeValue", defaultValue);

            if (afterFrame != null && MathF.Abs(afterFrame.Time - time) < 0.001f)
                return afterFrame.GetMetadata("KeyframeValue", defaultValue);

            // Interpolate between keyframes
            if (beforeFrame != null && afterFrame != null)
            {
                var t = (time - beforeFrame.Time) / (afterFrame.Time - beforeFrame.Time);

                var value1 = beforeFrame.GetMetadata("KeyframeValue", defaultValue);
                var value2 = afterFrame.GetMetadata("KeyframeValue", defaultValue);

                return InterpolateKeyframes(value1, value2, t);
            }

            // Extrapolate from last keyframe
            if (beforeFrame != null && afterFrame == null)
            {
                var timeDiff = time - beforeFrame.Time;
                var beforeValue = beforeFrame.GetMetadata("KeyframeValue", defaultValue);

                if (beforeValue is IInterpolatable<T> interpolatable)
                    return interpolatable.Extrapolate(timeDiff);
            }

            return defaultValue;
        }

        ///<summary>
        ///Interpolates between two keyframe values.
        ///Uses IInterpolatable&lt;T&gt; when available, otherwise falls back to built-in numeric and vector interpolation.
        ///</summary>
        ///<typeparam name="T">The type of values to interpolate.</typeparam>
        ///<param name="value1">The first value.</param>
        ///<param name="value2">The second value.</param>
        ///<param name="t">Interpolation factor (0-1).</param>
        ///<returns>Interpolated value.</returns>
        private static T InterpolateKeyframes<T>(T value1, T value2, float t)
        {
            // Custom interpolatable type
            if (value1 is IInterpolatable<T> interpolatable)
                return interpolatable.Interpolate(value2, t);

            // Float interpolation
            if (typeof(T) == typeof(float))
            {
                float f1 = System.Convert.ToSingle(value1);
                float f2 = System.Convert.ToSingle(value2);
                return (T)(object)(f1 * (1f - t) + f2 * t);
            }

            // Int interpolation (force System.Convert to avoid custom generic overload)
            if (typeof(T) == typeof(int))
            {
                int i1 = System.Convert.ToInt32(value1);
                int i2 = System.Convert.ToInt32(value2);
                return (T)(object)(int)((i1 * (1f - t) + i2 * t));
            }

            // Vector3 interpolation
            if (typeof(T) == typeof(Vector3))
            {
                var v1 = (Vector3)(object)value1!;
                var v2 = (Vector3)(object)value2!;
                return (T)(object)(v1 + (v2 - v1) * t);
            }

            // Bool interpolation: step at 0.5
            if (typeof(T) == typeof(bool))
            {
                bool b1 = (bool)(object)value1!;
                bool b2 = (bool)(object)value2!;
                return (T)(object)(t < 0.5f ? b1 : b2);
            }

            // Fallback: return first value
            return value1;
        }

        ///<summary>
        ///Gets debug information about this animation track.
        ///</summary>
        ///<returns>Debug information string.</returns>
        public string GetDebugInfo()
        {
            var info =
                $"AnimationTrack Debug Info:\n" +
                $"  Name: {Name}\n" +
                $"  Type: {Type}\n" +
                $"  Frame Count: {Frames.Count}\n" +
                $"  Metadata Count: {Metadata.Count}\n";

            if (Frames.Count > 0)
            {
                info += "  Keyframes:\n";
                for (int i = 0; i < MathF.Min(5, Frames.Count); i++)
                {
                    var frame = Frames[i];
                    if (frame.Metadata.ContainsKey("KeyframeValue"))
                        info += $"    [{i}] Time: {frame.Time:F3}, Value: {frame.Metadata["KeyframeValue"]}\n";
                }

                if (Frames.Count > 5)
                    info += $"    ... and {Frames.Count - 5} more keyframes\n";
            }

            return info;
        }
    }

    ///<summary>
    ///P11-16-01: Types of animation tracks.
    ///</summary>

    ///<summary>
    ///P11-16-01: Interface for interpolatable values.
    ///</summary>
    public interface IInterpolatable<T>
    {
        ///<summary>Interpolates this value towards another value.</summary>
        T Interpolate(T target, float t);

        ///<summary>Extrapolates this value forward in time.</summary>
        T Extrapolate(float timeDelta);
    }

    ///<summary>
    ///P11-16-01: Interpolatable implementation for float values.
    ///</summary>
    public struct InterpolatableFloat : IInterpolatable<float>
    {
        private readonly float _value;

        public InterpolatableFloat(float value) => _value = value;

        public float Interpolate(float target, float t) => _value + (target - _value) * t;
        public float Extrapolate(float timeDelta) => _value;
    }

    ///<summary>
    ///P11-16-01: Interpolatable implementation for Vector3 values.
    ///</summary>
    public struct InterpolatableVector3 : IInterpolatable<Vector3>
    {
        private readonly Vector3 _value;

        public InterpolatableVector3(Vector3 value) => _value = value;

        public Vector3 Interpolate(Vector3 target, float t) => _value + (target - _value) * t;
        public Vector3 Extrapolate(float timeDelta) => _value;
    }

    ///<summary>
    ///P11-16-01: Interpolatable implementation for int values.
    ///</summary>
    public struct InterpolatableInt : IInterpolatable<int>
    {
        private readonly int _value;

        public InterpolatableInt(int value) => _value = value;

        public int Interpolate(int target, float t) => (int)(_value + (target - _value) * t);
        public int Extrapolate(float timeDelta) => _value;
    }

    ///<summary>
    ///P11-16-01: Interpolatable implementation for bool values.
    ///</summary>
    public struct InterpolatableBool : IInterpolatable<bool>
    {
        private readonly bool _value;

        public InterpolatableBool(bool value) => _value = value;

        public bool Interpolate(bool target, float t) => t < 0.5f ? _value : target;
        public bool Extrapolate(float timeDelta) => _value;
    }

    ///<summary>
    ///P11-16-01: Simple info container for animation frames.
    ///</summary>
    public class AnimationFrameInfo
    {
        public int FrameNumber { get; set; }
        public float Time { get; set; }
        public Dictionary<string, float> Parameters { get; set; } = new();
    }
}

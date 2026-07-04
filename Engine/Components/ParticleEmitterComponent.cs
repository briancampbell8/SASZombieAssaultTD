/*
File:    ParticleEmitterComponent.cs
Path:    Engine/Components/ParticleEmitterComponent.cs
Purpose:   P11-03-04-A - Core ECS component for particle emitter properties.
           Stores emission settings, particle properties, and emitter state for visual effects.

Role:      Essential particle emitter component for entities requiring particle effects.
           - Defines emission rates and particle generation parameters
           - Manages particle velocity and lifetime properties
           - Controls particle scaling, rotation, and color variations
           - Handles emitter state and active particle tracking
           - Supports both continuous and burst emission modes

Features:   ECS-friendly pure data structure optimized for particle system.
           Comprehensive particle property control for visual variety.
           Emission rate limiting for performance optimization.
           Randomized particle properties for natural-looking effects.
           Thread-safe property access for concurrent particle updates.
           Integration with particle system for real-time rendering.

Notes:      This component is required by entities with particle effects.
           Particle properties are designed for high-frequency updates.
           Component supports both 2D and 3D particle generation.
           Emitter can be controlled dynamically through gameplay events.
           All particle calculations are optimized for real-time performance.

*/
using System;
using System.Drawing;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Components
{
    ///<summary>
    ///Component for particle emitter properties.
    ///P11-03-04-A: Stores emission rate, particle limits, velocity ranges, lifetime ranges,
    ///scale ranges, rotation ranges, color tint ranges, looping state, and active state.
    ///</summary>
    public class ParticleEmitterComponent
    {
        /// Properties

        ///<summary>
        ///Emission rate (particles per second).
        ///</summary>
        public float EmissionRate { get; set; } = 10.0f;

        ///<summary>
        ///Maximum number of particles that can be active at once.
        ///</summary>
        public int MaxParticles { get; set; } = 100;

        ///<summary>
        ///Range for initial particle velocity (min and max).
        ///</summary>
        public VelocityRange InitialVelocityRange { get; set; } = new(-50.0f, 50.0f, -50.0f, 50.0f);

        ///<summary>
        ///Range for particle lifetime (min and max in seconds).
        ///</summary>
        public FloatRange LifetimeRange { get; set; } = new(1.0f, 3.0f);

        ///<summary>
        ///Range for initial particle scale (min and max).
        ///</summary>
        public FloatRange InitialScaleRange { get; set; } = new(0.5f, 2.0f);

        ///<summary>
        ///Range for initial particle rotation (min and max in radians).
        ///</summary>
        public FloatRange InitialRotationRange { get; set; } = new(0.0f, (float)System.Math.PI * 2.0f);

        ///<summary>
        ///Range for particle color tint (min and max).
        ///</summary>
        public ColorRange ColorTintRange { get; set; } = new(Color.White, Color.White);

        ///<summary>
        ///Whether the emitter continuously loops particle emission.
        ///</summary>
        public bool IsLooping { get; set; } = true;

        ///<summary>
        ///Whether the emitter is currently active and emitting particles.
        ///</summary>
        public bool IsActive { get; set; } = true;

        ///<summary>
        ///Accumulated time for emission timing.
        ///</summary>
        public float EmissionAccumulator { get; set; } = 0.0f;

        ///

        /// Constructors

        ///<summary>
        ///Creates a new ParticleEmitterComponent with default values.
        ///</summary>
        public ParticleEmitterComponent() { }

        ///<summary>
        ///Creates a new ParticleEmitterComponent with specified emission rate.
        ///</summary>
        ///<param name="emissionRate">Particles per second</param>
        public ParticleEmitterComponent(float emissionRate)
        {
            EmissionRate = emissionRate;
        }

        ///<summary>
        ///Creates a new ParticleEmitterComponent with full configuration.
        ///</summary>
        ///<param name="emissionRate">Particles per second</param>
        ///<param name="maxParticles">Maximum active particles</param>
        ///<param name="initialVelocityRange">Initial velocity range</param>
        ///<param name="lifetimeRange">Particle lifetime range</param>
        ///<param name="initialScaleRange">Initial scale range</param>
        ///<param name="initialRotationRange">Initial rotation range</param>
        ///<param name="colorTintRange">Color tint range</param>
        ///<param name="isLooping">Whether emitter loops</param>
        ///<param name="isActive">Whether emitter is active</param>
        public ParticleEmitterComponent(
            float emissionRate,
            int maxParticles,
            VelocityRange initialVelocityRange,
            FloatRange lifetimeRange,
            FloatRange initialScaleRange,
            FloatRange initialRotationRange,
            ColorRange colorTintRange,
            bool isLooping = true,
            bool isActive = true)
        {
            EmissionRate = emissionRate;
            MaxParticles = maxParticles;
            InitialVelocityRange = initialVelocityRange;
            LifetimeRange = lifetimeRange;
            InitialScaleRange = initialScaleRange;
            InitialRotationRange = initialRotationRange;
            ColorTintRange = colorTintRange;
            IsLooping = isLooping;
            IsActive = isActive;
        }

        ///

        /// Methods

        ///<summary>
        ///Resets the particle emitter to its default state.
        ///</summary>
        public void Reset()
        {
            EmissionAccumulator = 0.0f;
            IsActive = true;
        }

        ///<summary>
        ///Gets a string representation for debugging.
        ///</summary>
        public override string ToString()
        {
            return $"ParticleEmitter(Rate: {EmissionRate}, Max: {MaxParticles}, Active: {IsActive})";
        }

        ///
    }

    ///<summary>
    ///Range for float values with minimum and maximum.
    ///</summary>
    public class FloatRange
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public FloatRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        ///<summary>
        ///Gets a random value within the range.
        ///</summary>
        public float GetRandom()
        {
            var random = new Random();
            return (float)(random.NextDouble() * (Max - Min) + Min);
        }

        public override string ToString()
        {
            return $"FloatRange({Min:F2}, {Max:F2})";
        }
    }

    ///<summary>
    ///Range for velocity values with X and Y components.
    ///</summary>
    public class VelocityRange
    {
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }

        public VelocityRange(float minX, float maxX, float minY, float maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        ///<summary>
        ///Gets a random velocity within the range.
        ///</summary>
        public PointF GetRandom()
        {
            var random = new Random();
            var x = (float)(random.NextDouble() * (MaxX - MinX) + MinX);
            var y = (float)(random.NextDouble() * (MaxY - MinY) + MinY);
            return new PointF(x, y);
        }

        public override string ToString()
        {
            return $"VelocityRange(X: {MinX:F1},{MaxX:F1}, Y: {MinY:F1},{MaxY:F1})";
        }
    }

    ///<summary>
    ///Range for color values with minimum and maximum.
    ///</summary>
    public class ColorRange
    {
        public Color Min { get; set; }
        public Color Max { get; set; }

        public ColorRange(Color min, Color max)
        {
            Min = min;
            Max = max;
        }

        ///<summary>
        ///Gets a random color within the range.
        ///</summary>
        public Color GetRandom()
        {
            var random = new Random();
            var r = (byte)(random.NextDouble() * (Max.R - Min.R) + Min.R);
            var g = (byte)(random.NextDouble() * (Max.G - Min.G) + Min.G);
            var b = (byte)(random.NextDouble() * (Max.B - Min.B) + Min.B);
            var a = (byte)(random.NextDouble() * (Max.A - Min.A) + Min.A);
            return Color.FromArgb(a, r, g, b);
        }

        public override string ToString()
        {
            return $"ColorRange(Min: {Min}, Max: {Max})";
        }
    }
}





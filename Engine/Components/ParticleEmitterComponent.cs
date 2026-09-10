// ====================================================================================================
//  FILE: ParticleEmitterComponent.cs
//  PATH: ./Engine/Components/
//  MODULE: Core
//
//  ROLE:
//      Defines particle emitter behavior for entities that generate visual particle effects.
//
//  RESPONSIBILITIES:
//      - Store emitter configuration (rate, lifetime, velocity, color).
//      - Maintain deterministic emission state per frame.
//      - Provide data for rendering and simulation subsystems.
//
//  NON-RESPONSIBILITIES:
//      - Rendering particles (delegated to ParticleRenderSystem).
//      - Physics or collision of particles (delegated to ParticlePhysicsSystem).
//
//  NOTES:
//      Designed for ECS integration; lightweight data container.
// ====================================================================================================

using System;
using System.Diagnostics;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.Components
{
    public sealed class ParticleEmitterComponent
    {
        internal object LifetimeRange;
        private object Min;
        private object Max;

        public bool IsActive { get; set; } = true;
        public float EmissionRate { get; set; } = 10f;          // particles per second
        public float ParticleLifetime { get; set; } = 1.5f;     // seconds
        public float NextFloat { get; set; }
        public Vector3 InitialVelocity { get; set; } = Vector3.Zero;
        public Vector4 ParticleColor { get; set; } = new(1f, 1f, 1f, 1f); // RGBA
        public float EmissionAccumulator { get; internal set; }
        public int MaxParticles { get; internal set; }
        public object InitialVelocityRange { get; internal set; }
        public object InitialRotationRange { get; internal set; }
        public object InitialScaleRange { get; internal set; }
        public object ColorTintRange { get; internal set; }
        public object RandomHelper { get; private set; }
        // A single shared static random instance for performance
        public static readonly Random RandomProvider = new Random();

        // Use the new Vector2Range type instead of 'object'




        public ParticleEmitterComponent()
        { }



        private string GetDebuggerDisplay()
        {
            return ToString();
        }



        public void ResetEmitter()
        {
            IsActive = true;
        }

        // 1. Define the range structure outside or above your component
        [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
        public struct Vector2Range
        {
            public Vector2 Min;
            public Vector2 Max;

            public Vector2Range(Vector2 min, Vector2 max)
            {
                Min = min;
                Max = max;
            }

            // Move the GetRandom logic here where Min and Max actually exist


            // 2. Clean up your component properties

            private string GetDebuggerDisplay() => ToString();
        }
    }
}















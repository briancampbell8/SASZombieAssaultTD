// =====================================================================================================
//  FILE: ParticleEmitterComponent.cs
//  PATH: Engine/Render/Particles/ParticleEmitterComponent.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Configuration container for particle emission parameters.
//
//  RESPONSIBILITIES:
//      - Store emission rate, max particles, and accumulator.
//      - Store deterministic range objects for velocity, rotation, scale, color, lifetime.
//      - Provide active flag and delta-time passthrough.
//
//  NON-RESPONSIBILITIES:
//      - Emission logic.
//      - Simulation.
//      - Rendering.
//      - Pool management.
//
//  ARCHITECTURAL NOTES:
//      - Pure data component.
//      - Used by ParticleEmitter.
// =====================================================================================================

using SASZombieAssaultTD.Engine.Render.Particles.Range;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    public sealed class ParticleEmitterComponent
    {
        public bool IsActive { get; set; } = true;

        public float EmissionRate { get; set; } = 10f;
        public float EmissionAccumulator { get; set; }
        public float DeltaTime { get; set; }

        public int MaxParticles { get; set; } = 200;

        public RangeVector2 InitialVelocityRange { get; set; } =
            new RangeVector2(new Vector3(-1f, -1f, 0f), new Vector3(1f, 1f, 0f));

        public RangeFloat InitialRotationRange { get; set; } =
            new RangeFloat(0f, 360f);

        public RangeFloat InitialScaleRange { get; set; } =
            new RangeFloat(0.5f, 2f);

        public RangeColor ColorTintRange { get; set; } =
            new RangeColor(new Color(255, 255, 255, 255), new Color(255, 255, 255, 255));

        public RangeFloat LifetimeRange { get; set; } =
            new RangeFloat(0.5f, 2f);
    }
}

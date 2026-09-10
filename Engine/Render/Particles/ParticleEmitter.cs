// =====================================================================================================
//  FILE: ParticleEmitter.cs
//  PATH: Engine/Render/Particles/ParticleEmitter.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Emits new particles using emitter component ranges and registers them into ParticleCore.
//
//  RESPONSIBILITIES:
//      - Read emitter component values.
//      - Spawn particles deterministically.
//      - Push new particles into ParticleCore.ActiveParticles.
//
//  NON-RESPONSIBILITIES:
//      - Simulation.
//      - Rendering.
//      - Pool management.
//
//  ARCHITECTURAL NOTES:
//      - Micro‑subprogram.
//      - All heavy logic resides in ParticleSimulation and ParticlePool.
// =====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    internal sealed class ParticleEmitter
    {
        private readonly ParticleCore _core;

        public ParticleEmitter(ParticleCore core)
        {
            _core = core;
        }

        // -------------------------------------------------------------------------------------------------
        //  UPDATE ACCUMULATOR (DETERMINISTIC)
        // -------------------------------------------------------------------------------------------------
        // Advances emitter time and prepares emission count for the next Emit() call.
        public void Update(ParticleEmitterComponent emitter, float deltaTime)
        {
            if (!emitter.IsActive)
                return;

            emitter.DeltaTime = deltaTime;
            emitter.EmissionAccumulator += deltaTime;
        }

        // -------------------------------------------------------------------------------------------------
        //  EMIT NEW PARTICLES (DETERMINISTIC)
        // -------------------------------------------------------------------------------------------------
        public void Emit(ParticleEmitterComponent emitter, Vector3 position)
        {
            if (!emitter.IsActive)
                return;

            // Convert accumulated time into particle count
            int count = (int)(emitter.EmissionAccumulator * emitter.EmissionRate);
            emitter.EmissionAccumulator -= count / emitter.EmissionRate;

            // Spawn deterministically, respecting max particle cap
            for (int i = 0; i < count && _core.ActiveParticles.Count < emitter.MaxParticles; i++)
                Spawn(emitter, position);
        }

        // -------------------------------------------------------------------------------------------------
        //  SPAWN PARTICLE (MICRO‑SUBPROGRAM)
        // -------------------------------------------------------------------------------------------------
        private void Spawn(ParticleEmitterComponent emitter, Vector3 position)
        {
            ParticleData particle =
                _core.Pool.Count > 0
                ? _core.Pool.Dequeue()
                : new ParticleData(position, Vector3.Zero, 0f, 1f, Color.White, 1f);

            particle.Position = position;

            // Velocity
            Vector3 vel = emitter.InitialVelocityRange.GetRandom();
            particle.Velocity = new Vector3(vel.X, vel.Y, 0f);

            // Rotation
            particle.Rotation = emitter.InitialRotationRange.GetRandom();

            // Scale
            particle.Scale = emitter.InitialScaleRange.GetRandom();

            // Color tint
            Color tint = emitter.ColorTintRange.GetRandom();
            particle.Color = new Color(tint.R, tint.G, tint.B, tint.A);

            // Lifetime
            particle.Lifetime = emitter.LifetimeRange.GetRandom();
            particle.Age = 0f;
            particle.IsAlive = true;

            _core.ActiveParticles.Add(particle);
        }
    }
}

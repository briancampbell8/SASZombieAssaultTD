// =====================================================================================================
//  FILE: ParticlePool.cs
//  PATH: Engine/Render/Particles/ParticlePool.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Provides deterministic pooling for ParticleData instances.
//      Supplies reusable particles to ParticleEmitter and returns expired ones.
//
//  RESPONSIBILITIES:
//      - Pre‑allocate a fixed pool size.
//      - Provide Get() and Return() operations.
//      - Maintain deterministic pool limits.
//
//  NON-RESPONSIBILITIES:
//      - Emission.
//      - Simulation.
//      - Rendering.
//
//  ARCHITECTURAL NOTES:
//      - Micro‑subprogram.
//      - All particle behavior resides in other subprograms.
// =====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    internal sealed class ParticlePool
    {
        private readonly ParticleCore _core;
        private const int PoolSize = 200;

        public ParticlePool(ParticleCore core)
        {
            _core = core;
        }

        public void Initialize()
        {
            for (int i = 0; i < PoolSize; i++)
            {
                _core.Pool.Enqueue(
                    new ParticleData(
                        Vector3.Zero,
                        Vector3.Zero,
                        0f,
                        1f,
                        Color.White,
                        1f));
            }
        }

        public ParticleData Get()
        {
            return _core.Pool.Count > 0
                ? _core.Pool.Dequeue()
                : new ParticleData(Vector3.Zero, Vector3.Zero, 0f, 1f, Color.White, 1f);
        }

        public void Return(ParticleData particle)
        {
            if (_core.Pool.Count >= PoolSize)
                return;

            particle.Age = 0f;
            particle.IsAlive = true;
            _core.Pool.Enqueue(particle);
        }
    }
}

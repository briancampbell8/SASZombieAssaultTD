// =====================================================================================================
//  FILE: ParticleSimulation.cs
//  PATH: Engine/Render/Particles/ParticleSimulation.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Advances particle state deterministically and returns expired particles to the pool.
//
//  RESPONSIBILITIES:
//      - Update particle age and position.
//      - Mark particles dead when lifetime expires.
//      - Return expired particles to ParticlePool.
//      - Maintain deterministic iteration order.
//
//  NON-RESPONSIBILITIES:
//      - Emission.
//      - Rendering.
//      - Pool initialization.
//
//  ARCHITECTURAL NOTES:
//      - Micro‑subprogram.
//      - All particle data stored in ParticleCore.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    internal sealed class ParticleSimulation
    {
        private readonly ParticleCore _core;
        private readonly ParticlePool _pool;

        public ParticleSimulation(ParticleCore core)
        {
            _core = core;
            _pool = new ParticlePool(core);
        }

        public void Update(float deltaTime)
        {
            if (_core.IsPaused)
                return;

            for (int i = _core.ActiveParticles.Count - 1; i >= 0; i--)
            {
                var p = _core.ActiveParticles[i];
                p.Update(deltaTime);

                if (!p.IsAlive)
                {
                    _core.ActiveParticles.RemoveAt(i);
                    _pool.Return(p);
                }
            }
        }
    }
}

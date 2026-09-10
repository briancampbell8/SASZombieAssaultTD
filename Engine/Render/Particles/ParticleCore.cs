// =====================================================================================================
//  FILE: ParticleCore.cs
//  PATH: Engine/Render/Particles/ParticleCore.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Central data hub for all particle subprograms.
//      Maintains shared particle collections, pause state, and deterministic sequencing.
//
//  RESPONSIBILITIES:
//      - Store active particles.
//      - Provide pooled particle access.
//      - Expose shared state to ParticleEmitter, ParticleSimulation, ParticleRender, ParticlePool.
//      - Support deterministic update ordering.
//
//  NON-RESPONSIBILITIES:
//      - Emission logic.
//      - Simulation logic.
//      - Rendering logic.
//      - Randomization.
//
//  ARCHITECTURAL NOTES:
//      - Micro‑core: minimal state, no heavy logic.
//      - All behavior resides in subprograms.
// =====================================================================================================

using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    internal sealed class ParticleCore
    {
        public readonly List<ParticleData> ActiveParticles = new();
        public readonly Queue<ParticleData> Pool = new();

        public bool IsPaused { get; set; }

        public void ClearAll()
        {
            ActiveParticles.Clear();
            Pool.Clear();
        }
    }
}

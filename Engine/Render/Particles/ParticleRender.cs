// =====================================================================================================
//  FILE: ParticleRender.cs
//  PATH: Engine/Render/Particles/ParticleRender.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Renders all active particles using deterministic draw operations.
//
//  RESPONSIBILITIES:
//      - Iterate active particles.
//      - Issue draw calls for each alive particle.
//      - Convert particle state into render‑ready rectangles.
//
//  NON-RESPONSIBILITIES:
//      - Simulation.
//      - Emission.
//      - Pooling.
//      - Randomization.
//
//  ARCHITECTURAL NOTES:
//      - Micro‑subprogram.
//      - All heavy logic resides in other subprograms.
// =====================================================================================================

using System.Drawing;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    internal sealed class ParticleRender
    {
        private readonly ParticleCore _core;

        public ParticleRender(ParticleCore core)
        {
            _core = core;
        }

        public void Render(D3D11Adapter_Core context)
        {
            if (_core.IsPaused)
                return;

            foreach (var particle in _core.ActiveParticles)
            {
                if (!particle.IsAlive)
                    continue;

                var rect = new Rectangle(
                    (int)(particle.Position.X - particle.Scale),
                    (int)(particle.Position.Y - particle.Scale),
                    (int)(particle.Scale * 2f),
                    (int)(particle.Scale * 2f));

                context.DrawRectangle(
                    rect,
                    particle.Position,
                    particle.Color,
                    1);
            }
        }
    }
}

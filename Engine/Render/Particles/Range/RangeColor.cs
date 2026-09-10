// =====================================================================================================
//  FILE: RangeColor.cs
//  PATH: Engine/Render/Particles/Range/RangeColor.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Provides deterministic random selection within a color range.
//
//  RESPONSIBILITIES:
//      - Store min/max color bounds.
//      - Provide GetRandom() returning a valid color within range.
//
//  NON-RESPONSIBILITIES:
//      - Rendering.
//      - Simulation.
//      - Emitter logic.
//
//  ARCHITECTURAL NOTES:
//      - Micro-class.
//      - Used by ParticleEmitter.
// =====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Render.Particles.Range
{
    public sealed class RangeColor
    {
        public Color Min { get; }
        public Color Max { get; }

        private readonly Random _rng = new Random();

        public RangeColor(Color min, Color max)
        {
            Min = min;
            Max = max;
        }

        public Color GetRandom()
        {
            byte r = (byte)_rng.Next(Min.R, Max.R + 1);
            byte g = (byte)_rng.Next(Min.G, Max.G + 1);
            byte b = (byte)_rng.Next(Min.B, Max.B + 1);
            byte a = (byte)_rng.Next(Min.A, Max.A + 1);
            return new Color(r, g, b, a);
        }
    }
}

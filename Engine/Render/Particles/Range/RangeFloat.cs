// =====================================================================================================
//  FILE: RangeFloat.cs
//  PATH: Engine/Render/Particles/Range/RangeFloat.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Provides deterministic random selection within a float range.
//
//  RESPONSIBILITIES:
//      - Store min/max float bounds.
//      - Provide GetRandom() returning a valid float within range.
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
    public sealed class RangeFloat
    {
        public float Min { get; }
        public float Max { get; }

        private readonly Random _rng = new Random();

        public RangeFloat(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float GetRandom()
        {
            return (float)(_rng.NextDouble() * (Max - Min) + Min);
        }
    }
}

// =====================================================================================================
//  FILE: RangeVector2.cs
//  PATH: Engine/Render/Particles/Range/RangeVector2.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Provides deterministic random selection within a 2D vector range.
//
//  RESPONSIBILITIES:
//      - Store min/max vector bounds.
//      - Provide GetRandom() returning a valid Vector2 within range.
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
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Render.Particles.Range
{
    public sealed class RangeVector2
    {
        public Vector3 Min { get; }
        public Vector3 Max { get; }

        private readonly Random _rng = new Random();

        public RangeVector2(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public Vector3 GetRandom()
        {
            float x = (float)(_rng.NextDouble() * (Max.X - Min.X) + Min.X);
            float y = (float)(_rng.NextDouble() * (Max.Y - Min.Y) + Min.Y);
            return new Vector3(x, y, 0f);
        }
    }
}

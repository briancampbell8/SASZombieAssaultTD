// =====================================================================================================
//  FILE: ParticleData.cs
//  PATH: Engine/Render/Particles/ParticleData.cs
//  SUBSYSTEM: Render Particles
//
//  ROLE:
//      Pure particle state container used by all particle subprograms.
//
//  RESPONSIBILITIES:
//      - Store particle position, velocity, rotation, scale, color.
//      - Track lifetime and age.
//      - Provide deterministic Update() for basic movement and aging.
//
//  NON-RESPONSIBILITIES:
//      - Rendering.
//      - Simulation logic beyond basic movement.
//      - Pooling or emitter behavior.
//
//  ARCHITECTURAL NOTES:
//      - Micro‑class.
//      - No external side effects.
// =====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Render.Particles
{
    internal sealed class ParticleData
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public Color Color { get; set; }
        public float Lifetime { get; set; }
        public float Age { get; set; }
        public bool IsAlive { get; set; } = true;

        public ParticleData(Vector3 position, Vector3 velocity, float rotation, float scale, Color color, float lifetime)
        {
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            Scale = scale;
            Color = color;
            Lifetime = lifetime;
            Age = 0f;
            IsAlive = true;
        }

        public void Update(float deltaTime)
        {
            if (!IsAlive)
                return;

            Age += deltaTime;
            Position += Velocity * deltaTime;

            if (Age >= Lifetime)
                IsAlive = false;
        }
    }
}

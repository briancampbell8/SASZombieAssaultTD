// =====================================================================================================
//  FILE: RigidBodyComponent.cs
//  PATH: Engine/Components/RigidBodyComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core engine component for representing rigid‑body physics state. Stores mass, drag,
//      kinematic/static flags, trigger flags, and basic impulse/force/torque entry points used
//      by physics and collision systems.
//
//  RESPONSIBILITIES:
//      - Store rigid‑body physical properties (mass, drag, speed, kinematic/static state)
//      - Provide flags for trigger and static collision behavior
//      - Expose impulse, force, and torque application entry points
//      - Act as a lightweight data carrier for physics and collision systems
//
//  NON-RESPONSIBILITIES:
//      - Performing collision detection or resolution
//      - Executing physics simulation or updating body motion
//      - Managing world-level ECSEntityCore lifecycle or ECS attachment
//      - Applying world‑space transforms (handled by TransformComponent)
//
//  ARCHITECTURAL NOTES:
//      - This is a pure engine component with minimal behavioral helpers
//      - Integrates with physics and collision systems but does not implement them
//      - Kept intentionally lightweight to preserve subsystem boundaries
// =====================================================================================================



using System.Drawing;

namespace SASZombieAssaultTD.Engine.Components
{
    internal class RigidBodyComponent
    {
        public object Body { get; set; }
        public object Shape { get; set; }
        public bool IsTrigger { get; set; }
        public bool IsStatic { get; set; }

        public bool IsKinematic { get; set; }
        public float Mass { get; set; }
        public float Speed { get; set; }
        public int MaxSpeed { get; set; }
        public float Drag { get; set; }
        public float Bounciness { get; set; }

        public RigidBodyComponent(
            bool isKinematic = false,
            float drag = 0.0f,
            float bounciness = 0.0f,
            float mass = 1.0f
            )
        {
            IsKinematic = false;
            Mass = 1.0f;
        }

        public void ApplyImpulse(PointF impulse, PointF point) { }
        public void ApplyForce(PointF force)
        { }
        public void ApplyTorque(float torque)
        { }


    }
}

// =====================================================================================================
//  FILE: DamageComponent.cs
//  PATH: Engine/Components/DamageComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core component for representing damage events and damage-related metadata. Stores damage
//      values, damage types, source/target identifiers, and positional/directional information
//      used by gameplay, combat, and hit‑resolution systems.
//
//  RESPONSIBILITIES:
//      - Store damage amount and damage type
//      - Store damage source and target identifiers
//      - Store positional, directional, and velocity data for damage events
//      - Provide a deterministic, ECS‑friendly data surface for combat and hit‑processing systems
//
//  NON-RESPONSIBILITIES:
//      - Executing combat logic or applying damage to entities
//      - Managing world‑level ECSEntityCore allocation or lifecycle sequencing
//      - Performing physics, collision, or movement calculations
//      - Acting as a system or orchestrator
//
//  ARCHITECTURAL NOTES:
//      - This is a pure data component with no behavioral logic
//      - All fields use internal setters to preserve deterministic engine mutation rules
//      - Integrates with combat, projectile, and hit‑resolution systems but does not implement them
// =====================================================================================================


using System.Numerics;

namespace SASZombieAssaultTD.Engine.Components
{
    public class DamageComponent
    {
        public object Damage { get; internal set; }
        public object DamageType { get; internal set; }
        public object DamageSource { get; internal set; }
        public int TargetId { get; internal set; }
        public int SourceId { get; internal set; }
        public Vector2 Position { get; internal set; }
        public Vector2 Direction { get; internal set; }
        public Vector2 Velocity { get; internal set; }

        public DamageComponent(
            object damage,
            object damageType,
            object damageSource,
            int targetId,
            int sourceId,
            Vector2 position,
            Vector2 direction,
            Vector2 velocity)
        {
            Damage = damage;
            DamageType = damageType;
            DamageSource = damageSource;
            TargetId = targetId;
            SourceId = sourceId;
            Position = position;
            Direction = direction;
            Velocity = velocity;
        }

        public DamageComponent()
        {
            Damage = null;
            DamageType = null;
            DamageSource = null;
        }

    }
}

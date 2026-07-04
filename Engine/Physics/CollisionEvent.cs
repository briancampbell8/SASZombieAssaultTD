/*
File:    CollisionEvent.cs
Purpose: P11-14-07 - Collision event data structure.
*/
using SASZombieAssaultTD.Engine.ECS;
using System;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Physics;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Physics
{
    ///<summary>
    ///P11-14-07: Contains information about a collision between two entities.
    ///Used by collision systems to communicate collision data to other systems.
    ///</summary>
    public readonly struct CollisionEvent : IEquatable<CollisionEvent>
    {
        ///<summary>
        ///The first entity in the collision.
        ///</summary>
        public readonly Entity EntityA;

        ///<summary>
        ///The second entity in the collision.
        ///</summary>
        public readonly Entity EntityB;

        ///<summary>
        ///Whether this is a trigger collision (no physics response).
        ///</summary>
        public readonly bool IsTrigger;

        ///<summary>
        ///The contact information for this collision.
        ///</summary>
        public readonly ContactInfo ContactInfo;

        ///<summary>
        ///The timestamp when the collision occurred.
        ///</summary>
        public readonly DateTime Timestamp;

        ///<summary>
        ///Initializes a new collision event.
        ///</summary>
        public CollisionEvent(Entity entityA, Entity entityB, bool isTrigger, ContactInfo contactInfo)
        {
            EntityA = entityA;
            EntityB = entityB;
            IsTrigger = isTrigger;
            ContactInfo = contactInfo;
            Timestamp = DateTime.UtcNow;
        }

        ///<summary>
        ///P11-04-03-C: Trigger enter event for proximity detection.
        ///Published when an entity enters the trigger range of another entity.
        ///</summary>
        public readonly struct TriggerEnterEvent
        {
            ///<summary>
            ///The trigger entity that detected the entry.
            ///</summary>
            public readonly object SourceEntityId;

            ///<summary>
            ///The entity that entered the trigger range.
            ///</summary>
            public readonly object TargetEntityId;

            ///<summary>
            ///The timestamp when the trigger occurred.
            ///</summary>
            public readonly DateTime Timestamp;

            ///<summary>
            ///Initializes a new trigger enter event.
            ///</summary>
            public TriggerEnterEvent(object sourceEntityId, object targetEntityId)
            {
                SourceEntityId = sourceEntityId;
                TargetEntityId = targetEntityId;
                Timestamp = DateTime.UtcNow;
            }
        }

        ///<summary>
        ///P11-04-03-D: Trigger exit event for proximity detection.
        ///Published when an entity exits the trigger range of another entity.
        ///</summary>
        public readonly struct TriggerExitEvent
        {
            ///<summary>
            ///The trigger entity that detected the exit.
            ///</summary>
            public readonly object SourceEntityId;

            ///<summary>
            ///The entity that exited the trigger range.
            ///</summary>
            public readonly object TargetEntityId;

            ///<summary>
            ///The timestamp when the trigger occurred.
            ///</summary>
            public readonly DateTime Timestamp;

            ///<summary>
            ///Initializes a new trigger exit event.
            ///</summary>
            public TriggerExitEvent(object sourceEntityId, object targetEntityId)
            {
                SourceEntityId = sourceEntityId;
                TargetEntityId = targetEntityId;
                Timestamp = DateTime.UtcNow;
            }
        }

        ///<summary>
        ///Initializes a new collision event without contact info.
        ///</summary>
        public CollisionEvent(Entity entityA, Entity entityB, bool isTrigger)
        {
            EntityA = entityA;
            EntityB = entityB;
            IsTrigger = isTrigger;
            ContactInfo = default;
            Timestamp = DateTime.UtcNow;
        }

        ///<summary>
        ///Gets a hash code for this collision event.
        ///</summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(EntityA, EntityB, IsTrigger);
        }

        ///<summary>
        ///Checks if this collision event is equal to another.
        ///</summary>
        public bool Equals(CollisionEvent other)
        {
            return EntityA.Id == other.EntityA.Id &&
                   EntityB.Id == other.EntityB.Id &&
                   IsTrigger == other.IsTrigger;
        }

        ///<summary>
        ///Checks if this collision event is equal to another object.
        ///</summary>
        public override bool Equals(object? obj)
        {
            return obj is CollisionEvent other && Equals(other);
        }

        ///<summary>
        ///Equality operator for collision events.
        ///</summary>
        public static bool operator ==(CollisionEvent left, CollisionEvent right)
        {
            return left.Equals(right);
        }

        ///<summary>
        ///Inequality operator for collision events.
        ///</summary>
        public static bool operator !=(CollisionEvent left, CollisionEvent right)
        {
            return !left.Equals(right);
        }

        ///<summary>
        ///Gets a string representation of this collision event for debugging.
        ///</summary>
        public override string ToString()
        {
            return $"Collision [{EntityA} <-> {EntityB}] Trigger: {IsTrigger}";
        }
    }

    ///<summary>
    ///Contains detailed contact information for a collision.
    ///</summary>
    public readonly struct ContactInfo : IEquatable<ContactInfo>
    {
        ///<summary>
        ///The contact point in world space where the collision occurred.
        ///</summary>
        public readonly Vector3 Point;

        ///<summary>
        ///The collision normal (direction from EntityA to EntityB).
        ///Normalized vector pointing from the first entity to the second.
        ///</summary>
        public readonly Vector3 Normal;

        ///<summary>
        ///The penetration depth (how much the shapes overlap).
        ///Positive value indicates overlap.
        ///</summary>
        public readonly float PenetrationDepth;

        ///<summary>
        ///The relative velocity at the contact point.
        ///</summary>
        public readonly Vector3 RelativeVelocity;

        ///<summary>
        ///Gets whether this contact information is valid.
        ///</summary>
        public bool IsValid => PenetrationDepth > 0f;

        ///<summary>
        ///Initializes a new ContactInfo.
        ///</summary>
        ///<param name="point">The contact point.</param>
        ///<param name="normal">The collision normal.</param>
        ///<param name="penetrationDepth">The penetration depth.</param>
        ///<param name="relativeVelocity">The relative velocity.</param>
        public ContactInfo(Vector3 point, Vector3 normal, float penetrationDepth, Vector3 relativeVelocity)
        {
            Point = point;
            Normal = normal.Normalized;
            PenetrationDepth = System.Math.Max(0f, penetrationDepth);
            RelativeVelocity = relativeVelocity;
        }

        ///<summary>
        ///Initializes a new ContactInfo with basic information.
        ///</summary>
        ///<param name="point">The contact point.</param>
        ///<param name="normal">The collision normal.</param>
        ///<param name="penetrationDepth">The penetration depth.</param>
        public ContactInfo(Vector3 point, Vector3 normal, float penetrationDepth) : this(point, normal, penetrationDepth, Vector3.Zero)
        {
        }

        ///<summary>
        ///Gets a hash code for this contact information.
        ///</summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Point, Normal, PenetrationDepth);
        }

        ///<summary>
        ///Checks if this contact information is equal to another.
        ///</summary>
        public bool Equals(ContactInfo other)
        {
            return Point.Equals(other.Point) &&
                   Normal.Equals(other.Normal) &&
                   System.Math.Abs(PenetrationDepth - other.PenetrationDepth) < 0.001f;
        }

        ///<summary>
        ///Checks if this contact information is equal to another object.
        ///</summary>
        public override bool Equals(object? obj)
        {
            return obj is ContactInfo other && Equals(other);
        }

        ///<summary>
        ///Equality operator for contact information.
        ///</summary>
        public static bool operator ==(ContactInfo left, ContactInfo right)
        {
            return left.Equals(right);
        }

        ///<summary>
        ///Inequality operator for contact information.
        ///</summary>
        public static bool operator !=(ContactInfo left, ContactInfo right)
        {
            return !left.Equals(right);
        }

        ///<summary>
        ///Gets a string representation of this contact information for debugging.
        ///</summary>
        public override string ToString()
        {
            return $"Contact [Point: {Point}, Normal: {Normal}, Depth: {PenetrationDepth:F3}]";
        }
    }
}

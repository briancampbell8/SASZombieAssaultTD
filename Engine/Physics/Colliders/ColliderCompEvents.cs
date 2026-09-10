// ====================================================================================================
//  FILE: ColliderCompEvents.cs
//  PATH: Engine/Physics/Components/
//  MODULE: Core
//
//  ROLE:
//      Collision event dispatch and debug output for ColliderComponent.
//
//  RESPONSIBILITIES:
//      - Provide TriggerCollisionEnter()
//      - Provide TriggerCollisionStay()
//      - Provide TriggerCollisionExit()
//      - Provide ToString() behavior
//
//  NON-RESPONSIBILITIES:
//      - Collision detection logic.
//      - Shape transforms or world‑space math (in ColliderCompCore.cs).
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Physics.Collision;
using static SASZombieAssaultTD.Engine.Physics.Collision.CollisionEnums;

namespace SASZombieAssaultTD.Engine.Physics
{
    public sealed class ColliderCompEvents
    {
        private CollisionShapeType? _shape;
        private object _mask;
        private object _isTrigger;
        private object _enabled;
        private object _layer;

        public event Action<CollisionEvent>? OnCollisionEnter;
        public event Action<CollisionEvent>? OnCollisionStay;
        public event Action<CollisionEvent>? OnCollisionExit;

        public void TriggerCollisionEnter(CollisionEvent collisionEvent)
        {
            OnCollisionEnter?.Invoke(collisionEvent);
        }

        public void TriggerCollisionStay(CollisionEvent collisionEvent)
        {
            OnCollisionStay?.Invoke(collisionEvent);
        }

        public void TriggerCollisionExit(CollisionEvent collisionEvent)
        {
            OnCollisionExit?.Invoke(collisionEvent);
        }

        public override string ToString()
        {
            return $"{base.ToString()} [Shape: {_shape}, " +
                $"Layer: {_layer}, Mask: {_mask}, Trigger: {_isTrigger}, Enabled: {_enabled}]";

        }
    }
}

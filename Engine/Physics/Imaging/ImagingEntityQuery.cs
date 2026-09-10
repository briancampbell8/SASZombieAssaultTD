// ====================================================================================================
//  FILE: ImagingEntityQuery.cs
//  PATH: Engine/Physics/Imaging/
//  MODULE: Imaging
// ====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Components;
using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Physics.Imaging
{
    /// <summary>
    /// Provides read‑only ECS ECSEntityCore queries for imaging modules.
    /// </summary>
    internal sealed class ImagingEntityQuery
    {
        private readonly ECSRuntimeCore _ecsWorld;

        public ImagingEntityQuery(ECSRuntimeCore ecsWorld)
        {
            _ecsWorld = ecsWorld;
        }

        /// <summary>
        /// Returns all entities containing ColliderCompCore + TransformComponent.
        /// </summary>
        public IEnumerable<ECSEntityCore> GetColliderEntities()
        {
            // Fixed CS1061: Use ActiveEntities list and manually check both component criteria
            foreach (var ECSEntityCore in _ecsWorld.ActiveEntities)
            {
                var collider = _ecsWorld.GetComponent<Colliders.ColliderCompCore>(ECSEntityCore);
                var transform = _ecsWorld.GetComponent<TransformComponent>(ECSEntityCore);

                if (collider != null && transform != null)
                    yield return ECSEntityCore;
            }
        }

        /// <summary>
        /// Returns all entities containing TransformComponent only.
        /// </summary>
        public IEnumerable<ECSEntityCore> GetTransformEntities()
        {
            // Fixed CS1579: Iterate over ActiveEntities (IEnumerable<ECSEntityCore>) instead of the missing property
            foreach (var ECSEntityCore in _ecsWorld.ActiveEntities)
            {
                var transform = _ecsWorld.GetComponent<TransformComponent>(ECSEntityCore);
                if (transform != null)
                    yield return ECSEntityCore;
            }
        }

        /// <summary>
        /// Returns all entities containing ColliderCompCore only.
        /// </summary>
        public IEnumerable<ECSEntityCore> GetColliderOnlyEntities()
        {
            // Fixed CS1579: Iterate over ActiveEntities (IEnumerable<ECSEntityCore>) instead of the missing property
            foreach (var ECSEntityCore in _ecsWorld.ActiveEntities)
            {
                var collider = _ecsWorld.GetComponent<Colliders.ColliderCompCore>(ECSEntityCore);
                var transform = _ecsWorld.GetComponent<TransformComponent>(ECSEntityCore);

                if (collider != null && transform == null)
                    yield return ECSEntityCore;
            }
        }
    }
}

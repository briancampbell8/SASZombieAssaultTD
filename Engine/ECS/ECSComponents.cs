// ====================================================================================================
//  FILE: ECSComponents.cs
//  PATH: Engine/ECS/ECSComponents/ECSComponents.cs
//  MODULE: ECS
//  AUTHOR: BDC
//  PURPOSE:
//      Minimal, modernized, deterministic component subsystem.
//
//      This file is intentionally small. It handles ONLY the core operations:
//          • AddComponent
//          • RemoveComponent
//          • GetComponent
//          • TryGetComponent
//          • HasComponent
//          • GetAllComponents
//
//      Additional behavior (SaveComponent, UseComponent, UpdateComponent, etc.)
//      will be added later *as the engine requires it*.
//
//  DESIGN PRINCIPLES:
//      • Do one thing extremely well.
//      • Deterministic behavior.
//      • No legacy per‑ECSEntityCore component storage.
//      • No runtime wrappers.
//      • No duplication across ECSRuntimeCore or ECSEntityCore.
//      • Grow the subsystem only when needed.
//
//  ROLE:
//      This file is the authoritative source for component storage and retrieval.
//
//  NON-RESPONSIBILITIES:
//      • Entity lifecycle
//      • System execution
//      • Runtime orchestration
// ====================================================================================================

using System.Collections.Generic;
using System.Numerics;

namespace SASZombieAssaultTD.Engine.ECS
{
    /// <summary>
    /// Minimal, authoritative component subsystem. Stores and retrieves component instances deterministically.
    /// </summary>
    public class ECSComponents
    {
        // ---------------------------------------------------------------------------------------------
        // INTERNAL STORAGE
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Component storage: EntityID → List<ComponentInstance>
        /// Deterministic, minimal, and modern.
        /// </summary>
        private readonly Dictionary<uint, List<object>> _components =
            new Dictionary<uint, List<object>>();

        public int Count => _components.Count;

        public ECSEntityCore Owner { get; internal set; }
        public ECSEntityCore Parent { get; internal set; }

        // ---------------------------------------------------------------------------------------------
        // ADD COMPONENT
        // ---------------------------------------------------------------------------------------------

        public void AddComponent<T>(uint ECSEntityCoreId, T component) where T : class
        {
            if (!_components.TryGetValue(ECSEntityCoreId, out var list))
            {
                list = new List<object>();
                _components[ECSEntityCoreId] = list;
            }

            list.Add(component);
        }

        // ---------------------------------------------------------------------------------------------
        // REMOVE COMPONENT
        // ---------------------------------------------------------------------------------------------

        public void RemoveComponent<T>(uint ECSEntityCoreId) where T : class
        {
            if (_components.TryGetValue(ECSEntityCoreId, out var list))
            {
                list.RemoveAll(c => c is T);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // GET COMPONENT
        // ---------------------------------------------------------------------------------------------

        public T GetComponent<T>(uint ECSEntityCoreId) where T : class
        {
            if (_components.TryGetValue(ECSEntityCoreId, out var list))
            {
                foreach (var c in list)
                {
                    if (c is T typed)
                        return typed;
                }
            }

            return null;
        }

        // ---------------------------------------------------------------------------------------------
        // TRY GET COMPONENT
        // ---------------------------------------------------------------------------------------------

        public bool TryGetComponent<T>(uint ECSEntityCoreId, out T component) where T : class
        {
            component = null;

            if (_components.TryGetValue(ECSEntityCoreId, out var list))
            {
                foreach (var c in list)
                {
                    if (c is T typed)
                    {
                        component = typed;
                        return true;
                    }
                }
            }

            return false;
        }

        // ---------------------------------------------------------------------------------------------
        // HAS COMPONENT
        // ---------------------------------------------------------------------------------------------

        public bool HasComponent<T>(uint ECSEntityCoreId) where T : class
        {
            return GetComponent<T>(ECSEntityCoreId) != null;
        }

        // ---------------------------------------------------------------------------------------------
        // GET ALL COMPONENTS
        // ---------------------------------------------------------------------------------------------

        public IEnumerable<T> GetAllComponents<T>(uint ECSEntityCoreId)
        {
            if (_components.TryGetValue(ECSEntityCoreId, out var list))
            {
                foreach (var c in list)
                {
                    if (c is T typed)
                        yield return typed;
                }
            }
        }

        // ---------------------------------------------------------------------------------------------
        // CLEAR COMPONENTS OF TYPE
        // ---------------------------------------------------------------------------------------------

        public void ClearComponents<T>(uint ECSEntityCoreId) where T : class
        {
            if (_components.TryGetValue(ECSEntityCoreId, out var list))
            {
                list.RemoveAll(c => c is T);
            }
        }

        // ---------------------------------------------------------------------------------------------
        // SAMPLE COMPONENT TYPES
        // ---------------------------------------------------------------------------------------------

        public class MovementComponent
        {
            public Vector3 Position { get; set; }
            public Vector2 Velocity { get; internal set; }
        }

        public class ActiveComponent
        {
            public bool IsActive { get; internal set; }
        }

        public class ScoreComponent
        {
            public int ScoreValue { get; internal set; }
        }
    }
}

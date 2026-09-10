//============================================================================
//FILE: Engine/Components/BaseComponent.cs
//AUTHOR: BDC
//PURPOSE:
//    Defines the foundational behavior for all ECS components.
//    Responsible for:
//      - Lifecycle callbacks (OnAttach, Update, OnDetach)
//      - Holding a reference to the owning Entity
//      - Providing a consistent, minimal API surface for all components
//
//DESIGN PRINCIPLES:
//    • Minimal, stable API surface
//    • No assumptions about systems or world-level behavior
//    • Deterministic lifecycle ordering
//    • Windsurf‑clean: no undefined symbols, no namespace drift
//
//NOTES:
//    All components in the engine must inherit from BaseComponent.
//    This file completes the ECS core triad:
//        1. ECSWorld
//        2. Entity
//        3. BaseComponent
//============================================================================

using SASZombieAssaultTD.Engine.ECS;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Base class for all ECS components. Provides lifecycle hooks and a reference to the owning ECSEntityCore.
    /// </summary>
    public abstract class BaseComponent
    {
        //--------------------------------------------------------------------
        //PUBLIC API — ENTITY REFERENCE
        //--------------------------------------------------------------------

        /// <summary>
        /// The ECSEntityCore that owns this component. Set internally by Entity.AddComponent and cleared on removal.
        /// </summary>
        public ECSEntityCore Entity { get; internal set; }

        /// <summary>
        /// Gets whether this component is currently attached to an ECSEntityCore.
        /// </summary>
        public bool IsAttached => Entity != null;

        //--------------------------------------------------------------------
        //LIFECYCLE CALLBACKS
        //--------------------------------------------------------------------

        /// <summary>
        /// Called when the component is attached to an ECSEntityCore. Override to initialize component state.
        /// </summary>
        public virtual void OnAttach()
        { }

        /// <summary>
        /// Called once per world update while the component is attached. Override to implement per-frame behavior.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
        public virtual void Update(float deltaTime)
        { }

        /// <summary>
        /// Called when the component is removed or the ECSEntityCore is destroyed. Override to clean up component state.
        /// </summary>
        public virtual void OnDetach()
        { }

        //--------------------------------------------------------------------
        //DEBUGGING
        //--------------------------------------------------------------------

        /// <summary>
        /// Returns a human-readable summary of the component.
        /// </summary>
        public override string ToString()
        {
            var ECSEntityCoreId = IsAttached ? Entity.Id.ToString() : "None";
            return $"{GetType().Name} (Entity={ECSEntityCoreId})";
        }
    }
}

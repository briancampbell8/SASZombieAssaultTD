/*
File:    IEntityComponent.cs
Purpose: P11-12-01 - Base interface for all ECS components.
*/
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.ECS
{
    ///<summary>
    ///P11-12-01: Base interface for all entity components in the ECS system.
    ///</summary>
    public interface IEntityComponent
    {
        ///<summary>
        ///Gets the entity this component is attached to.
        ///Set by the ECS system when the component is attached.
        ///</summary>
        Entity? Owner { get; }

        ///<summary>
        ///Gets whether this component is currently enabled.
        ///Disabled components don't receive update calls.
        ///</summary>
        bool IsEnabled { get; set; }

        ///<summary>
        ///P11-12-04: Called when the component is attached to an entity.
        ///</summary>
        ///<param name="entity">The entity this component is being attached to.</param>
        void OnAttach(Entity entity);

        ///<summary>
        ///P11-12-04: Called when the component is detached from its entity.
        ///</summary>
        void OnDetach();

        ///<summary>
        ///P11-12-04: Called every frame when the component is enabled.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last frame.</param>
        void OnUpdate(float deltaTime);
    }
}





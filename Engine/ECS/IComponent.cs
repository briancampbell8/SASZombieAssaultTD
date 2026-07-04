using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.ECS
{
    ///<summary>
    ///Defines the contract for components in the ECS framework.
    ///</summary>
    public interface IComponent
    {
        ///<summary>
        ///Gets or sets the entity that owns this component.
        ///</summary>
        Entity Owner { get; set; }
        
        ///<summary>
        ///Gets or sets whether the component is active.
        ///</summary>
        bool IsActive { get; set; }
        
        ///<summary>
        ///Called when the component is added to an entity.
        ///</summary>
        void OnAdded();
        
        ///<summary>
        ///Called when the component is removed from an entity.
        ///</summary>
        void OnRemoved();
    }
}





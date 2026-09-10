// =====================================================================================================
//  FILE: RenderableComponent.cs
//  PATH: Engine/Components/RenderableComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core engine component for representing renderable state. Stores sprite identifiers,
//      positional data, Z-index ordering, and visibility flags used by rendering and gameplay
//      systems.
//
//  RESPONSIBILITIES:
//      - Store sprite ID and render name metadata
//      - Store screen-space position (X, Y)
//      - Store Z-index for render ordering
//      - Store visibility state for conditional rendering
//
//  NON-RESPONSIBILITIES:
//      - Executing rendering commands or drawing sprites
//      - Managing world-level ECSEntityCore lifecycle or ECS attachment
//      - Acting as a rendering system or orchestrator
//
//  ARCHITECTURAL NOTES:
//      - This is a pure engine component with no behavioral logic
//      - Integrates with rendering and gameplay systems but does not implement them
// =====================================================================================================


namespace SASZombieAssaultTD.Engine.Components
{
    ///<summary>
    ///Represents the rendering details of an ECSEntityCore.
    ///</summary>
    public class RenderableComponent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public string SpriteId { get; set; }
        public int ZIndex { get; set; }
        public bool IsVisible { get; set; }

        public RenderableComponent(string spriteId, int zIndex, bool isVisible)
        {
            SpriteId = spriteId;
            ZIndex = zIndex;
            IsVisible = isVisible;
        }

        public RenderableComponent() { }

    }
}

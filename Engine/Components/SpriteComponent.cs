// =====================================================================================================
//  FILE: SpriteComponent.cs
//  PATH: Engine/Components/SpriteComponent.cs
//  SUBSYSTEM: Engine Components
//
//  ROLE:
//      Core engine component for representing sprite rendering state. Stores sprite index,
//      transform offsets, color tint, visibility flags, render-layer metadata, and optional
//      animation-time values used by rendering and gameplay systems.
//
//  RESPONSIBILITIES:
//      - Store sprite index and sprite-name metadata
//      - Store transform offsets for render positioning
//      - Store color tint values for visual effects
//      - Store visibility, enablement, and render-layer information
//      - Store optional animation-time values for systems that read them
//
//  NON-RESPONSIBILITIES:
//      - Executing animation playback or state-machine logic
//      - Dispatching animation events or handling callbacks
//      - Managing world-level ECSEntityCore lifecycle or ECS attachment
//      - Performing rendering operations or GPU commands
//
//  ARCHITECTURAL NOTES:
//      - This is a pure engine component with no behavioral logic
//      - Integrates with rendering and animation systems but does not implement them
//      - Kept lightweight to preserve subsystem boundaries
// =====================================================================================================


namespace SASZombieAssaultTD.Engine.Components
{
    internal class SpriteComponent
    {
        //public object Renderable { get; set; }
        public int SpriteIndex { get; set; }
        public (float X, float Y) TransformOffset { get; set; }
        public (float R, float G, float B, float A) ColorTint { get; set; }
        public float AnimationTime { get; set; }

        public SpriteComponent(object renderable) => this.IsRenderable = (bool)renderable;

        public bool IsRenderable { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEnabled { get; set; }
        public int RenderLayer { get; set; }
        public int RenderWidth { get; set; }
        public int RenderHeight { get; set; }
        public int RenderX { get; set; }
        public int RenderY { get; set; }
        public string RenderName { get; set; }
        public string RenderSpriteName { get; set; }


        public int VersionId { get; set; }
        public int Idx { get; set; }
        public int Entity { get; set; }
        public int ComponentId { get; set; }
        public int Type { get; set; }
        public int Version { get; set; }
        public int Index { get; set; }
        public int EntityId { get; set; }
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public int Layer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public string SpriteName { get; set; } = string.Empty;
        public string AnimationClip { get; internal set; }
        public object Transform { get; internal set; }

        public struct Renderable
        {

            public bool IsRenderable { get; set; }
            public bool IsVisible { get; set; }
            public bool IsEnabled { get; set; }
            public int RenderLayer { get; set; }
            public int RenderWidth { get; set; }
            public int RenderHeight { get; set; }
            public int RenderX { get; set; }
            public int RenderY { get; set; }
            public string RenderName { get; set; }
            public string RenderSpriteName { get; set; }
        }
        public class Sprite
        {
            public int SpriteIndex { get; set; }
            public (float X, float Y) TransformOffset { get; set; }
            public (float R, float G, float B, float A) ColorTint { get; set; }
            public float AnimationTime { get; set; }
        }
    }
}

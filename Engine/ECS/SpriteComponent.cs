using SASZombieAssaultTD.Engine.Components;

namespace SASZombieAssaultTD.Engine.ECS
{
    // Compatibility wrapper so code referencing SASZombieAssaultTD.Engine.ECS.SpriteComponent
    // can continue to work while the authoritative component resides in Engine.Components.
    public class SpriteComponent : SASZombieAssaultTD.Engine.Components.SpriteComponent
    {
        public SpriteComponent() : base() { }
        public SpriteComponent(string assetId) : base(assetId) { }
    }
}

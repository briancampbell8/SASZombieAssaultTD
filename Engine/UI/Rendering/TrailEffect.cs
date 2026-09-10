// ====================================================================================================
//  FILE: TrailEffect.cs
//  PATH: ./Engine/Rendering/
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide ToString() behavior for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Lightweight representation of a visual trail effect attached to projectiles. Kept intentionally small: stores a
    /// color and a width so existing code can construct one.
    /// </summary>
    public sealed class TrailEffect
    {
        public Color Color { get; }
        public float Width { get; }

        public TrailEffect(Color color, float width)
        {
            Color = color;
            Width = width;
        }

        public override string ToString() => $"TrailEffect(Color={Color}, Width={Width})";
    }
}

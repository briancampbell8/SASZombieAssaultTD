using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Lightweight representation of a visual trail effect attached to projectiles.
    ///Kept intentionally small: stores a color and a width so existing code can construct one.
    ///</summary>
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

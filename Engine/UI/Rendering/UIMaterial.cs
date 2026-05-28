
using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.Rendering;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// UI material interface for rendering system.
    /// </summary>
    // DUPLICATE - Properly referenced in Engine\UI\UIMaterial.cs
    // This file contains duplicate UIMaterial definition - use Engine\UI\UIMaterial.cs for valid definition
    //
    public class UIMaterial
    {
        internal System.Drawing.Color Color;
        internal Texture2D Texture;

        public string Name { get; set; }
        public int Value { get; set; }
    }

    //
}

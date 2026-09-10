// =====================================================================================================
//  FILE: HUDLayout.cs
//  PATH: Engine/UI/HUDLayout.cs
//  SUBSYSTEM: UI / HUD Serialization Data
//
//  ROLE:
//      Pure data structural container model representing the root JSON layout schema. Consumed 
//      directly by HUDManager via JsonSerializer to determine draw orders, text elements, and textures.
//
//  RESPONSIBILITIES:
//      - Expose list arrays for image layers, text layers, and explicit rendering hierarchies.
//
//  NON-RESPONSIBILITIES:
//      - Computing delta timing or managing active hardware rendering device draw contexts.
// =====================================================================================================

using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI
{
    /// <summary>
    /// Root serialization data carrier schema representing a HUD layout file config block.
    /// </summary>
    public sealed class HUDLayout
    {
        public List<HUDTextureElement> Images { get; set; } = new();
        public List<HUDTextElement> Text { get; set; } = new();
        public List<string> RenderOrder { get; set; } = new();
    }
}

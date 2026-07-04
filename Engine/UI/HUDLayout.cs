// ====================================================================================================
//  FILE: HUDLayout.cs
//  PATH: Engine/UI/
//  MODULE: UI Data Models (HUD Layout)
//
//  ROLE:
//      Defines the JSON-backed data models and interfaces used to describe HUD layouts.
//
//  RESPONSIBILITIES:
//      - Provide strongly-typed classes for Images, Text, and RenderOrder entries in HUD JSON.
//      - Define IHUDElement interface used by the rendering pipeline.
//      - Offer simple Draw() helper for HUDTextureElement to forward to IDrawingContext.
//
//  NON-RESPONSIBILITIES:
//      - Actual rendering backend implementations (Rendering subsystem provides IDrawingContext).
//      - Validation or transformation of layout beyond basic JSON deserialization.
//
//  ARCHITECTURAL NOTES:
//      - These types should remain POCOs to keep JSON (de)serialization stable and predictable.
//      - Keep Draw() implementations lightweight and side-effect free.
// ====================================================================================================

using System.Collections.Generic;

using System.Numerics;

using System.Text.Json.Serialization;

using SASZombieAssaultTD.Engine.Rendering;

using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI

{
    ///<summary>

    ///Root HUD layout definition from JSON.

    ///</summary>

    public class HUDLayout

    {
        [JsonPropertyName("images")]
        public List<HUDTextureElement> Images { get; set; } = new();

        [JsonPropertyName("text")]
        public List<HUDTextElement> Text { get; set; } = new();

        [JsonPropertyName("renderOrder")]
        public List<string> RenderOrder { get; set; } = new();
    }

    ///<summary>

    ///Interface for all HUD elements.

    ///</summary>

    public interface IHUDElement

    {
        string Id { get; }

        int Layer { get; }

        void Draw(IDrawingContext context);
    }

    ///<summary>

    ///HUD texture element for static images.

    ///</summary>

    public class HUDTextureElement : IHUDElement

    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("layer")]
        public int Layer { get; set; }

        [JsonPropertyName("visible")]
        public bool Visible { get; set; } = true;

        [JsonIgnore]
        public object Texture { get; set; }

        public void Draw(IDrawingContext context)

        {
            if (!Visible || Texture == null)

                return;

            context.DrawSprite(Texture, X, Y, Width, Height, Color.FromArgb((byte)(1f * 255), (byte)(1f * 255), (byte)(1f * 255), (byte)(1f * 255)));
        }
    }
}

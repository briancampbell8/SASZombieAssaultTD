/*
Program Name: SASZombieAssaultTD
File Path: Engine/UI/HUDLayout.cs
Purpose: Data models for HUD JSON schema defining layout structure and element types.
Features:
  - Provides HUDLayout root class with Images, Text, and RenderOrder collections
  - Defines IHUDElement interface for all HUD elements with Id, Layer, and Draw methods
  - Includes HUDTextureElement class for static image elements with position, size, layer, and visibility
  - Supports JSON serialization with JsonPropertyName attributes
  - Provides Draw method for texture rendering with IDrawingContext
  - Supports layer-based rendering order control
*/

//

// *HUDLayout.cs

// * Data models for HUD JSON schema

using SASZombieAssaultTD.Engine.Diagnostics;

 //

using System.Collections.Generic;

using System.Numerics;

using System.Text.Json.Serialization;

using SASZombieAssaultTD.Engine.Rendering;

using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI

{
    /// <summary>

    /// Root HUD layout definition from JSON.

    /// </summary>

    public class HUDLayout

    {
        [JsonPropertyName("images")]
        public List<HUDTextureElement> Images { get; set; } = new();

        [JsonPropertyName("text")]
        public List<HUDTextElement> Text { get; set; } = new();

        [JsonPropertyName("renderOrder")]
        public List<string> RenderOrder { get; set; } = new();
    }

    /// <summary>

    /// Interface for all HUD elements.

    /// </summary>

    public interface IHUDElement

    {
        string Id { get; }

        int Layer { get; }

        void Draw(IDrawingContext context);
    }

    /// <summary>

    /// HUD texture element for static images.

    /// </summary>

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

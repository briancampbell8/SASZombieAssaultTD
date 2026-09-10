// =====================================================================================================
//  FILE: ImageElement.cs
//  PATH: Engine/UI/Elements/ImageElement.cs
//  SUBSYSTEM: UI Framework — Static Image Element
//
//  ROLE:
//      Deterministic UI element responsible for rendering a static texture (PNG/JPG) through the modern
//      RenderSystem pipeline. Provides position, size, tinting, and deterministic draw behavior without
//      animation or interaction.
//
//  RESPONSIBILITIES:
//      - Store a texture name referencing a GPU‑resident asset.
//      - Provide deterministic bounds via UIElementBase (position, size, visibility).
//      - Forward rendering to RenderSystem → IDrawingContext for GPU‑backed sprite drawing.
//      - Support optional tinting for UI styling.
//
//  NON‑RESPONSIBILITIES:
//      - Texture loading (handled by TextureManager).
//      - Material creation or atlas packing (handled by UITextureAtlasManager).
//      - Input handling or interaction logic.
//      - Performing any direct adapter‑level rendering.
//
//  ARCHITECTURAL NOTES:
//      - Mirrors Button.cs structure but without interaction.
//      - Fully integrated with the modern RenderSystem pipeline (no D3D11Adapter_Core dependency).
//      - Legacy adapter‑based rendering paths are deprecated and scheduled for removal.
// =====================================================================================================

using System.Drawing;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Elements;

namespace SASZombieAssaultTD.Engine.UI
{
    public sealed class ImageElement : UIElementBase
    {
        private RenderSystem? _renderSystem;

        /// <summary>
        /// The texture name as registered in TextureManager / UITextureAtlasManager.
        /// Example: "MainMenu.png"
        /// </summary>
        public string TextureName { get; set; } = string.Empty;

        /// <summary>
        /// Optional tint color. Defaults to white (no tint).
        /// </summary>
        public ColorRGBA Tint { get; set; } = ColorRGBA.White;

        /// <summary>
        /// Floating‑point size used for layout and rendering.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Floating‑point position used for layout and rendering.
        /// </summary>
        public Vector2 PositionVector { get; set; }

        // -------------------------------------------------------------------------------------------------
        // WIRING
        // -------------------------------------------------------------------------------------------------

        public void SetRenderSystem(RenderSystem system)
        {
            _renderSystem = system;
        }

        // -------------------------------------------------------------------------------------------------
        // UPDATE
        // -------------------------------------------------------------------------------------------------

        public override void Update(float deltaTime)
        {
            if (float.IsNaN(PositionVector.X) || float.IsNaN(PositionVector.Y))
                return;

            Position = new Point(
                (int)System.Math.Round(PositionVector.X),
                (int)System.Math.Round(PositionVector.Y));
        }

        // -------------------------------------------------------------------------------------------------
        // RENDER — Modern RenderSystem Pipeline
        // -------------------------------------------------------------------------------------------------

        public override void Render(SASZombieAssaultTD.Engine.Render.D3D11.Adapter.D3D11Adapter_Core adapter, float deltaTime)
        {
            if (!IsVisible || _renderSystem == null || string.IsNullOrWhiteSpace(TextureName))
                return;

            var bounds = new System.Drawing.Rectangle(
                (int)PositionVector.X,
                (int)PositionVector.Y,
                (int)Size.X,
                (int)Size.Y);

            _renderSystem.DrawSprite(
                textureName: TextureName,
                x: bounds.X,
                y: bounds.Y,
                width: bounds.Width,
                height: bounds.Height,
                tint: Tint
            );
        }
    }
}

// =====================================================================================================
//  FILE: IPrimitivesAdapter.cs
//  PATH: Engine/Render/Adapter/D3D11Adapter_Primitives.cs
//  SUBSYSTEM: Rendering / Adapter Primitives
//
//  ROLE:
//      Deterministic high‑level primitive subsystem for the D3D11 Adapter Pipeline.
//      Handles all sprite, texture, text, and clear operations routed from D3D11Adapter_Core.
//
//  RESPONSIBILITIES:
//      - Provide a stable API for drawing sprites, textures, and text.
//      - Provide deterministic clear and screen‑reset operations.
//      - Serve as the unified resource layer for the Adapter Pipeline.
//      - Maintain strict Option‑B separation: no device, swap‑chain, or GPU ownership.
//
//  NON‑RESPONSIBILITIES:
//      - Frame lifecycle control.
//      - Primitive drawing (handled by D3D11Adapter_Primitives implementation).
//      - Device creation, swap‑chain management, or backend context manipulation.
// =====================================================================================================

using System.Numerics;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter
{
    internal interface IPrimitivesAdapter
    {
        // -------------------------------------------------------------------------------------------------
        // Clear / Screen Reset
        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Clears the current render target to the specified color.
        /// High-level operation routed from D3D11Adapter_Core; does not own device or swap-chain.
        /// </summary>
        /// <param name="color">RGBA clear color.</param>
        void Clear(ColorRGBA color);

        /// <summary>
        /// Performs a full screen reset to a deterministic default color (e.g., black or engine-defined).
        /// </summary>
        void ClearScreen();

        // -------------------------------------------------------------------------------------------------
        // Textures / Sprites
        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Draws a texture identified by a deterministic name at the given position with a tint.
        /// Texture resolution and GPU resource ownership are handled by upstream systems (TextureManager, etc.).
        /// </summary>
        /// <param name="textureName">Deterministic texture ID.</param>
        /// <param name="position">Screen-space position.</param>
        /// <param name="tint">RGBA tint color.</param>
        void DrawTexture(string textureName, Vector2 position, ColorRGBA tint);

        /// <summary>
        /// Draws a sprite identified by a deterministic name at the given position with a tint.
        /// Sprite composition (atlas, frames, etc.) is handled by higher-level systems.
        /// </summary>
        /// <param name="spriteName">Deterministic sprite ID.</param>
        /// <param name="position">Screen-space position.</param>
        /// <param name="tint">RGBA tint color.</param>
        void DrawSprite(string spriteName, Vector2 position, ColorRGBA tint);

        // -------------------------------------------------------------------------------------------------
        // Text Rendering
        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Draws text using a deterministic font ID at the given position with a tint.
        /// Font loading and glyph atlas management are handled by higher-level font systems.
        /// </summary>
        /// <param name="fontId">Deterministic font ID.</param>
        /// <param name="text">Text to render.</param>
        /// <param name="position">Screen-space position.</param>
        /// <param name="tint">RGBA tint color.</param>
        void DrawText(string fontId, string text, Vector2 position, ColorRGBA tint);

        /// <summary>
        /// Measures the rendered size of text using a deterministic font ID.
        /// Used by UI layout systems; does not perform any rendering.
        /// </summary>
        /// <param name="fontId">Deterministic font ID.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Measured size in screen-space units.</returns>
        Vector2 MeasureText(string fontId, string text);
    }
}

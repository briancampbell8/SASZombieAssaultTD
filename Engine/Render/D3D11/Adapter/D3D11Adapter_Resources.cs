// =====================================================================================================
//  FILE: D3D11Adapter_Resources.cs
//  PATH: Engine/Render/Adapter/D3D11Adapter_Resources.cs
//  SUBSYSTEM: Rendering / Adapter Resources
//
//  ROLE:
//      Deterministic high‑level resource subsystem for the D3D11 Adapter Pipeline.
//      Routes sprite, texture, and text operations to the RenderSystem.
//      Owns no GPU resources and performs no device/swap‑chain logic.
//
//  RESPONSIBILITIES:
//      - Provide a stable API for drawing sprites, textures, and text.
//      - Provide deterministic clear and screen‑reset operations.
//      - Serve as the unified resource layer for the Adapter Pipeline.
//      - Maintain strict Option‑B separation.
//
//  NON‑RESPONSIBILITIES:
//      - Primitive drawing (handled by D3D11AdapterDrawPrimitives).
//      - GPU resource creation.
//      - Swap‑chain management.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.Render.Sprites;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter
{
    public sealed class D3D11Adapter_Resources : ID3D11Subsystem
    {
        private readonly RenderSystem _renderSystem;
        private readonly D3D11DeviceCore _deviceCore;

        // SINGLE AUTHORITATIVE CONSTRUCTOR
        public D3D11Adapter_Resources(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));

            // FIX: Instantiate or acquire RenderSystem using the hardware context so it is never null
            _renderSystem = new RenderSystem(deviceCore); // If RenderSystem expects parameters, adapt as needed
        }

        public D3D11Adapter_Resources(RenderSystem renderSystem)
        {
            _renderSystem = renderSystem ?? throw new ArgumentNullException(nameof(renderSystem));
        }

        // -------------------------------------------------------------------------------------------------
        // LIFECYCLE (NO GPU OWNERSHIP)
        // -------------------------------------------------------------------------------------------------
        public void Initialize() { }
        public void Shutdown() { }
        public void BeginFrame() { }
        public void EndFrame() { }

        // -------------------------------------------------------------------------------------------------
        // CLEAR OPERATIONS
        // -------------------------------------------------------------------------------------------------
        public void Clear(ColorRGBA color)
        {
            if (_renderSystem == null)
                throw new InvalidOperationException("Cannot Clear: RenderSystem backend is not initialized.");

            _renderSystem.Clear(color.ToColor());
        }

        public void Clear(float r, float g, float b, float a)
            => _renderSystem.Clear(new ColorRGBA((byte)r, (byte)g, (byte)b, (byte)a).ToColor());

        public void ClearScreen()
            => _renderSystem.ClearScreen();

        // -------------------------------------------------------------------------------------------------
        // SPRITE OPERATIONS
        // -------------------------------------------------------------------------------------------------
        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, ColorRGBA tint)
            => _renderSystem.DrawSprite(texture, position, size, tint.ToColor());

        public void DrawSprite(string textureName, Vector2 position, Vector2 size, ColorRGBA tint)
        {
            var sprite = SpriteCache.GetSprite(textureName);
            _renderSystem.DrawSprite(sprite.Texture, position, size, tint.ToColor());
        }

        // -------------------------------------------------------------------------------------------------
        // TEXTURE OPERATIONS
        // -------------------------------------------------------------------------------------------------
        public void DrawTexture(Texture2D texture, Core.Rectangle rect, ColorRGBA tint)
            => _renderSystem.DrawTexture(texture, rect, tint.ToColor());

        public void DrawTexture(string textureName, Vector2 position, ColorRGBA tint)
        {
            var sprite = SpriteCache.GetSprite(textureName);
            var rect = new Core.Rectangle(
                (int)position.X,
                (int)position.Y,
                (float)sprite.Width,
                (float)sprite.Height);
            _renderSystem.DrawTexture(sprite.Texture, rect, tint.ToColor());
        }

        // -------------------------------------------------------------------------------------------------
        // TEXT OPERATIONS
        // -------------------------------------------------------------------------------------------------
        public void DrawText(string text, Vector2 position, float size, ColorRGBA color)
            => _renderSystem.DrawText(text, position, size, color.ToColor());

        public Vector2 MeasureText(string text, float size)
            => _renderSystem.MeasureText(text, size);

        // -------------------------------------------------------------------------------------------------
        // SERVICE ACCESSOR (OPTION‑B SAFE)
        // -------------------------------------------------------------------------------------------------
        public T GetService<T>()
            => throw new InvalidOperationException("D3D11Adapter_Resources exposes no services.");

        internal void DrawSprite(string textureName, float x, float y, float width, float height, Color tint)
        {
            throw new NotImplementedException();
        }
    }

    internal static class ColorExtensions
    {
        public static Color ToColor(this ColorRGBA rgba)
            => new Color((byte)rgba.R, (byte)rgba.G, (byte)rgba.B, (byte)rgba.A);
    }
}

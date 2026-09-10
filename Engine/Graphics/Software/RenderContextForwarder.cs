// =====================================================================================================
//  FILE: RenderContextForwarder.cs
//  PATH: Engine/Graphics/Software/RenderContextForwarder.cs
//  SUBSYSTEM: Graphics Software
//
//  ROLE:
//      Forwards modern drawing operations (textures, sprites, shapes, text) to a backend render
//      implementation via delegates. This keeps the modern pipeline clean and decoupled from
//      legacy overloads and avoids any direct dependency on D3D11Adapter_Core.
//
//  RESPONSIBILITIES:
//      - Provide a small, deterministic façade for modern draw calls.
//      - Forward operations to a backend through strongly-typed delegates.
//      - Keep FramebufferContext / FramebufferDrawing free of rendering logic.
//
//  NON-RESPONSIBILITIES:
//      - Legacy overloads (handled by RenderContextForwarderLegacy).
//      - Pixel buffer management (FramebufferCore).
//      - Memory allocation (FramebufferAllocator).
//      - Frame lifecycle (FramebufferContext).
//
//  ARCHITECTURAL NOTES:
//      - No interface types are referenced here.
//      - Backend behavior is injected via a simple delegate bundle.
//      - This class is intentionally small and focused.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Graphics.Software
{
    /// <summary>
    /// Thin forwarder that delegates modern drawing operations to a backend implementation
    /// via injected delegates. It does not implement or reference any render interfaces.
    /// </summary>
    internal sealed class RenderContextForwarder
    {
        /// <summary>
        /// Bundle of backend operations required for modern drawing.
        /// </summary>
        internal sealed class BackendOps
        {
            public Action<byte, byte, byte, byte> ClearRaw { get; init; }
            public Action<ColorRGBA> Clear { get; init; }
            public Action<Vector2, Vector2, ColorRGBA> FillRectangle { get; init; }
            public Action<Core.Rectangle, Vector2, Vector2, ColorRGBA, float> DrawRectangle { get; init; }
            public Action<Vector2, Vector2, ColorRGBA, float> DrawLine { get; init; }
            public Action<Vector2, float, ColorRGBA, float> DrawCircle { get; init; }
            public Action<Vector2, float, ColorRGBA> FillCircle { get; init; }
            public Action<string, int, Vector2, float, ColorRGBA> DrawText { get; init; }
            public Func<string, float, Vector2> MeasureText { get; init; }
            public Action<Texture2D, Vector2, ColorRGBA> DrawTexture { get; init; }
            public Action<Texture2D, Core.Rectangle, Vector2, ColorRGBA> DrawTextureRegion { get; init; }
            public Action<Texture2D, Vector2, Vector2, ColorRGBA> DrawSprite { get; init; }
            public Action<RenderCommand> Submit { get; init; }
        }

        private readonly BackendOps _ops;

        public RenderContextForwarder(BackendOps ops)
        {
            _ops = ops ?? throw new ArgumentNullException(nameof(ops));
        }

        // ----------------------------------------------------------------------------------------------
        //  Clear / pixel-level
        // ----------------------------------------------------------------------------------------------

        public void Clear(byte r, byte g, byte b, byte a = 255)
            => _ops.ClearRaw?.Invoke(r, g, b, a);

        public void Clear(ColorRGBA color)
            => _ops.Clear?.Invoke(color);

        // ----------------------------------------------------------------------------------------------
        //  Rectangles / lines / circles
        // ----------------------------------------------------------------------------------------------

        //public void FillRectangle(Vector2 position, Vector2 size, ColorRGBA color)
        //    => _ops.FillRectangle?.Invoke(position, size, color);  FILLTEST1
        public void FillRectangle(Vector2 position, Vector2 size, ColorRGBA color)
        => _ops.FillRectangle?.Invoke(position, size, new ColorRGBA(0, 0, 0, 0));


        public void DrawRectangle(Core.Rectangle rect, Vector2 position, Vector2 size, ColorRGBA color, float thickness = 1.0f)
            => _ops.DrawRectangle?.Invoke(rect, position, size, color, thickness);

        public void DrawLine(Vector2 start, Vector2 end, ColorRGBA color, float thickness = 1.0f)
            => _ops.DrawLine?.Invoke(start, end, color, thickness);

        public void DrawCircle(Vector2 center, float radius, ColorRGBA color, float thickness = 1.0f)
            => _ops.DrawCircle?.Invoke(center, radius, color, thickness);

        public void FillCircle(Vector2 center, float radius, ColorRGBA color)
            => _ops.FillCircle?.Invoke(center, radius, color);

        // ----------------------------------------------------------------------------------------------
        //  Text
        // ----------------------------------------------------------------------------------------------

        public void DrawText(string text, int fontId, Vector2 position, float size, ColorRGBA color)
            => _ops.DrawText?.Invoke(text, fontId, position, size, color);

        public Vector2 MeasureText(string text, float size)
            => _ops.MeasureText != null ? _ops.MeasureText(text, size) : Vector2.Zero;

        // ----------------------------------------------------------------------------------------------
        //  Textures / sprites
        // ----------------------------------------------------------------------------------------------

        public void DrawTexture(Texture2D texture, Vector2 position, ColorRGBA tint)
            => _ops.DrawTexture?.Invoke(texture, position, tint);

        public void DrawTextureRegion(Texture2D texture, Core.Rectangle sourceRect, Vector2 destination, ColorRGBA tint)
            => _ops.DrawTextureRegion?.Invoke(texture, sourceRect, destination, tint);

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, ColorRGBA tint)
            => _ops.DrawSprite?.Invoke(texture, position, size, tint);

        // ----------------------------------------------------------------------------------------------
        //  Command submission
        // ----------------------------------------------------------------------------------------------

        public void Submit(RenderCommand cmd)
            => _ops.Submit?.Invoke(cmd);
    }
}

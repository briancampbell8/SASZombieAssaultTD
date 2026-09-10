// =====================================================================================================
//  FILE: D3D11AdapterDrawPrimitives.cs
//  PATH: Engine/Render/D3D11/Adapter/D3D11AdapterDrawPrimitives.cs
//  SUBSYSTEM: Rendering Adapter — Primitive Routing
//
//  ROLE:
//      Deterministic routing layer for primitive drawing operations (lines, rectangles, circles).
//      Converts engine‑level types (ColorRGBA, Rectangle, Vector2) into backend‑ready types and
//      forwards them to the RenderSystem → IDrawingContext pipeline.
//
//  RESPONSIBILITIES:
//      - Convert engine primitive types into drawing‑context types.
//      - Forward all primitive draw calls to RenderSystem (Option‑B: no GPU ownership).
//      - Maintain a clean, minimal, deterministic API surface.
//      - Remain strictly stateless and side‑effect free.
//
//  NON‑RESPONSIBILITIES:
//      - GPU resource creation.
//      - Swap‑chain or device management.
//      - Software rendering or pixel rasterization.
//      - Sprite/text rendering (handled by D3D11Adapter_Resources).
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter
{
    public sealed class D3D11AdapterDrawPrimitives : ID3D11Subsystem
    {
        private readonly RenderSystem _backend;
        private readonly D3D11DeviceCore _deviceCore;

        // SINGLE AUTHORITATIVE CONSTRUCTOR
        public D3D11AdapterDrawPrimitives(D3D11DeviceCore deviceCore)
        {
            _deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));

            // FIX: Instantiate your RenderSystem using the hardware device context right on creation!
            _backend = new RenderSystem(deviceCore);
        }

        public D3D11AdapterDrawPrimitives(RenderSystem backend)
        {
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));
        }

        public void Initialize() { }
        public void Shutdown() { }
        public void BeginFrame() { }
        public void EndFrame() { }

        // -------------------------------------------------------------------------------------------------
        // COLOR CONVERSION
        // -------------------------------------------------------------------------------------------------

        private Color Convert(ColorRGBA c)
            => new Color((byte)c.R, (byte)c.G, (byte)c.B, (byte)c.A);

        private Color Convert(uint hex)
        {
            byte r = (byte)((hex >> 16) & 0xFF);
            byte g = (byte)((hex >> 8) & 0xFF);
            byte b = (byte)(hex & 0xFF);
            byte a = (byte)((hex >> 24) & 0xFF);
            return new Color(r, g, b, a);
        }

        // -------------------------------------------------------------------------------------------------
        // LINE DRAWING
        // -------------------------------------------------------------------------------------------------

        public void DrawLine(Vector2 start, Vector2 end, ColorRGBA color, float thickness = 1f)
        {
            _backend.DrawLine(start, end, Convert(color), thickness);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, uint hexColor, float thickness = 1f)
        {
            var s = new Vector2(x1, y1);
            var e = new Vector2(x2, y2);
            _backend.DrawLine(s, e, Convert(hexColor), thickness);
        }

        // -------------------------------------------------------------------------------------------------
        // RECTANGLES
        // -------------------------------------------------------------------------------------------------

        public void DrawRectangle(Core.Rectangle rect, ColorRGBA color, float thickness = 1f)
        {
            _backend.DrawRectangle(rect, Convert(color), thickness);
        }

        public void FillRectangle(Core.Rectangle rect, ColorRGBA color)
        {
            // RESTORED: Pointing back to your real engine color conversion!
            _backend.FillRectangle(rect, Convert(color));
        }

        public void DrawRectangle(Vector2 pos, Vector2 size, ColorRGBA color, float thickness = 1f)
        {
            var rect = new Core.Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            _backend.DrawRectangle(rect, Convert(color), thickness);
        }

        public void FillRectangle(Vector2 pos, Vector2 size, ColorRGBA color)
        {
            var rect = new Core.Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            _backend.FillRectangle(rect, Convert(color));
        }

        // -------------------------------------------------------------------------------------------------
        // CIRCLES
        // -------------------------------------------------------------------------------------------------

        public void DrawCircle(Vector2 center, float radius, ColorRGBA color, float thickness = 1f)
        {
            _backend.DrawCircle(center, radius, Convert(color), thickness);
        }

        public void FillCircle(Vector2 center, float radius, ColorRGBA color)
        {
            _backend.FillCircle(center, radius, Convert(color));
        }

        public void DrawCircle(int x, int y, int radius, uint hexColor)
        {
            _backend.DrawCircle(new Vector2(x, y), radius, Convert(hexColor), 1f);
        }
    }
}

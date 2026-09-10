// =====================================================================================================
//  FILE: D3D11Adapter_Core.cs
//  PATH: Engine/Render/D3D11/Adapter/D3D11Adapter_Core.cs
//  SUBSYSTEM: Rendering / Adapter Core
//
//  ROLE:
//      High-level deterministic routing core for rendering operations.
//      Delegates all sprite, texture, text, and primitive drawing to subsystem adapters.
//      Provides a unified API surface for RenderManager, UI systems, and gameplay systems.
//
//  RESPONSIBILITIES:
//      - Route clear/draw/text operations to D3D11Adapter_Resources.
//      - Route primitive operations to D3D11AdapterDrawPrimitives.
//      - Forward Present() directly to the active hardware IDXGISwapChain.
//      - Expose ManualClearColor for global clear-color control.
//      - Maintain strict Option-B separation: no direct GPU ownership.
//
//  NON-RESPONSIBILITIES:
//      - Device creation or destruction.
//      - Swap-chain management internals.
//      - Direct manipulation of ID3D11Device / IDXGISwapChain.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Platform;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Rendering.Modern;
using Vortice.Mathematics;
using CoreRect = SASZombieAssaultTD.Engine.Core.Rectangle;
using DrawRect = System.Drawing.Rectangle;
using MathVec2 = SASZombieAssaultTD.Engine.VectorMath.Vector2;
using MathVec3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;
using NumVec2 = System.Numerics.Vector2;
using NumVec3 = System.Numerics.Vector3;

namespace SASZombieAssaultTD.Engine.Render.D3D11.Adapter
{
    public sealed class D3D11Adapter_Core : ID3D11Subsystem
    {
        private readonly D3D11Adapter_Resources _resources;
        private readonly D3D11AdapterDrawPrimitives _primitives;
        private D3D11Window window;
        private D3D11DeviceCore deviceCore;
        private D3D11RenderContext hardwareGpuContext;

        public ColorRGBA ManualClearColor { get; set; } = new ColorRGBA(0, 0, 255, 255);

        public NumVec2 ViewportSize { get; internal set; }
        public float ScreenWidth { get; internal set; }
        public int ScreenHeight { get; internal set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public D3D11DeviceCore DeviceCore { get; internal set; }
        public ModernUIRenderer UIRenderer { get; internal set; }
        public object ClearFlags { get; set; }

        public D3D11Adapter_Core(D3D11Adapter_Resources resources, D3D11AdapterDrawPrimitives primitives)
        {
            _resources = resources;
            _primitives = primitives;
        }

        public D3D11Adapter_Core(D3D11Window window, D3D11DeviceCore deviceCore)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            this.deviceCore = deviceCore ?? throw new ArgumentNullException(nameof(deviceCore));
            this.DeviceCore = deviceCore;

            _resources = new D3D11Adapter_Resources(deviceCore);
            _primitives = new D3D11AdapterDrawPrimitives(deviceCore);

            SetScreenSize(1280, 720);
        }

        public D3D11Adapter_Core(D3D11RenderContext hardwareGpuContext)
        {
            this.hardwareGpuContext = hardwareGpuContext;
        }

        private void SetScreenSize(object width, object height)
        {
            static int ToInt(object value, string paramName)
            {
                if (value is null)
                    throw new ArgumentNullException(paramName);
                switch (value)
                {
                    case int i: return i;
                    case uint ui: return checked((int)ui);
                    case long l: return checked((int)l);
                    case short s: return s;
                    case byte b: return b;
                    case float f: return (int)f;
                    case double d: return (int)d;
                    case decimal dec: return (int)dec;
                    case string s when int.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed): return parsed;
                    case string s:
                        if (double.TryParse(s, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out var d2))
                            return (int)d2;
                        throw new ArgumentException($"String value for '{paramName}' is not a valid number.", paramName);
                    case IConvertible conv: return Convert.ToInt32(conv);
                    default: throw new ArgumentException($"Unsupported type '{value.GetType()}' for parameter '{paramName}'.", paramName);
                }
            }

            int w = ToInt(width, nameof(width));
            int h = ToInt(height, nameof(height));

            SetScreenSize(w, h);
        }

        public void Initialize() { }
        public void Shutdown() { }
        public void BeginFrame() { }
        public void EndFrame() { }
        public void Reset() { }

        private static ColorRGBA ToRGBA(Color c) => new ColorRGBA((byte)c.R, (byte)c.G, (byte)c.B, (byte)c.A);

        public void Clear(ColorRGBA color) => _resources.Clear(color);
        public void Clear(float r, float g, float b, float a) => _resources.Clear(new ColorRGBA((byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f), (byte)(a * 255f)));

        public void ClearScreen()
        {
            var rtv = deviceCore?.BackbufferRtv;
            var dsv = deviceCore?.DepthStencilView;

            if (deviceCore?.Context != null && rtv != null)
            {
                // Bind RTV + DSV
                deviceCore.Context.OMSetRenderTargets(rtv, dsv);

                // Convert ManualClearColor → Color4
                var c = ManualClearColor;
                var color4 = new Vortice.Mathematics.Color4(
                    c.R / 255f,
                    c.G / 255f,
                    c.B / 255f,
                    c.A / 255f);

                // Clear the swap-chain backbuffer
                deviceCore.Context.ClearRenderTargetView(rtv, color4);

                // Clear depth + stencil
                if (dsv != null)
                {
                    deviceCore.Context.ClearDepthStencilView(dsv, Vortice.Direct3D11.DepthStencilClearFlags.Depth | Vortice.Direct3D11.DepthStencilClearFlags.Stencil, 1.0f, 0);
                }    
            }
        }



        public void DrawSprite(Texture2D texture, NumVec2 position, NumVec2 size, ColorRGBA tint) => _resources.DrawSprite(texture, position, size, tint);
        public void DrawTexture(Texture2D texture, CoreRect rect, ColorRGBA tint) => _resources.DrawTexture(texture, rect, tint);
        public void DrawTexture(Texture2D texture, DrawRect rect, ColorRGBA tint) => _resources.DrawTexture(texture, rect, tint);
        public void DrawTexture(string textureName, NumVec2 position, ColorRGBA tint) => _resources.DrawTexture(textureName, position, tint);
        public void DrawText(string text, NumVec2 position, float size, ColorRGBA color) => _resources.DrawText(text, position, size, color);
        public NumVec2 MeasureText(string text, float size) => _resources.MeasureText(text, size);
        public T GetService<T>() => _resources.GetService<T>();

        public void DrawLine(int x, NumVec2 start, NumVec2 end, ColorRGBA color, float thickness = 1) => _primitives.DrawLine(start, end, color, thickness);
        public void DrawRectangle(CoreRect rect, ColorRGBA color, float thickness = 1) => _primitives.DrawRectangle(rect, color, thickness);
        public void DrawRectangle(NumVec2 position, NumVec2 size, ColorRGBA color, float thickness = 1) => _primitives.DrawRectangle(position, size, color, thickness);
        public void FillRectangle(CoreRect rect, ColorRGBA color) => _primitives.FillRectangle(rect, color);
        public void FillRectangle(NumVec2 position, NumVec2 size, ColorRGBA color) => _primitives.FillRectangle(position, size, color);
        public void DrawCircle(NumVec2 center, float radius, ColorRGBA color, float thickness = 1) => _primitives.DrawCircle(center, radius, color, thickness);
        public void FillCircle(NumVec2 center, float radius, ColorRGBA color) => _primitives.FillCircle(center, radius, color);

        internal void Present()
        {
            if (deviceCore?.SwapChain != null)
            {
                deviceCore.SwapChain.Present(1, Vortice.DXGI.PresentFlags.None);
            }
        }

        internal NumVec2 MeasureText(string v) => MeasureText(v, 1f);
        internal void DrawText(string text, float x, float y, float size, Color color) => _resources.DrawText(text, new NumVec2(x, y), size, ToRGBA(color));
        internal void DrawText(string text, int x, int y, float size, Color color) => _resources.DrawText(text, new NumVec2(x, y), size, ToRGBA(color));
        internal void DrawText(string text, int x, int y, Color color) => _resources.DrawText(text, new NumVec2(x, y), 1f, ToRGBA(color));
        internal void DrawText(string v, NumVec2 textPos) => _resources.DrawText(v, textPos, 1f, ManualClearColor);
        internal void DrawText(string text, NumVec3 pos, Color white) => _resources.DrawText(text, new NumVec2(pos.X, pos.Y), 1f, ToRGBA(white));
        internal void DrawText(string v, MathVec2 pos, float size, Color white) => _resources.DrawText(v, new NumVec2(pos.X, pos.Y), size, ToRGBA(white));
        internal void DrawText(string v1, MathVec3 pos, Color white, int v2) => _resources.DrawText(v1, new NumVec2(pos.X, pos.Y), 1f, ToRGBA(white));
        internal void DrawText(string v, float x, float y, Color white) => _resources.DrawText(v, new NumVec2(x, y), 1f, ToRGBA(white));
        internal void DrawText(string text, int x, float y, int fontSize, Color color) => _resources.DrawText(text, new NumVec2(x, y), fontSize, ToRGBA(color));
        internal void DrawText(string iconText, float x, float y, int fontSize, Color iconColor) => _resources.DrawText(iconText, new NumVec2(x, y), fontSize, ToRGBA(iconColor));
        internal void DrawText(string v1, int v2, int v3, float v4, ColorRGBA white) => _resources.DrawText(v1, new NumVec2(v2, v3), v4, white);
        internal void DrawSprite(string textureName, float x, float y, float width, float height, Color tint) => _resources.DrawTexture(textureName, new NumVec2(x, y), ToRGBA(tint));
        internal void DrawTexture(string textureName, float x, float y, float width, float height, Color tint) => _resources.DrawTexture(textureName, new NumVec2(x, y), ToRGBA(tint));

        internal void DrawTexture(object texture, int x, int y, int width, int height)
        {
            if (texture is Texture2D tex)
            {
                var rect = new CoreRect(x, y, width, height);
                _resources.DrawTexture(tex, rect, ManualClearColor);
            }
        }

        internal void DebugDrawTestPattern()
        {
            if (Width <= 0 || Height <= 0) return;
            var cx = Width / 4;
            var cy = Height / 4;
            var w = Width / 2;
            var h = Width / 2;
            var rect = new CoreRect(cx, cy, w, h);
            var red = new ColorRGBA(255, 0, 0, 255);
            _primitives.FillRectangle(rect, red);
        }

        internal void DrawTexture(Texture2D pixelRed, DrawRect dest, Color red)
        {
            var rect = new CoreRect(dest.X, dest.Y, dest.Width, dest.Height);
            _resources.DrawTexture(pixelRed, rect, ToRGBA(red));
        }

        internal void DrawTexture(Texture2D texture, Matrix3x2 transform, ColorRGBA tint)
        {
            var pos = new NumVec2(transform.M31, transform.M32);
            _resources.DrawSprite(texture, pos, new NumVec2(1f, 1f), tint);
        }

        internal void DrawTextureRegion(Texture2D texture, DrawRect sourceRect, NumVec2 destination, ColorRGBA tint)
        {
            var rect = new CoreRect((int)destination.X, (int)destination.Y, sourceRect.Width, sourceRect.Height);
            _resources.DrawTexture(texture, rect, tint);
        }

        internal void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            var rect = new CoreRect((int)x, (int)y, (int)width, (int)height);
            _primitives.DrawRectangle(rect, ToRGBA(color), 1f);
        }

        internal void DrawRectangle(DrawRect rect, MathVec3 position, Color color, int thickness)
        {
            var r = new CoreRect(rect.X + (int)position.X, rect.Y + (int)position.Y, rect.Width, rect.Height);
            _primitives.DrawRectangle(r, ToRGBA(color), thickness);
        }

        internal void DrawRectangle(DrawRect rect, Color color)
        {
            var r = new CoreRect(rect.X, rect.Y, rect.Width, rect.Height);
            _primitives.DrawRectangle(r, ToRGBA(color), 1f);
        }

        public void FillRectangle(DrawRect rectangle, Color color)
        {
            var rect = new CoreRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            _primitives.FillRectangle(rect, ToRGBA(color));
        }

        internal void DrawLine(int x1, int y1, int x2, int y2, uint pathColor)
        {
            var color = new ColorRGBA((byte)((pathColor >> 16) & 0xFF), (byte)((pathColor >> 8) & 0xFF), (byte)(pathColor & 0xFF), 255);
            _primitives.DrawLine(new NumVec2(x1, y1), new NumVec2(x2, y2), color, 1f);
        }

        internal void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            _primitives.DrawLine(new NumVec2(x1, y1), new NumVec2(x2, y2), ToRGBA(color), 1f);
        }

        internal void DrawCircle(int x, int y, int radius, uint pathColor)
        {
            var color = new ColorRGBA((byte)((pathColor >> 16) & 0xFF), (byte)((pathColor >> 8) & 0xFF), (byte)(pathColor & 0xFF), 255);
            _primitives.DrawCircle(new NumVec2(x, y), radius, color, 1f);
        }

        internal void DrawCircle(MathVec3 center, float radius, Color color)
        {
            _primitives.DrawCircle(new NumVec2(center.X, center.Y), radius, ToRGBA(color), 1f);
        }

        internal void Clear(Color color) => Clear(ToRGBA(color));

        internal void Render(TextureRendering.RenderCommand cmd) { }
        internal void DrawSprite(object handle, DrawRect rectangle, object value, Color white, float v1, NumVec2 zero, float v2) { }

        internal void DrawRectangle(CoreRect boundsRect, Color color, float thickness)
        {
            _primitives.DrawRectangle(boundsRect, ToRGBA(color), thickness);
        }

        internal void SetScreenSize(int width, int height)
        {
            Width = width;
            Height = height;
            ViewportSize = new NumVec2(width, height);
            ScreenWidth = width;
            ScreenHeight = height;
        }

        public class RenderQualityPreset { }
    }
}

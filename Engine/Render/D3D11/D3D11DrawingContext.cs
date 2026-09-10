// =====================================================================================================
//  FILE: D3D11DrawingContext.cs
//  PATH: Engine/Render/D3D11/D3D11DrawingContext.cs
//  SUBSYSTEM: Render Adapter / D3D11
//
//  ROLE:
//      GPU-backed implementation of IDrawingContext that routes all UI/HUD and gameplay draw operations
//      to the D3D11 adapter core. Provides a deterministic, backend-agnostic drawing surface while
//      binding directly to the active Direct3D 11 device and swap-chain.
//
//  RESPONSIBILITIES:
//      - Implement IDrawingContext for the D3D11 pipeline.
//      - Forward clear, primitive, sprite, texture, and text operations to D3D11Adapter_Core.
//      - Maintain viewport metrics (Width, Height, ViewportSize) in sync with the hardware device core.
//      - Provide a stable bridge between RenderSystem and the GPU-backed adapter.
//
//  NON-RESPONSIBILITIES:
//      - Creating or owning ID3D11Device, ID3D11DeviceContext, or IDXGISwapChain.
//      - Managing resource lifetime (textures, SRVs, buffers) beyond draw-time usage.
//      - Implementing high-level scene, UI, or gameplay logic.
//      - Handling windowing, input, or state management.
//
//  ARCHITECTURAL NOTES:
//      - This class is the D3D11-specific implementation of IDrawingContext used by RenderSystem.
//      - All draw calls from UI/HUD and gameplay layers flow through RenderSystem → D3D11DrawingContext
//        → D3D11Adapter_Core → D3D11DeviceCore.Context.
//      - ClearScreen() uses the adapter’s swap-chain-backed clear path (blue debug clear).
//      - Submit(RenderCommand) forwards directly to D3D11Adapter_Core.Render(RenderCommand) for
//        deterministic command routing.
// =====================================================================================================

using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.TextureRendering;
using SASZombieAssaultTD.Engine.UI.Rendering;
using Vector3 = SASZombieAssaultTD.Engine.VectorMath.Vector3;

namespace SASZombieAssaultTD.Engine.Render.D3D11
{
    internal sealed class D3D11DrawingContext : IDrawingContext
    {
        private readonly D3D11Adapter_Core _adapter;
        private readonly D3D11DeviceCore _deviceCore;

        private Vector2 _viewportSize;

        public D3D11DrawingContext(D3D11Adapter_Core adapter, D3D11DeviceCore deviceCore)
        {
            _adapter = adapter ?? throw new System.ArgumentNullException(nameof(adapter));
            _deviceCore = deviceCore ?? throw new System.ArgumentNullException(nameof(deviceCore));

            Width = _deviceCore.Width;
            Height = _deviceCore.Height;
            ViewportSize = new Vector2(Width, Height);
        }

        // -------------------------------------------------------------------------------------------------
        //  CLEAR OPERATIONS
        // -------------------------------------------------------------------------------------------------

        public void Clear(Color color)
        {
            _adapter.Clear(color);
        }

        public void Clear(float r, float g, float b, float a)
        {
            var color = new Color(
                (byte)(r * 255f),
                (byte)(g * 255f),
                (byte)(b * 255f),
                (byte)(a * 255f));

            _adapter.Clear(color);
        }

        public void ClearScreen()
        {
            _adapter.ClearScreen();
        }

        // -------------------------------------------------------------------------------------------------
        //  TEXT OPERATIONS
        // -------------------------------------------------------------------------------------------------

        public void DrawText(string text, Vector2 position, float size, Color color)
        {
            var pos = new System.Numerics.Vector2(position.X, position.Y);
            var rgba = ToRgba(color);
            _adapter.DrawText(text, pos, size, rgba);
        }

        public Vector2 MeasureText(string text, float size)
        {
            var measured = _adapter.MeasureText(text, size);
            return new Vector2(measured.X, measured.Y);
        }

        // -------------------------------------------------------------------------------------------------
        //  SPRITE / TEXTURE OPERATIONS
        // -------------------------------------------------------------------------------------------------

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, Color color)
        {
            var pos = new System.Numerics.Vector2(position.X, position.Y);
            var sz = new System.Numerics.Vector2(size.X, size.Y);
            var rgba = ToRgba(color);
            _adapter.DrawSprite(texture, pos, sz, rgba);
        }

        public void DrawTexture(Texture2D texture, Rectangle rect, Color color)
        {
            var coreRect = new Core.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            var rgba = ToRgba(color);
            _adapter.DrawTexture(texture, coreRect, rgba);
        }

        // -------------------------------------------------------------------------------------------------
        //  PRIMITIVE OPERATIONS
        // -------------------------------------------------------------------------------------------------

        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1)
        {
            var s = new System.Numerics.Vector2(start.X, start.Y);
            var e = new System.Numerics.Vector2(end.X, end.Y);
            var rgba = ToRgba(color);
            _adapter.DrawLine(0, s, e, rgba, thickness);
        }

        public void DrawRectangle(Rectangle rect, Color color, float thickness = 1)
        {
            var coreRect = new Core.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            var rgba = ToRgba(color);
            _adapter.DrawRectangle(coreRect, rgba, thickness);
        }

        public void FillRectangle(Rectangle rect, Color color)
        {
            var coreRect = new Core.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            var rgba = ToRgba(color);
            _adapter.FillRectangle(coreRect, rgba);
        }

        public void DrawCircle(Vector2 center, float radius, Color color, float thickness = 1)
        {
            var c = new System.Numerics.Vector2(center.X, center.Y);
            var rgba = ToRgba(color);
            _adapter.DrawCircle(c, radius, rgba, thickness);
        }

        public void FillCircle(Vector2 center, float radius, Color color)
        {
            var c = new System.Numerics.Vector2(center.X, center.Y);
            var rgba = ToRgba(color);
            _adapter.FillCircle(c, radius, rgba);
        }

        // -------------------------------------------------------------------------------------------------
        //  FRAME CONTROL (NO GPU OWNERSHIP)
        // -------------------------------------------------------------------------------------------------

        public void BeginBatch()
        {
            // No-op: batching handled by adapter/primitives.
        }

        public void EndBatch()
        {
            // No-op: batching handled by adapter/primitives.
        }

        public void Initialize()
        {
            // No-op: device/adapter already initialized by engine.
        }

        public void Shutdown()
        {
            // No-op: lifetime managed by engine host.
        }

        // -------------------------------------------------------------------------------------------------
        //  RENDER COMMAND SUBMISSION
        // -------------------------------------------------------------------------------------------------

        public void Submit(in RenderCommand cmd)
        {
            _adapter.Render(cmd);
        }

        public void FillRectangle(float x0, float y0, float v1, float v2, Color color)
        {
            var rect = new Core.Rectangle((int)x0, (int)y0, (int)v1, (int)v2);
            FillRectangle(rect, color);
        }

        public void DrawLine(int v1, Vector3 left, Vector3 mid, Color color, float v2)
        {
            var s = new Vector2(left.X, left.Y);
            var e = new Vector2(mid.X, mid.Y);
            DrawLine(s, e, color, v2);
        }

        // -------------------------------------------------------------------------------------------------
        //  VIEWPORT METRICS
        // -------------------------------------------------------------------------------------------------

        public Vector2 ViewportSize
        {
            get => _viewportSize;
            set
            {
                _viewportSize = value;
                Width = value.X;
                Height = value.Y;
                _adapter.SetScreenSize((int)value.X, (int)value.Y);
            }
        }

        public float Width { get; set; }
        public float Height { get; set; }

        // -------------------------------------------------------------------------------------------------
        //  INTERNAL HELPERS
        // -------------------------------------------------------------------------------------------------

        private static ColorRGBA ToRgba(Color color)
        {
            return new ColorRGBA(color.R, color.G, color.B, color.A);
        }
    }
}

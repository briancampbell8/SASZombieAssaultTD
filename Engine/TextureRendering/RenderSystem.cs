// =====================================================================================================
//  FILE: RenderSystem.cs
//  PATH: Engine/TextureRendering/RenderSystem.cs
//  SUBSYSTEM: Rendering Backend
//
//  ROLE:
//      GPU‑agnostic rendering router that forwards all draw operations to the active IDrawingContext.
//      RenderSystem provides a deterministic, unified surface for gameplay, HUD, and UI rendering,
//      while remaining fully decoupled from GPU device ownership and swap‑chain management.
//
//  RESPONSIBILITIES:
//      - Forward primitive, sprite, texture, and text operations to the active IDrawingContext.
//      - Maintain optional camera/view/projection transforms for gameplay/UI layers.
//      - Serve as the stable rendering entry point for all engine subsystems.
//      - Allow RenderManager to bind a GPU‑backed drawing context (D3D11DrawingContext).
//
//  NON‑RESPONSIBILITIES:
//      - Rasterization, GPU command submission, or pipeline binding (handled by D3D11DrawingContext).
//      - Resource creation, caching, or asset management.
//      - Frame lifecycle control (handled by RenderManager).
//      - Swap‑chain or device ownership.
//
//  ARCHITECTURAL NOTES:
//      - RenderSystem is intentionally thin: it forwards, never owns GPU logic.
//      - When RenderManager calls SetDrawingContext(), all RenderSystem draw calls become GPU‑backed.
//      - Clear operations route through IClearableDrawingContext when available.
// =====================================================================================================

using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SASZombieAssaultTD.Engine.Render.Sprites;
using SASZombieAssaultTD.Engine.UI.Rendering;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    public sealed class RenderSystem
    {
        private IDrawingContext _context;

        public void SetContext(IDrawingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Initialize() { }
        public void Shutdown() { }
        public void BeginFrame() { }
        public void EndFrame() { }

        // -------------------------------------------------------------------------------------------------
        //  PRIMITIVE OPERATIONS — ROUTED TO IDrawingContext
        // -------------------------------------------------------------------------------------------------

        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1f)
        {
            _context?.DrawLine(start, end, color, thickness);
        }

        public void DrawRectangle(Core.Rectangle rect, Color color, float thickness = 1f)
        {
            _context?.DrawRectangle(rect, color, thickness);
        }

        public void FillRectangle(Core.Rectangle rect, Color color)
        {
            _context?.FillRectangle(rect, color);
        }

        public void DrawCircle(Vector2 center, float radius, Color color, float thickness = 1f)
        {
            _context?.DrawCircle(center, radius, color, thickness);
        }

        public void FillCircle(Vector2 center, float radius, Color color)
        {
            _context?.FillCircle(center, radius, color);
        }

        // -------------------------------------------------------------------------------------------------
        //  SPRITE / TEXTURE OPERATIONS
        // -------------------------------------------------------------------------------------------------

        public void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, Color color)
        {
            _context?.DrawSprite(texture, position, size, color);
        }

        public void DrawTexture(Texture2D texture, Core.Rectangle rect, Color color)
        {
            _context?.DrawTexture(texture, rect, color);
        }

        // -------------------------------------------------------------------------------------------------
        //  TEXT OPERATIONS
        // -------------------------------------------------------------------------------------------------

        public void DrawText(string text, Vector2 position, float size, Color color)
        {
            _context?.DrawText(text, position, size, color);
        }

        public Vector2 MeasureText(string text, float size)
        {
            return _context?.MeasureText(text, size) ?? Vector2.Zero;
        }

        // -------------------------------------------------------------------------------------------------
        //  CAMERA TRANSFORMS (OPTIONAL)
        // -------------------------------------------------------------------------------------------------

        private Matrix4x4 _view = Matrix4x4.Identity;
        private Matrix4x4 _proj = Matrix4x4.Identity;
        private D3D11DeviceCore deviceCore;

        public RenderSystem(D3D11DeviceCore deviceCore)
        {
            this.deviceCore = deviceCore;
        }

        public void SetCamera(Matrix4x4 view, Matrix4x4 projection)
        {
            _view = view;
            _proj = projection;
        }

        // -------------------------------------------------------------------------------------------------
        //  BATCHING / FLUSH (NO GPU OWNERSHIP)
        // -------------------------------------------------------------------------------------------------

        public void Flush()
        {
            // Deterministic no‑op — batching handled by adapters.
        }

        // -------------------------------------------------------------------------------------------------
        //  INTERNAL HELPERS (ROUTED TO CONTEXT)
        // -------------------------------------------------------------------------------------------------

        internal void DrawSprite(Sprite sprite, Vector2 position, Vector2 scale, Color color)
        {
            var size = new Vector2(scale.X, scale.Y);
            DrawSprite(sprite.Texture, position, size, color);
        }

        internal void DrawString(string text, Vector2 position, Color color, float size = 1f)
        {
            DrawText(text, position, size, color);
        }

        internal void DrawLine(Vector3 vector31, Vector3 vector32, Color color, int thickness)
        {
            var start = new Vector2(vector31.X, vector31.Y);
            var end = new Vector2(vector32.X, vector32.Y);
            DrawLine(start, end, color, thickness);
        }

        internal void DrawRectangle(Rectangle targetRect, Vector3 positionVec, Color color, int thickness)
        {
            var rect = new Core.Rectangle(
                (int)positionVec.X + targetRect.X,
                (int)positionVec.Y + targetRect.Y,
                targetRect.Width,
                targetRect.Height);

            DrawRectangle(rect, color, thickness);
        }

        internal void DrawCircle(Vector3 engineCenter, int radius, Color color)
        {
            var center = new Vector2(engineCenter.X, engineCenter.Y);
            DrawCircle(center, radius, color);
        }

        internal void DrawCheckmark(VectorMath.Vector3 position, Color color, float v)
        {
            var p = new Vector2(position.X, position.Y);
            var left = p + new Vector2(-v, 0);
            var mid = p + new Vector2(0, v);
            var right = p + new Vector2(v, -v);

            DrawLine(left, mid, color, 1f);
            DrawLine(mid, right, color, 1f);
        }

        internal void DrawX(VectorMath.Vector3 position, Color color, float v)
        {
            var p = new Vector2(position.X, position.Y);
            var a = p + new Vector2(-v, -v);
            var b = p + new Vector2(v, v);
            var c = p + new Vector2(-v, v);
            var d = p + new Vector2(v, -v);

            DrawLine(a, b, color, 1f);
            DrawLine(c, d, color, 1f);
        }

        internal void DrawSprite(Texture2D texture, Vector2 pos, VectorMath.Vector3Int size, Color? finalColor)
        {
            var s = new Vector2(size.X, size.Y);
            DrawSprite(texture, pos, s, finalColor ?? Color.White);
        }

        internal void DrawCircle(Vector2 worldPos, float currentRange, Color? rangeColor, float v)
        {
            DrawCircle(worldPos, currentRange, rangeColor ?? Color.White, v);
        }

        // -------------------------------------------------------------------------------------------------
        //  CLEAR OPERATIONS — ROUTED TO DRAWING CONTEXT
        // -------------------------------------------------------------------------------------------------

        internal void ClearScreen()
        {
            _context?.ClearScreen();
        }

        internal void Clear(Color color)
        {
            if (_context is IClearableDrawingContext clearable)
            {
                clearable.Clear(color);
                return;
            }

            _context?.Clear(color);
        }

        internal void DrawSprite(Texture2D hudRight, VectorMath.Vector2 rightPosition, VectorMath.Vector2 rightSize, ColorRGBA white)
        {
            throw new NotImplementedException();
        }

        internal void DrawTexture(Texture2D meanStreetsTexture, System.Drawing.Rectangle destRect, ColorRGBA white)
        {
            throw new NotImplementedException();
        }

        internal void DrawText(string v, Vector2 textPos, ColorRGBA black)
        {
            throw new NotImplementedException();
        }

        internal Vector2 MeasureText(string v)
        {
            throw new NotImplementedException();
        }

        internal void DrawRectangle(System.Drawing.Rectangle bounds, ColorRGBA white)
        {
            throw new NotImplementedException();
        }

        internal void DrawSprite(string textureName, int x, int y, int width, int height, ColorRGBA tint)
        {
            throw new NotImplementedException();
        }
    }
}

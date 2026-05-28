// ============================================================================
// File:    FramebufferControl.cs
// Path:    Engine/Rendering/Framebuffer/FramebufferControl.cs
// Purpose: Root driver for the Framebuffer subsystem.
// ============================================================================

using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Drawing;
using System.Security.AccessControl;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public partial class Framebuffer : IDrawingContext, IFrameBuffer, IDisposable
    {
        internal int Width;
        internal int Height;
        internal byte[] Pixels;
        private object TheType;
        private object TheMember;

        public Framebuffer(int width, int height)
        {
            Width = width;
            Height = height;
        }

        void IDrawingContext.DrawCircle(float x, float y, float radius, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawCircle(Vector3 center, float radius, Color color, float thickness)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawLine(int layer, Vector3 start, Vector3 end, Color color, float thickness)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawRectangle(float x, float y, float width, float height, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawRectangle(Func<float> x, Func<float> y, int width, int height, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawRectangle(int x, int y, int width, int height, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawRectangle(Rectangle rect, Color color, float thickness)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawTexture(object texture, Vector3 position, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.FillCircle(Vector3 center, float radius, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.FillRectangle(Rectangle rect, Color color)
        {
            NI.Hit();
        }

        public uint GetPixel(int x, int y)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.GetPixel - STUB PROCESSED");
            return default;
        }

        public void Clear()
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.Clear - STUB PROCESSED");
            Array.Fill(_pixels, 0u);
        }

        public void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            NI.Hit();
        }

        public void DrawMonospacedDigits(string cashStr, float textX, float textY, Color textColor, float v)
        {
            NI.Hit();
        }

        Vector3 IDrawingContext.ViewportSize
        {
            get => NI.Hit<Vector3>();
            set => NI.Hit();
        }

        int IFrameBuffer.Width
        {
            get
            {
                NotImplementedGuard.Hit($"{nameof(IFrameBuffer)}.{nameof(IFrameBuffer.Width)}: width not implemented yet.");
                return default; // placeholder to satisfy compiler
            }
        }

        int IFrameBuffer.Height
        {
            get
            {
                NotImplementedGuard.Hit($"{nameof(IFrameBuffer)}.{nameof(IFrameBuffer.Height)}: height not implemented yet.");
                return default; // placeholder to satisfy compiler
            }
        }


        public void DrawText(string text, int x, int y, int size) { }
        public void DrawTexture(Texture2D texture, Rectangle rect, Color color) { }

        public void DrawSprite(Sprite towerSprite, Vector3 towerPos, Vector3 size, Color c)
        {
            NI.Hit();
        }

        public void FillRectangle(Rectangle horizRect, Core.Colorize.Color crossColor)
        {
            NI.Hit();
        }

        public void DrawText(string label, Vector3 pos, object r, object g, object b, object a, float v)
        {
            NI.Hit();
        }

        public void DrawText(string text, System.Numerics.Vector3 vector3, float r, float g, float b, float a, float v)
        {
            NI.Hit();
        }

        public void DrawText(string value, int v1, int v2, Color color, float scale)
        {
            NI.Hit();
        }

        public void DrawSprite(object value, int x, int y, int width, int height, Color color)
        {
            NI.Hit();
        }

        public void DrawRectangle(RectangleF rect, Color color, float thickness = 1)
        {
            NI.Hit();
        }

        public void FillRectangle(RectangleF rect, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.Clear(Color color)
        {
            uint packed =
                ((uint)color.A << 24) |
                ((uint)color.R << 16) |
                ((uint)color.G << 8) |
                ((uint)color.B);

            Array.Fill(_pixels, packed);
        }

        void IDrawingContext.Clear(float r, float g, float b, float a)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawText(string text, Vector3 position, Color color, float size)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawText(string text, float x, float y, float size, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawText(string text, int x, int y, int size, Color color)
        {
            NI.Hit();
        }

        Vector3 IDrawingContext.MeasureText(string text, float size)
        {
            return NI.Hit<Vector3>();
        }

        void IDrawingContext.ClearScreen()
        {
            NI.Hit();
        }

        void IDrawingContext.Present()
        {
            NI.Hit();
        }

        void IDrawingContext.BeginBatch()
        {
            NI.Hit();
        }

        void IDrawingContext.EndBatch()
        {
            NI.Hit();
        }

        void IDrawingContext.Initialize()
        {
            NI.Hit();
        }

        void IDrawingContext.Shutdown()
        {
            NI.Hit();
        }

        void IDrawingContext.DrawSprite(object texture, float x, float y, float width, float height, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawSprite(object texture, float x, float y, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawSprite(object value, object x, object y, object width, object height, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawSprite(object value, object height, Color color)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawLine(int x1, int y1, int x2, int y2, uint pathColor)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawLine(int x, int y, int v1, int v2, object value)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawCircle(int x, int y, int v, uint pathColor)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawText(string stateText, int v1, int v2)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawText(string iconText, float x, float y, int fontSize, Core.Color iconColor)
        {
            NI.Hit();
        }

        void IDrawingContext.DrawRectangle(int x, int y, int cellSize1, int cellSize2, object color)
        {
            NI.Hit();
        }

        internal void Clear(int v)
        {
            Array.Fill(_pixels, (uint)v);
        }

        void IFrameBuffer.SetPixel(int x, int y, uint color)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        uint IFrameBuffer.GetPixel(int x, int y)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        void IFrameBuffer.Clear()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal void SetPixel(int v1, int v2, uint rgba)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        internal void ClearScreen()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }

        public void DrawTexture(Texture2D pixelRed, Rectangle dest, System.Drawing.Color red)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");
            throw new NotImplementedException();
        }
    }
}

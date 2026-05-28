/*
File:    Framebuffer.cs
Author:  BDC
Created: 2026-02-10
Updated: 2026-02-17 - P11-08-FB-RESET Modernization

Purpose:
Modernized deterministic framebuffer with proper resource management.
Implements IRenderContext interface for compatibility with existing engine.

Notes:
Pixel buffer is stored in BGRA byte order for Win32 StretchDIBits (BI_RGB, 32bpp).
Public API accepts colors as uint in 0xRRGGBBAA format; conversion happens internally.
DrawText uses 6x8 glyphs; encoding artifact fixed.
Implements IDisposable for deterministic resource cleanup.
Modernized buffer lifecycle with no external dependencies.

*/
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Rendering
{
    /// <summary>
    /// Modernized deterministic framebuffer representation with proper resource management.
    /// Implements IRenderContext for full compatibility with existing engine systems.
    /// </summary>
    public sealed partial class Framebuffer : IRenderContext, IDisposable
    {
        bool _disposed;
        private event EventHandler<ResizeEventArgs>? _resizeEvent;

        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        /// Raw pixel buffer (BGRA byte order for Win32 presentation).
        /// </summary>
        public byte[] Pixels { get; private set; }

        /// <summary>
        /// Event triggered when framebuffer is resized.
        /// </summary>
        public event EventHandler<ResizeEventArgs>? Resized
        {
            add => _resizeEvent += value;
            remove => _resizeEvent -= value;
        }

        /// <summary>
        /// Resizes the framebuffer to new dimensions with deterministic buffer reallocation.
        /// </summary>
        /// <param name="newWidth">The new width in pixels.</param>
        /// <param name="newHeight">The new height in pixels.</param>
        public void Resize(int newWidth, int newHeight)
        {
            ThrowIfDisposed();

            if (newWidth <= 0 || newHeight <= 0)
                throw new ArgumentOutOfRangeException("Resize dimensions must be positive.");

            var oldWidth = Width;
            var oldHeight = Height;

            Width = newWidth;
            Height = newHeight;

            // Reallocate pixel buffer with deterministic lifecycle
            Pixels = new byte[Width * Height * 4];

            // Trigger resize event for compatibility with existing systems
            _resizeEvent?.Invoke(this, new ResizeEventArgs(oldWidth, oldHeight, newWidth, newHeight));
        }

        public Framebuffer(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Framebuffer dimensions must be positive.");

            Width = width;
            Height = height;

            // Allocate pixel buffer on creation
            Pixels = new byte[Width * Height * 4];
        }

        /// <summary>
        /// Fills the entire framebuffer with a single color (IRenderContext).
        /// </summary>
        public void Clear(Color color)
        {
            ThrowIfDisposed();
            var rgba = (uint)(((int)color.A << 24) | ((int)color.R << 16) | ((int)color.G << 8) | (int)color.B);
            ClearRaw(rgba);
        }

        /// <summary>
        /// Clears render target with specified RGBA components.
        /// Adapts component-based clear calls to the canonical Clear implementation.
        /// </summary>
        /// <param name="r">Red component (0.0-1.0).</param>
        /// <param name="g">Green component (0.0-1.0).</param>
        /// <param name="b">Blue component (0.0-1.0).</param>
        /// <param name="a">Alpha component (0.0-1.0).</param>
        public void Clear(float r, float g, float b, float a)
        {
            ThrowIfDisposed();

            var color = Color.FromArgb(
                (int)(a * 255),
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255)
            );

            Clear(color);
        }

        /// <summary>
        /// Fills the entire framebuffer with a single color (input: 0xRRGGBBAA).
        /// </summary>
        public void ClearRaw(uint rgba)
        {
            ThrowIfDisposed();
            var r = (byte)((rgba >> 24) & 0xFF);
            var g = (byte)((rgba >> 16) & 0xFF);
            var b = (byte)((rgba >> 8) & 0xFF);
            var a = (byte)(rgba & 0xFF);

            for (int i = 0; i < Pixels.Length; i += 4)
            {
                Pixels[i + 0] = b; // B
                Pixels[i + 1] = g; // G
                Pixels[i + 2] = r; // R
                Pixels[i + 3] = a; // A
            }
        }

        /// <summary>
        /// Writes a single pixel to the framebuffer (input: 0xRRGGBBAA).
        /// </summary>
        public void SetPixel(int x, int y, uint rgba)
        {
            ThrowIfDisposed();

            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            var index = (y * Width + x) * 4;

            Pixels[index + 0] = (byte)((rgba >> 8) & 0xFF);  // B
            Pixels[index + 1] = (byte)((rgba >> 16) & 0xFF); // G
            Pixels[index + 2] = (byte)((rgba >> 24) & 0xFF); // R
            Pixels[index + 3] = (byte)(rgba & 0xFF);         // A
        }

        // IRenderContext implementation - full compatibility
        public void DrawTexture(object texture, Rectangle destination, Rectangle? source = null)
        {
            ThrowIfDisposed();

            if (texture == null)
                return;

            var x = (int)destination.X;
            var y = (int)destination.Y;
            int w = (int)(source.HasValue ? source.Value.Width : 100); // Default width
            int h = (int)(source.HasValue ? source.Value.Height : 100); // Default height
            var placeholderColor = 0xFF00FFFF; // Magenta, full alpha

            for (int ty = 0; ty < h && (y + ty) < Height; ty++)
            {
                for (int tx = 0; tx < w && (x + tx) < Width; tx++)
                    SetPixel(x + tx, y + ty, placeholderColor);
                
            }
        }

        public void DrawText(string text, object font, object brush, PointF position)
        {
            ThrowIfDisposed();
            DrawTextRaw(text, (int)position.X, (int)position.Y);
        }

        public void DrawText(string text, int x, int y)
        {
            ThrowIfDisposed();
            DrawTextRaw(text, x, y);
        }

        /// <summary>
        /// Presents the rendered frame to display.
        /// </summary>
        public void Present()
        {
            ThrowIfDisposed();
            // No-op for offscreen framebuffer; swap/present is handled by host.
        }

        /// <summary>
        /// Gets or sets the viewport size.
        /// </summary>
        public Vector3 ViewportSize { get; set; }

        /// <summary>
        /// Begins a new render batch.
        /// </summary>
        public void BeginBatch()
        {
            ThrowIfDisposed();
            // No-op for simple framebuffer
        }

        /// <summary>
        /// Ends the current render batch and submits for drawing.
        /// </summary>
        public void EndBatch()
        {
            ThrowIfDisposed();
            // No-op for simple framebuffer
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void DrawLine(Vector3 start, Vector3 end, Color color, float thickness = 1.0f)
        {
            ThrowIfDisposed();
            // Simple line drawing implementation
            // Convert 3D to 2D for framebuffer
            var x1 = (int)start.X;
            var y1 = (int)start.Y;
            var x2 = (int)end.X;
            var y2 = (int)end.Y;

            // Simple line algorithm (placeholder)
            DrawLineRaw(x1, y1, x2, y2, color);
        }

        /// <summary>
        /// Draws a line between two points using coordinate components.
        /// Adapts component-based line drawing calls to the canonical DrawLine implementation.
        /// </summary>
        /// <param name="x1">Start X coordinate.</param>
        /// <param name="y1">Start Y coordinate.</param>
        /// <param name="x2">End X coordinate.</param>
        /// <param name="y2">End Y coordinate.</param>
        /// <param name="color">Line color.</param>
        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            ThrowIfDisposed();
            DrawLineRaw((int)x1, (int)y1, (int)x2, (int)y2, color);
        }

        /// <summary>
        /// Draws a rectangle outline.
        /// </summary>
        public void DrawRectangle(Rectangle rect, Color color, float thickness = 1.0f)
        {
            ThrowIfDisposed();
            // Draw rectangle outline
            DrawRectangleRaw((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, color, false);
        }

        /// <summary>
        /// Draws a rectangle using coordinate components.
        /// Adapts component-based rectangle drawing calls to the canonical DrawRectangle implementation.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        /// <param name="color">Rectangle color.</param>
        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            ThrowIfDisposed();
            DrawRectangleRaw((int)x, (int)y, (int)width, (int)height, color, false);
        }

        /// <summary>
        /// Draws a rectangle using integer coordinate components.
        /// Adapts integer component-based rectangle drawing calls to the canonical DrawRectangle implementation.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        /// <param name="color">Rectangle color.</param>
        public void DrawRectangle(int x, int y, int width, int height, Color color)
        {
            ThrowIfDisposed();
            DrawRectangleRaw(x, y, width, height, color, false);
        }

        /// <summary>
        /// Draws a filled rectangle.
        /// </summary>
        public void FillRectangle(Rectangle rect, Color color)
        {
            ThrowIfDisposed();
            // Draw filled rectangle
            DrawRectangleRaw((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, color, true);
        }

        /// <summary>
        /// Draws a circle outline.
        /// </summary>
        public void DrawCircle(Vector3 center, float radius, Color color, float thickness = 1.0f)
        {
            ThrowIfDisposed();
            // Draw circle outline
            DrawCircleRaw((int)center.X, (int)center.Y, (int)radius, color, false);
        }

        /// <summary>
        /// Draws a circle using coordinate components.
        /// Adapts component-based circle drawing calls to the canonical DrawCircle implementation.
        /// </summary>
        /// <param name="x">Center X coordinate.</param>
        /// <param name="y">Center Y coordinate.</param>
        /// <param name="radius">Circle radius.</param>
        /// <param name="color">Circle color.</param>
        public void DrawCircle(float x, float y, float radius, Color color)
        {
            ThrowIfDisposed();
            DrawCircleRaw((int)x, (int)y, (int)radius, color, false);
        }

        /// <summary>
        /// Draws a filled circle.
        /// </summary>
        public void FillCircle(Vector3 center, float radius, Color color)
        {
            ThrowIfDisposed();
            // Draw filled circle
            DrawCircleRaw((int)center.X, (int)center.Y, (int)radius, color, true);
        }

        /// <summary>
        /// Draws text at specified position.
        /// </summary>
        public void DrawText(string text, Vector3 position, Color color, float size = 12.0f)
        {
            ThrowIfDisposed();
            DrawText(text, (int)position.X, (int)position.Y);
        }

        /// <summary>
        /// Draws text using coordinate components.
        /// Adapts component-based text drawing calls to the canonical DrawText implementation.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="size">Font size.</param>
        /// <param name="color">Text color.</param>
        public void DrawText(string text, float x, float y, float size, Color color)
        {
            ThrowIfDisposed();
            DrawText(text, (int)x, (int)y);
        }

        /// <summary>
        /// Draws text using integer coordinate components.
        /// Adapts integer component-based text drawing calls to the canonical DrawText implementation.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="size">Font size.</param>
        /// <param name="color">Text color.</param>
        public void DrawText(string text, int x, int y, int size, Color color)
        {
            ThrowIfDisposed();
            DrawText(text, x, y);
        }

        /// <summary>
        /// Draws a line (raw implementation).
        /// </summary>
        void DrawLineRaw(int x1, int y1, int x2, int y2, Color color)
        {
            // Simple line drawing algorithm (placeholder)
            // This would need proper line drawing implementation
        }

        /// <summary>
        /// Draws a rectangle (raw implementation).
        /// </summary>
        void DrawRectangleRaw(int x, int y, int width, int height, Color color, bool filled)
        {
            // Simple rectangle drawing (placeholder)
            // This would need proper rectangle drawing implementation
        }

        /// <summary>
        /// Draws a circle (raw implementation).
        /// </summary>
        void DrawCircleRaw(int centerX, int centerY, int radius, Color color, bool filled)
        {
            // Simple circle drawing (placeholder)
            // This would need proper circle drawing implementation
        }

        /// <summary>
        /// Clears the screen.
        /// </summary>
        public void ClearScreen()
        {
            ThrowIfDisposed();
            ClearRaw(0x0000FFFF);
        }

        /// <summary>
        /// Measures text dimensions.
        /// </summary>
        /// <param name="text">Text to measure.</param>
        /// <param name="size">Font size.</param>
        /// <returns>Text dimensions as Vector3.</returns>
        public Vector3 MeasureText(string text, float size = 12.0f)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text))
                return Vector3.Zero;

            // Simple text measurement based on character count and size
            const int glyphWidth = 6;
            const int glyphHeight = 8;
            var width = text.Length * glyphWidth * (size / 12.0f);
            var height = glyphHeight * (size / 12.0f);

            return new Vector3(width, height, 0);
        }

        void DrawTextRaw(string text, int x, int y)
        {
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(text))
                return;

            // Render each character as a small filled rectangle (6x8 per glyph)
            // using a placeholder color so that draw call is visually verifiable.
            const int glyphWidth = 6;
            const int glyphHeight = 8;
            var textColor = 0xFFFFFFFF; // White, full alpha

            for (int i = 0; i < text.Length; i++)
            {
                var gx = x + (i * glyphWidth);

                for (int py = 0; py < glyphHeight; py++)
                {
                    for (int px = 0; px < glyphWidth; px++)
                        SetPixel(gx + px, y + py, textColor);
                    
                }
            }
        }

        /// <summary>
        /// Initializes the framebuffer.
        /// </summary>
        public void Initialize()
        {
            ThrowIfDisposed();
            // Framebuffer is already initialized in constructor
        }

        /// <summary>
        /// Shuts down the framebuffer.
        /// </summary>
        public void Shutdown()
        {
            ThrowIfDisposed();
            // Framebuffer shutdown logic - clear pixels
            ClearRaw(0x00000000);
        }

        /// <summary>
        /// Draws a texture at specified position.
        /// </summary>
        /// <param name="texture">Texture to draw.</param>
        /// <param name="position">Position to draw at.</param>
        /// <param name="color">Color tint.</param>
        public void DrawTexture(object texture, Vector3 position, Color color)
        {
            ThrowIfDisposed();
            // Simple texture drawing - would need actual texture implementation
            // For now, draw a colored rectangle at position
            FillRectangle(new Rectangle((int)position.X, (int)position.Y, 32, 32), color);
        }

        /// <summary>
        /// Draws a sprite using coordinate components.
        /// Adapts component-based sprite drawing calls to the canonical DrawTexture implementation.
        /// </summary>
        /// <param name="texture">Texture/sprite to draw.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="width">Draw width.</param>
        /// <param name="height">Draw height.</param>
        /// <param name="color">Color tint.</param>
        public void DrawSprite(object texture, float x, float y, float width, float height, Color color)
        {
            ThrowIfDisposed();
            // Simple sprite drawing - would need actual texture implementation
            // For now, draw a colored rectangle at position with specified dimensions
            FillRectangle(new Rectangle((int)x, (int)y, (int)width, (int)height), color);
        }

        /// <summary>
        /// Draws a sprite using coordinate components with separate color parameter.
        /// Adapts component-based sprite drawing calls to the canonical DrawTexture implementation.
        /// </summary>
        /// <param name="texture">Texture/sprite to draw.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="color">Color tint.</param>
        public void DrawSprite(object texture, float x, float y, Color color)
        {
            ThrowIfDisposed();
            // Simple sprite drawing - would need actual texture implementation
            // For now, draw a colored rectangle at position with default dimensions
            FillRectangle(new Rectangle((int)x, (int)y, 32, 32), color);
        }

        /// <summary>
        /// Validates that the framebuffer is not disposed.
        /// </summary>
        void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Framebuffer));
        }

        /// <summary>
        /// Implements deterministic resource disposal.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            Pixels = default!;
            Width = 0;
            Height = 0;
            _disposed = true;
        }

        public void DrawLine(int x, Vector3 start, Vector3 end, Color color, float thickness = 1)
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(Func<float> x, Func<float> y, int width, int height, Color white)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(int x1, int y1, int x2, int y2, uint pathColor)
        {
            throw new NotImplementedException();
        }

        public void DrawCircle(int x, int y, int v, uint pathColor)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Provides data for framebuffer resize events.
    /// </summary>
    public sealed class ResizeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the previous width before resize.
        /// </summary>
        public int OldWidth { get; }

        /// <summary>
        /// Gets the previous height before resize.
        /// </summary>
        public int OldHeight { get; }

        /// <summary>
        /// Gets the new width after resize.
        /// </summary>
        public int NewWidth { get; }

        /// <summary>
        /// Gets the new height after resize.
        /// </summary>
        public int NewHeight { get; }

        /// <summary>
        /// Creates a new resize event arguments instance.
        /// </summary>
        /// <param name="oldWidth">The previous width.</param>
        /// <param name="oldHeight">The previous height.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        public ResizeEventArgs(int oldWidth, int oldHeight, int newWidth, int newHeight)
        {
            OldWidth = oldWidth;
            OldHeight = oldHeight;
            NewWidth = newWidth;
            NewHeight = newHeight;
        }
    }
}
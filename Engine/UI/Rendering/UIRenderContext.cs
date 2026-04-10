using SASZombieAssaultTD.Engine.Assets;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// UI-specific render context for UI rendering operations.
    /// </summary>
    public class UIRenderContext : IRenderContext
    {
        public float ScreenWidth => ViewportSize.X;
        public float ScreenHeight => ViewportSize.Y;

        public float Width => ViewportSize.X;
        public float Height => ViewportSize.Y;

        public Viewport Viewport { get; set; }

        // Corrected ViewportSize to explicitly use the canonical Vector3
        public SASZombieAssaultTD.Engine.VectorMath.Vector3 ViewportSize
        { get; set; } = SASZombieAssaultTD.Engine.VectorMath.Vector3.Zero;
        object IRenderContext.Viewport { get => Viewport; set => throw new NotImplementedException(); }

        public void Clear(Color color) { }

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
            var color = Color.FromArgb(
                (int)(a * 255),
                (int)(r * 255),
                (int)(g * 255),
                (int)(b * 255)
            );

            Clear(color);
        }

        public void DrawLine(SASZombieAssaultTD.Engine.VectorMath.Vector3 start, SASZombieAssaultTD.Engine.VectorMath.Vector3 end, Color color, float thickness = 1.0f) { }

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
            var start = new SASZombieAssaultTD.Engine.VectorMath.Vector3(x1, y1, 0);
            var end = new SASZombieAssaultTD.Engine.VectorMath.Vector3(x2, y2, 0);
            DrawLine(start, end, color);
        }

        public void DrawRectangle(Rectangle rect, Color color, float thickness = 1.0f) { }

        /// <summary>
        /// Draws a rectangle using the Rect structure.
        /// Implements the IRenderContext.DrawRectangle method.
        /// </summary>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Rectangle color</param>
        public void DrawRectangle(Rect rect, Color color)
        {
            // Convert Rect to Rectangle for internal implementation
            var rectangle = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            DrawRectangle(rectangle, color);
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
            var rect = new Rectangle((int)x, (int)y, (int)width, (int)height);
            DrawRectangle(rect, color);
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
            var rect = new Rectangle(x, y, width, height);
            DrawRectangle(rect, color);
        }

        public void FillRectangle(Rectangle rect, Color color) { }

        public void DrawCircle(SASZombieAssaultTD.Engine.VectorMath.Vector3 center, float radius, Color color, float thickness = 1.0f) { }

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
            var center = new SASZombieAssaultTD.Engine.VectorMath.Vector3(x, y, 0);
            DrawCircle(center, radius, color);
        }

        public void FillCircle(SASZombieAssaultTD.Engine.VectorMath.Vector3 center, float radius, Color color) { }

        public void DrawText(string text, SASZombieAssaultTD.Engine.VectorMath.Vector3 position, Color color, float size = 12.0f) { }

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
            var position = new SASZombieAssaultTD.Engine.VectorMath.Vector3(x, y, 0);
            DrawText(text, position, color, size);
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
            var position = new SASZombieAssaultTD.Engine.VectorMath.Vector3(x, y, 0);
            DrawText(text, position, color, size);
        }

        /// <summary>
        /// Draws text at the specified position with font.
        /// Implements the IRenderContext.DrawText method.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position for text (Z component ignored)</param>
        /// <param name="color">Text color</param>
        /// <param name="font">Font to use for text rendering</param>
        public void DrawText(string text, Vector3 position, Color color, Font font)
        {
            // Convert Font to size for internal implementation
            var fontSize = font?.Size ?? 12f;
            DrawText(text, position, color, fontSize);
        }

        /// <summary>
        /// Draws an image/texture at the specified rectangle.
        /// Implements the IRenderContext.DrawImage method.
        /// </summary>
        /// <param name="texture">Texture to render</param>
        /// <param name="rect">Destination rectangle for texture rendering</param>
        public void DrawImage(Texture texture, Rect rect)
        {
            // Convert Rect to Rectangle for internal implementation
            var rectangle = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            FillRectangle(rectangle, Color.White); // Placeholder implementation
        }

        // Overloads for KillFeedSystem compatibility
        public void DrawText(string text, float x, float y, Color color, float fontSize)
        {
            DrawText(text, new SASZombieAssaultTD.Engine.VectorMath.Vector3(x, y, 0), color, fontSize);
        }

        public void Present() { }

        public void BeginBatch() { }

        public void EndBatch() { }

        // Missing interface methods that need to be implemented
        public void ClearScreen() => Clear(Color.Black);

        /// <summary>
        /// Measures text dimensions.
        /// </summary>
        /// <param name="text">Text to measure.</param>
        /// <param name="size">Font size.</param>
        /// <returns>Text dimensions as Vector3.</returns>
        public SASZombieAssaultTD.Engine.VectorMath.Vector3 MeasureText(string text, float size = 12.0f)
        {
            // Simple text measurement
            var width = text.Length * size * 0.6f; // Approximate width
            var height = size;
            return new SASZombieAssaultTD.Engine.VectorMath.Vector3(width, height, 0);
        }

        /// <summary>
        /// Initializes the UI render context.
        /// </summary>
        public void Initialize()
        {
            // Initialize UI render context
            PlaceholderTexture.Initialize();
        }

        /// <summary>
        /// Shuts down the UI render context.
        /// </summary>
        public void Shutdown()
        {
            // Shutdown UI render context
        }

        /// <summary>
        /// Draws a texture at specified position.
        /// </summary>
        /// <param name="texture">Texture to draw.</param>
        /// <param name="position">Position to draw at.</param>
        /// <param name="color">Color tint.</param>
        public void DrawTexture(object texture, SASZombieAssaultTD.Engine.VectorMath.Vector3 position, Color color)
        {
            // Simple texture drawing for UI
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
            var position = new SASZombieAssaultTD.Engine.VectorMath.Vector3(x, y, 0);
            var rect = new Rectangle((int)x, (int)y, (int)width, (int)height);
            FillRectangle(rect, color);
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
            var position = new SASZombieAssaultTD.Engine.VectorMath.Vector3(x, y, 0);
            DrawTexture(texture, position, color);
        }

        // Additional methods for compatibility
        public void ClearClipRect() { }

        public void SetAlpha(float alpha) { }

        public void Reset() { }

        /// <summary>
        /// Sets the transformation matrix for UI rendering.
        /// </summary>
        /// <param name="transform">The transformation matrix to apply.</param>
        public void SetTransform(object transform)
        {
            // Placeholder implementation for UI transformation
            // In a full implementation, this would apply the transform to subsequent rendering operations
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

        public void DrawText(string stateText, int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public void DrawText(object line, int v, int y)
        {
            throw new NotImplementedException();
        }
    }
}
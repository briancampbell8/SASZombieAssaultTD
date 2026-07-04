/*
File:    IRenderContext.cs
Purpose: Core interface for rendering operations across the engine.
*/
using SASZombieAssaultTD.Engine.VectorMath;
using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Core interface for rendering operations.
    ///Provides abstraction for different rendering backends.
    ///</summary>
    public interface IRenderContext
    {
        ///<summary>
        ///Clears the render target with specified color.
        ///</summary>
        ///<param name="color">Clear color.</param>
        void Clear(Color color);

        ///<summary>
        ///Clears the render target with specified RGBA components.
        ///Adapts component-based clear calls to the canonical Clear implementation.
        ///</summary>
        ///<param name="r">Red component (0.0-1.0).</param>
        ///<param name="g">Green component (0.0-1.0).</param>
        ///<param name="b">Blue component (0.0-1.0).</param>
        ///<param name="a">Alpha component (0.0-1.0).</param>
        void Clear(float r, float g, float b, float a);

        ///<summary>
        ///Draws a line between two points.
        ///</summary>
        ///<param name="start">Start point.</param>
        ///<param name="end">End point.</param>
        ///<param name="color">Line color.</param>
        ///<param name="thickness">Line thickness.</param>
        void DrawLine(int x, Vector3 start, Vector3 end, Color color, float thickness = 1.0f);

        ///<summary>
        ///Draws a line between two points using coordinate components.
        ///Adapts component-based line drawing calls to the canonical DrawLine implementation.
        ///</summary>
        ///<param name="x1">Start X coordinate.</param>
        ///<param name="y1">Start Y coordinate.</param>
        ///<param name="x2">End X coordinate.</param>
        ///<param name="y2">End Y coordinate.</param>
        ///<param name="color">Line color.</param>
        void DrawLine(float x1, float y1, float x2, float y2, Color color);

        ///<summary>
        ///Draws a rectangle outline.
        ///</summary>
        ///<param name="rect">Rectangle bounds.</param>
        ///<param name="color">Rectangle color.</param>
        ///<param name="thickness">Line thickness.</param>
        void DrawRectangle(Rectangle rect, Color color, float thickness = 1.0f);

        ///<summary>
        ///Draws a rectangle using coordinate components.
        ///Adapts component-based rectangle drawing calls to the canonical DrawRectangle implementation.
        ///</summary>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="width">Rectangle width.</param>
        ///<param name="height">Rectangle height.</param>
        ///<param name="color">Rectangle color.</param>
        void DrawRectangle(float x, float y, float width, float height, Color color);

        ///<summary>
        ///Draws a rectangle using integer coordinate components.
        ///Adapts integer component-based rectangle drawing calls to the canonical DrawRectangle implementation.
        ///</summary>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="width">Rectangle width.</param>
        ///<param name="height">Rectangle height.</param>
        ///<param name="color">Rectangle color.</param>
        void DrawRectangle(int x, int y, int width, int height, Color color);

        ///<summary>
        ///Draws a filled rectangle.
        ///</summary>
        ///<param name="rect">Rectangle bounds.</param>
        ///<param name="color">Fill color.</param>
        void FillRectangle(Rectangle rect, Color color);

        ///<summary>
        ///Draws a circle outline.
        ///</summary>
        ///<param name="center">Center position.</param>
        ///<param name="radius">Circle radius.</param>
        ///<param name="color">Circle color.</param>
        ///<param name="thickness">Line thickness.</param>
        void DrawCircle(Vector3 center, float radius, Color color, float thickness = 1.0f);

        ///<summary>
        ///Draws a circle using coordinate components.
        ///Adapts component-based circle drawing calls to the canonical DrawCircle implementation.
        ///</summary>
        ///<param name="x">Center X coordinate.</param>
        ///<param name="y">Center Y coordinate.</param>
        ///<param name="radius">Circle radius.</param>
        ///<param name="color">Circle color.</param>
        void DrawCircle(float x, float y, float radius, Color color);

        ///<summary>
        ///Draws a filled circle.
        ///</summary>
        ///<param name="center">Center position.</param>
        ///<param name="radius">Circle radius.</param>
        ///<param name="color">Fill color.</param>
        void FillCircle(Vector3 center, float radius, Color color);

        ///<summary>
        ///Draws text at specified position.
        ///</summary>
        ///<param name="text">Text to draw.</param>
        ///<param name="position">Text position.</param>
        ///<param name="color">Text color.</param>
        ///<param name="size">Font size.</param>
        void DrawText(string text, Vector3 position, Color color, float size = 12.0f);

        ///<summary>
        ///Draws text using coordinate components.
        ///Adapts component-based text drawing calls to the canonical DrawText implementation.
        ///</summary>
        ///<param name="text">Text to draw.</param>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="size">Font size.</param>
        ///<param name="color">Text color.</param>
        void DrawText(string text, float x, float y, float size, Color color);

        ///<summary>
        ///Draws text using integer coordinate components.
        ///Adapts integer component-based text drawing calls to the canonical DrawText implementation.
        ///</summary>
        ///<param name="text">Text to draw.</param>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="size">Font size.</param>
        ///<param name="color">Text color.</param>
        void DrawText(string text, int x, int y, int size, Color color);

        ///<summary>
        ///Measures text dimensions.
        ///</summary>
        ///<param name="text">Text to measure.</param>
        ///<param name="size">Font size.</param>
        ///<returns>Text dimensions as Vector3.</returns>
        Vector3 MeasureText(string text, float size = 12.0f);

        ///<summary>
        ///Clears the screen.
        ///</summary>
        void ClearScreen();

        ///<summary>
        ///Presents the current frame to the display.
        ///</summary>
        void Present();

        ///<summary>
        ///Gets or sets the viewport size.
        ///</summary>
        Vector3 ViewportSize { get; set; }

        ///<summary>
        ///Gets the screen width.
        ///</summary>
        float ScreenWidth => ViewportSize.X;

        ///<summary>
        ///Gets the screen height.
        ///</summary>
        float ScreenHeight => ViewportSize.Y;

        ///<summary>
        ///Begins a new render batch.
        ///</summary>
        void BeginBatch();

        ///<summary>
        ///Ends the current render batch and submits for drawing.
        ///</summary>
        void EndBatch();

        ///<summary>
        ///Initializes the render context.
        ///</summary>
        void Initialize();

        ///<summary>
        ///Shuts down the render context.
        ///</summary>
        void Shutdown();

        ///<summary>
        ///Draws a texture at specified position.
        ///</summary>
        ///<param name="texture">Texture to draw.</param>
        ///<param name="position">Position to draw at.</param>
        ///<param name="color">Color tint.</param>
        void DrawTexture(object texture, Vector3 position, Color color);

        ///<summary>
        ///Draws a sprite using coordinate components.
        ///Adapts component-based sprite drawing calls to the canonical DrawTexture implementation.
        ///</summary>
        ///<param name="texture">Texture/sprite to draw.</param>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="width">Draw width.</param>
        ///<param name="height">Draw height.</param>
        ///<param name="color">Color tint.</param>
        void DrawSprite(object texture, float x, float y, float width, float height, Color color);

        ///<summary>
        ///Draws a sprite using coordinate components with separate color parameter.
        ///Adapts component-based sprite drawing calls to the canonical DrawTexture implementation.
        ///</summary>
        ///<param name="texture">Texture/sprite to draw.</param>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="color">Color tint.</param>
        void DrawSprite(object texture, float x, float y, Color color);
        void DrawRectangle(Func<float> x, Func<float> y, int width, int height, Color white);
        void DrawLine(int x1, int y1, int x2, int y2, uint pathColor);
        void DrawCircle(int x, int y, int v, uint pathColor);
        void DrawText(string stateText, int v1, int v2);
        void DrawText(string displayText, int v1, int v2, System.Drawing.Color sysText);
        void DrawFilledRectangle(int x, int y, int width, int height, System.Drawing.Color sysFill);

        ///<summary>
        ///Gets the render context width.
        ///</summary>
        float Width => ViewportSize.X;

        ///<summary>
        ///Gets the render context height.
        ///</summary>
        float Height => ViewportSize.Y;
    }
}

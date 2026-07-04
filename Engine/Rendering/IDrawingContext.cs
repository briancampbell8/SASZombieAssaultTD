using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Interface for drawing context operations.
    ///</summary>
    public interface IDrawingContext
    {
        void DrawCircle(float x, float y, float radius, Color color);
        void DrawCircle(Vector3 center, float radius, Color color, float thickness);
        void DrawLine(float x1, float y1, float x2, float y2, Color color);
        void DrawLine(int layer, Vector3 start, Vector3 end, Color color, float thickness);
        void DrawRectangle(float x, float y, float width, float height, Color color);
        void DrawRectangle(Func<float> x, Func<float> y, int width, int height, Color color);
        void DrawRectangle(int x, int y, int width, int height, Color color);
        void DrawRectangle(Rectangle rect, Color color, float thickness);
        void DrawTexture(object texture, Vector3 position, Color color);
        void FillCircle(Vector3 center, float radius, Color color);
        void FillRectangle(Rectangle rect, Color color);
        void Clear(Color color);
        void Clear(float r, float g, float b, float a);
        void DrawText(string text, Vector3 position, Color color, float size);
        void DrawText(string text, float x, float y, float size, Color color);
        void DrawText(string text, int x, int y, int size, Color color);
        Vector3 MeasureText(string text, float size);
        void ClearScreen();
        void Present();
        void BeginBatch();
        void EndBatch();
        void Initialize();
        void Shutdown();
        void DrawSprite(object texture, float x, float y, float width, float height, Color color);
        void DrawSprite(object texture, float x, float y, Color color);
        void DrawSprite(object value, object x, object y, object width, object height, Color color);
        void DrawSprite(object value, object height, Color color);
        void DrawLine(int x1, int y1, int x2, int y2, uint pathColor);
        void DrawLine(int x, int y, int v1, int v2, object value);
        void DrawCircle(int x, int y, int v, uint pathColor);
        void DrawText(string stateText, int v1, int v2, Color textColor);
        void DrawText(string iconText, float x, float y, int fontSize, Core.Color iconColor);
        void DrawRectangle(int x, int y, int cellSize1, int cellSize2, object color);
        void DrawTexture(Texture2D pixelRed, Rectangle dest, System.Drawing.Color red);

        Vector3 ViewportSize { get; set; }
    }
}

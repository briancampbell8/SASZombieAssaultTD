// ====================================================================================================
//  FILE: IDrawingContext.cs
//  PATH: Engine/UI/Rendering/IDrawingContext.cs
//  SUBSYSTEM: UI Rendering Abstraction
//
//  ROLE:
//      Unified, deterministic drawing context abstraction for UI and HUD systems.
//      Provides a stable, strongly‑typed API that routes to the engine’s rendering adapters.
//
//  RESPONSIBILITIES:
//      - Expose high‑level draw operations for UI/HUD layers.
//      - Remain backend‑agnostic (no GPU calls, no device ownership).
//      - Forward all rendering work to the engine’s rendering pipeline.
//
//  NON‑RESPONSIBILITIES:
//      - GPU resource creation.
//      - Swap‑chain or device management.
//      - Primitive rasterization logic.
//      - Software rendering fallbacks.
// ====================================================================================================

using System.Numerics;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Clean, deterministic drawing context abstraction for UI/HUD rendering.
    /// </summary>
    public interface IDrawingContext
    {
        // -------------------------------------------------------------------------------------------------
        // CLEAR OPERATIONS
        // -------------------------------------------------------------------------------------------------
        void Clear(Color color);
        void Clear(float r, float g, float b, float a);
        void ClearScreen();

        // -------------------------------------------------------------------------------------------------
        // TEXT OPERATIONS
        // -------------------------------------------------------------------------------------------------
        void DrawText(string text, Vector2 position, float size, Color color);
        Vector2 MeasureText(string text, float size);

        // -------------------------------------------------------------------------------------------------
        // SPRITE / TEXTURE OPERATIONS
        // -------------------------------------------------------------------------------------------------
        void DrawSprite(Texture2D texture, Vector2 position, Vector2 size, Color color);
        void DrawTexture(Texture2D texture, Core.Rectangle rect, Color color);

        // -------------------------------------------------------------------------------------------------
        // PRIMITIVE OPERATIONS
        // -------------------------------------------------------------------------------------------------
        void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1);
        void DrawRectangle(Core.Rectangle rect, Color color, float thickness = 1);
        void FillRectangle(Core.Rectangle rect, Color color);
        void DrawCircle(Vector2 center, float radius, Color color, float thickness = 1);
        void FillCircle(Vector2 center, float radius, Color color);

        // -------------------------------------------------------------------------------------------------
        // FRAME CONTROL (NO GPU OWNERSHIP)
        // -------------------------------------------------------------------------------------------------
        void BeginBatch();
        void EndBatch();
        void Initialize();
        void Shutdown();

        // -------------------------------------------------------------------------------------------------
        // RENDER COMMAND SUBMISSION
        // -------------------------------------------------------------------------------------------------
        void Submit(in RenderCommand cmd);
        void FillRectangle(float x0, float y0, float v1, float v2, Color color);
        void DrawLine(int v1, VectorMath.Vector3 left, VectorMath.Vector3 mid, Color color, float v2);

        // -------------------------------------------------------------------------------------------------
        // VIEWPORT METRICS
        // -------------------------------------------------------------------------------------------------
        Vector2 ViewportSize { get; set; }
        float Width { get; set; }
        float Height { get; set; }
    }
}

// ====================================================================================================
//  FILE: DebugHUDRenderer.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: DebugHUDRenderer.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

//Integration: Works with D3D11Adapter_Core and Texture2D for hardware-accelerated rendering.

//Performance: Uses single 1x1 texture stretched for all outlines, minimal draw calls.

//Standards: Full XML documentation with parameter descriptions and usage examples.
//

//

using System;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Render;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.UI.Rendering

{
    ///<summary>

    ///Debug HUD helper that draws outlines for UI areas using the engine rendering abstractions.

    ///Uses D3D11Adapter_Core.DrawTexture with a 1x1 BGRA pixel texture to build rectangle borders.

    ///</summary>

    ///<remarks>

    ///The DebugHUDRenderer provides real-time visualization of HUD element boundaries

    ///during development and debugging. It filters elements by ID suffix and renders

    ///red outlines to verify positioning, scaling, and alignment.

    ///

    ///Performance Characteristics:

    ///- Single 1x1 texture reused for all outlines

    ///- Four draw calls per rectangle (top, bottom, left, right edges)

    ///- Toggleable via Enabled property for release builds

    ///- Minimal memory footprint (4 bytes for pixel texture)

    ///

    ///Usage Pattern:

    ///Create once per render context, call DrawAreaOutline for each HUD element

    ///during the debug rendering pass.

    ///</remarks>

    ///<example>

    ///<code>

    /////Initialize debug renderer

    ///var debugHud = new DebugHUDRenderer(renderContext);

    ///debugHud.GlobalOffsetX = 2f;  //Fine-tune horizontal alignment

    ///debugHud.GlobalOffsetY = -1f; //Fine-tune vertical alignment

    ///

    /////In render loop, draw outlines for area elements

    ///foreach (var element in hudElements)

    ///{
    ///    debugHud.DrawAreaOutline(element.Id, element.Bounds, thickness: 2f);

    ///}

    ///</code>

    ///</example>

    public class DebugHUDRenderer

    {
        ///<summary>

        ///Enables or disables debug outline rendering.

        ///When disabled, all draw calls return immediately without rendering.

        ///</summary>

        public bool Enabled = true;

        ///<summary>

        ///Global horizontal offset applied to all debug outlines.

        ///Used for fine-tuning HUD alignment during development.

        ///</summary>

        public float GlobalOffsetX = 0f;

        ///<summary>

        ///Global vertical offset applied to all debug outlines.

        ///Used for fine-tuning HUD alignment during development.

        ///</summary>

        public float GlobalOffsetY = 0f;

        ///<summary>

        ///Toggles pixel-perfect circle diagnostic rendering.

        ///When true, DrawCircleDiagnostic will render circles.

        ///</summary>

        public bool DrawCircleEnabled = false;

        ///<summary>

        ///1x1 red pixel texture used for drawing red outlines.

        ///BGRA32 format (B=0, G=0, R=255, A=255).

        ///Stretched and tinted to draw colored outline edges.

        ///</summary>

        private readonly Texture2D _pixelRed;

        ///<summary>

        ///1x1 white pixel texture used for drawing white outlines.

        ///BGRA32 format (B=255, G=255, R=255, A=255).

        ///Stretched and tinted to draw colored outline edges.

        ///</summary>

        private readonly Texture2D _pixelWhite;

        /// <summary>

        ///Render context for drawing operations.

        ///Must not be null; validated in constructor.

        ///</summary>

        private readonly D3D11Adapter_Core _context;

        ///<summary>

        ///Initializes a new instance of the DebugHUDRenderer class.

        ///Creates the 1x1 white pixel texture used for all outline rendering.

        ///</summary>

        ///<param name="context">Render context for drawing operations. Must not be null.</param>

        ///<exception cref="ArgumentNullException">Thrown when context is null.</exception>

        ///<remarks>




        ///</remarks>

        public DebugHUDRenderer(D3D11Adapter_Core context)

        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            //1x1 RED pixel in BGRA32 (B=0, G=0, R=255, A=255)

            var redPixels = new byte[] { 0, 0, 255, 255 };

            _pixelRed = new Texture2D("debug_pixel_red", 1, 1, redPixels);

            //1x1 WHITE pixel in BGRA32 (B=255, G=255, R=255, A=255)

            var whitePixels = new byte[] { 255, 255, 255, 255 };

            _pixelWhite = new Texture2D("debug_pixel_white", 1, 1, whitePixels);
        }

        ///<summary>

        ///Draws a red outline rectangle for any HUD element whose ID ends with "_area".

        ///Accepts the engine's Rect type (float-based) and applies global offsets.

        ///</summary>

        ///<param name="elementId">Element identifier. Only IDs ending with "_area" are rendered.</param>

        ///<param name="destRect">Destination rectangle in screen coordinates (float-based).</param>

        ///<param name="thickness">Outline thickness in pixels. Default is 2f.</param>

        ///<remarks>

        ///This method filters elements by ID suffix to avoid cluttering the debug view.

        ///Only elements with IDs ending in "_area" (case-insensitive) will render outlines.

        ///GlobalOffsetX and GlobalOffsetY are applied to the destination rectangle for

        ///alignment tuning during development.

        ///

        ///Rendering Details:

        ///- Draws four edges separately (top, bottom, left, right)

        ///- Color is always Color.Red for visibility

        ///- Respects the Enabled property (returns immediately if false)

        ///</remarks>

        ///<example>

        ///<code>

        /////Draw outline for a HUD area element

        ///debugHud.DrawAreaOutline("health_bar_area", element.Bounds, thickness: 3f);

        ///

        /////This will NOT draw (no "_area" suffix)

        ///debugHud.DrawAreaOutline("health_bar", element.Bounds);

        ///</code>

        ///</example>
        public void DrawRectangleOutline(System.Drawing.RectangleF rect, System.Drawing.Color color, float thickness)

        {
            //Top
            DrawLine(rect.Left, rect.Top, rect.Right, rect.Top, color, thickness);

            //Bottom
            DrawLine(rect.Left, rect.Bottom, rect.Right, rect.Bottom, color, thickness);

            //Left
            DrawLine(rect.Left, rect.Top, rect.Left, rect.Bottom, color, thickness);

            //Right
            DrawLine(rect.Right, rect.Top, rect.Right, rect.Bottom, color, thickness);

        }



        DrawRectangleCommand DrawLineCommand { get; }


        private void DrawLine(float left, float top1, float right, float top2, System.Drawing.Color color, float thickness)
        {
            NI.Hit();
        }

        public void DrawAreaOutline(string elementId, System.Drawing.Rectangle destRect, float thickness = 2f)

        {
            DLogger.Log($"DebugHUDRenderer: {elementId} " +
                $"System.Drawing.Rectangle = {destRect.X},{destRect.Y},{destRect.Width},{destRect.Height}");

            if (!Enabled)

                return;

            if (string.IsNullOrEmpty(elementId) ||

                !elementId.EndsWith("_area", StringComparison.OrdinalIgnoreCase))

                return;

            //Apply global offsets for tuning

            var r = new System.Drawing.RectangleF(

                destRect.X + GlobalOffsetX,

                destRect.Y + GlobalOffsetY,

                destRect.Width,

                destRect.Height

            );

            //DrawRectangleOutline(r, Color.Red, thickness);
        }



        ///<summary>

        ///Draws a pixel-perfect, 1-pixel-thick circle diagnostic in HUD-space.

        ///Uses Bresenham's circle algorithm for discrete pixel-aligned rendering.

        ///</summary>

        ///<param name="centerX">Circle center X in HUD-space (float, snapped to integer pixel).</param>

        ///<param name="centerY">Circle center Y in HUD-space (float, snapped to integer pixel).</param>

        ///<param name="radius">Circle radius in pixels.</param>

        ///<remarks>

        ///Renders a 1-pixel-thick stroke that remains exactly 1 pixel under all

        ///scaling conditions. Center coordinates are snapped to integer boundaries

        ///for pixel-perfect alignment. Uses 8-way symmetry for efficient rendering.

        ///</remarks>

        public void DrawCircleDiagnostic(float centerX, float centerY, float radius)

        {
            if (!Enabled || !DrawCircleEnabled || radius <= 0)

                return;

            //Snap center to integer pixel boundaries

            int cx = (int)MathF.Round(centerX + GlobalOffsetX);

            int cy = (int)MathF.Round(centerY + GlobalOffsetY);

            int r = (int)MathF.Round(radius);

            //Bresenham's circle algorithm for pixel-perfect rendering

            int x = 0;

            int y = r;

            int d = 3 - 2 * r;

            while (x <= y)

            {
                //Draw 8 points using symmetry (8-way symmetry)

                DrawPixel(cx + x, cy + y);

                DrawPixel(cx - x, cy + y);

                DrawPixel(cx + x, cy - y);

                DrawPixel(cx - x, cy - y);

                DrawPixel(cx + y, cy + x);

                DrawPixel(cx - y, cy + x);

                DrawPixel(cx + y, cy - x);

                DrawPixel(cx - y, cy - x);

                if (d < 0)

                {
                    d = d + 4 * x + 6;
                }
                else

                {
                    d = d + 4 * (x - y) + 10;

                    y--;
                }

                x++;
            }
        }

        ///<summary>

        ///Draws a white rectangle outline for Lives panel positioning.

        ///Used for diagnostic alignment of the Lives "+" button hitbox.

        ///</summary>

        ///<param name="x">Rectangle X coordinate.</param>

        ///<param name="y">Rectangle Y coordinate.</param>

        ///<param name="width">Rectangle width.</param>

        ///<param name="height">Rectangle height.</param>

        ///<param name="thickness">Outline thickness in pixels. Default is 3.</param>


        ///<summary>

        ///Draws a single pixel using the 1x1 texture at specified integer coordinates.

        ///Ensures pixel-perfect rendering with no scaling or interpolation.

        ///</summary>

        ///<param name="x">Integer X coordinate.</param>

        ///<param name="y">Integer Y coordinate.</param>

        private void DrawPixel(int x, int y)

        {
            var dest = new System.Drawing.Rectangle(
                (int)(float)x,
                (int)(float)y,
                (int)(float)1,
                (int)(float)1);

            _context.DrawTexture(_pixelRed, dest, Color.Red);
        }

        ///<summary>

        ///Draws a rectangle outline by rendering four separate edges.

        ///</summary>

        ///<param name="rect">Rectangle bounds to outline.</param>

        ///<param name="color">Outline color.</param>

        ///<param name="thickness">Edge thickness in pixels.</param>

        ///<remarks>

        ///Decomposes the outline into four solid rectangles:

        ///<param name="x">X coordinate (float, rounded to integer).</param>

        ///<param name="y">Y coordinate (float, rounded to integer).</param>

        ///<param name="width">Rectangle width (float, rounded to integer).</param>

        ///<param name="height">Rectangle height (float, rounded to integer).</param>

        ///<param name="color">Fill color for the rectangle.</param>

        ///<remarks>

        ///Converts float coordinates to integers using MathF.Round for pixel-perfect

        ///positioning. The 1x1 pixel texture is stretched to the destination

        ///rectangle and color-tinted to produce the desired color.

        ///

        ///Coordinate Conversion:

        ///- Float coordinates are rounded to nearest integer

        ///- This ensures crisp 1-pixel lines even with sub-pixel positioning

        ///</remarks>

        public void DrawSolidRect(float x, float y, float width, float height, System.Drawing.Color color)
        {
            // Fixes both the syntax error (var Rectangle) and the ambiguous constructor call
            var dest = new Rectangle(
                (float)x,
                (float)y,
                (float)width,
                (float)height
            );

            // Use white texture tinted with color
            var texture = _pixelWhite;

            _context.DrawTexture(texture, dest, color);
        }

    }
}


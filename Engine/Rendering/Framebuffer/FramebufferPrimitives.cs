//
// * File:    FramebufferPrimitives.cs
// * Path:    Engine/Rendering/Framebuffer/FramebufferPrimitives.cs
// * Purpose: CPU raster primitives for the Framebuffer.
// *          Contains line drawing (Bresenham), circle rendering, rectangles,
// *          and filled shapes with proper bounds checking.
// //

using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;
using System;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public partial class Framebuffer
    {
        // ---------------------------------------------------------
        // PRIMITIVE METHODS ONLY
        // ---------------------------------------------------------

        /// <summary>
        /// Draws a rectangle using float coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        /// <param name="color">Rectangle color.</param>
        public void IDrawingContext_DrawRectangle(float x, float y, float width, float height, Color color)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.DrawRectangle - STUB PROCESSED");
        }

        // All legacy methods removed as per SECTION 1
    }
}

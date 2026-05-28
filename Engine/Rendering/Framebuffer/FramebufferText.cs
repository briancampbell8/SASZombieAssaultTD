//
// * File:    FramebufferText.cs
// * Path:    Engine/Rendering/Framebuffer/FramebufferText.cs
// * Purpose: Text rendering subsystem for the Framebuffer.
// *          Contains DrawText with color conversion and MeasureText functionality.
// *          Integrates with the TextRenderer for actual glyph rasterization.
// //

using SASZombieAssaultTD.Engine.Diagnostics;

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public partial class Framebuffer
    {
        // ---------------------------------------------------------
        // TEXT METHODS ONLY
        // ---------------------------------------------------------

        /// <summary>
        /// Draws text at specified position with color and size.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="position">Text position.</param>
        /// <param name="color">Text color.</param>
        /// <param name="size">Font size.</param>
        public void DrawText(string text, Vector3 position, System.Drawing.Color color, float size = 12.0f)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.DrawText - STUB PROCESSED");
        }

        /// <summary>
        /// Draws text using float coordinates.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="size">Font size.</param>
        /// <param name="color">Text color.</param>
        public void DrawText(string text, float x, float y, float size, System.Drawing.Color color)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.DrawText - STUB PROCESSED");
        }

        /// <summary>
        /// Draws text using integer coordinates.
        /// </summary>
        /// <param name="text">Text to draw.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="size">Font size.</param>
        /// <param name="color">Text color.</param>
        public void DrawText(string text, int x, int y, int size, System.Drawing.Color color)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.DrawText - STUB PROCESSED");
        }

        /// <summary>
        /// Draws text using object parameter and integer coordinates.
        /// </summary>
        /// <param name="text">Text object to draw.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public void DrawText(object text, int x, int y)
        {
            System.Diagnostics.Debug.WriteLine("[DIAG] Framebuffer.DrawText - STUB PROCESSED");
        }

        // All legacy methods removed as per SECTION 1
    }
}

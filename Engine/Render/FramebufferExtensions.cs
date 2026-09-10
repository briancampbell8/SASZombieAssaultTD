// ====================================================================================================
//  FILE: FramebufferExtensions.cs
//  PATH: Engine/Render/FramebufferExtensions.cs
//  MODULE: Render / Framebuffer Extensions
//
//  ROLE:
//      Provide deterministic, type‑safe extension methods for FramebufferDrawing to support software
//      rendering operations such as clearing, filling, and pixel manipulation.
//
//  RESPONSIBILITIES:
//      - Provide ClearScreen() and related helpers for the Render subsystem.
//      - Maintain deterministic behavior for software rendering.
//      - Remain side‑effect free outside framebuffer mutation.
//
//  NON‑RESPONSIBILITIES:
//      - Resource loading or asset management.
//      - GPU rendering or SpriteBatch submission.
//      - Diagnostics, logging, or performance metrics.
//
//  NOTES:
//      This module extends FramebufferDrawing only; it does not interact with RenderSurface or GPU
//      subsystems. Relocated from Engine/Extensions to Engine/Render.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Graphics.Software;

namespace SASZombieAssaultTD.Engine.Render
{
    public static class FramebufferExtensions
    {
        /// <summary>
        /// Clears the framebuffer using the specified color.
        /// </summary>
        public static void ClearScreen(this FramebufferDrawing framebuffer, Color color)
        {
            framebuffer.Clear(color);
        }

        /// <summary>
        /// Clears the framebuffer to fully transparent.
        /// </summary>
        public static void ClearTransparent(this FramebufferDrawing framebuffer)
        {
            framebuffer.Clear(Color.FromArgb(0, 0, 0, 0));
        }
    }
}

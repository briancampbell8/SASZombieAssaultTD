// =====================================================================================================
//  FILE: FramebufferContextBase.cs
//  PATH: Engine/Graphics/Software/FramebufferContextBase.cs
//  SUBSYSTEM: Graphics Software / Shared Framebuffer Utilities
//
//  ROLE:
//      Provides shared deterministic helpers used by software framebuffer contexts. This class
//      intentionally does NOT implement D3D11Adapter_Core and contains NO rendering methods. It exists
//      purely as a safe, stable base for FramebufferContext and any future software framebuffer
//      variants.
//
//  RESPONSIBILITIES:
//      - Provide safe pixel bounds checking.
//      - Provide clamped coordinate helpers.
//      - Provide optional debug instrumentation hooks.
//      - Provide shared utility methods used by software framebuffer contexts.
//
//  NON-RESPONSIBILITIES:
//      - Rendering (shapes, text, textures, sprites).
//      - Command submission.
//      - Backend routing.
//      - Frame lifecycle management.
//      - Any D3D11Adapter_Core method surface.
// =====================================================================================================

namespace SASZombieAssaultTD.Engine.Graphics.Software
{
    internal abstract class FramebufferContextBase
    {
        // ---------------------------------------------------------------------------------------------
        // SAFE COORDINATE HELPERS
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns true if (x,y) is inside the framebuffer bounds.
        /// </summary>
        protected static bool InBounds(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        /// <summary>
        /// Clamps X to the valid framebuffer range.
        /// </summary>
        protected static int ClampX(int x, int width)
        {
            if (x < 0) return 0;
            if (x >= width) return width - 1;
            return x;
        }

        /// <summary>
        /// Clamps Y to the valid framebuffer range.
        /// </summary>
        protected static int ClampY(int y, int height)
        {
            if (y < 0) return 0;
            if (y >= height) return height - 1;
            return y;
        }

        // ---------------------------------------------------------------------------------------------
        // OPTIONAL DEBUG HOOKS
        // ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Debug hook for pixel writes. Empty in release builds.
        /// </summary>
        protected void DebugPixelWrite(int x, int y)
        {
#if DEBUG
            // Add logging or instrumentation here if needed.
#endif
        }

        /// <summary>
        /// Debug hook for command routing. Empty in release builds.
        /// </summary>
        protected void DebugCommand(string message)
        {
#if DEBUG
            // Add logging or instrumentation here if needed.
#endif
        }

        // ---------------------------------------------------------------------------------------------
        // NO RENDERING METHODS HERE
        // ---------------------------------------------------------------------------------------------
        // This class MUST NOT contain:
        //   - Submit()
        //   - DrawLine()
        //   - DrawRectangle()
        //   - DrawText()
        //   - Clear()
        //   - Present()
        //   - BeginFrame()
        //   - EndFrame()
        //
        // Those belong exclusively in FramebufferContext, which implements D3D11Adapter_Core.
        // ---------------------------------------------------------------------------------------------
    }
}

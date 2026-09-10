// ====================================================================================================
//  FILE: CompositeMergeTool.cs
//  PATH: Engine/Render/Composite
//  MODULE: Rendering Subsystem – Composite Pipeline
//
//  ROLE:
//      Deterministic CPU-side pixel merge tool used by CompositePipeline.
//      Performs per‑pixel RGBA8 merges from EngineTexture into CompositeSurface.
//
//  RESPONSIBILITIES:
//      - Merge RGBA8 pixel buffers deterministically.
//      - Skip transparent pixels.
//      - Respect surface bounds.
//      - Provide stable, side‑effect‑free composition behavior.
//
//  NON-RESPONSIBILITIES:
//      - GPU upload or rendering (handled by Texture2D / IGraphicsDevice).
//      - Resource loading or caching (handled by Resource Management Framework).
//      - Diagnostics, logging, or performance metrics.
//      - Gameplay logic or UI layout.
//
//  ARCHITECTURAL NOTES:
//      - CompositeMergeTool is pure CPU logic.
//      - CompositeSurface is always RGBA8 (byte[]).
//      - EngineTexture provides CPU pixel data for composition.
//      - Deterministic Option‑B compliant: no branching beyond bounds/alpha checks.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Render.Composite
{
    public sealed class CompositeMergeTool : ICompositeMergeTool
    {
        public byte[] srcPixels { get; private set; }

        public byte[] dstPixels { get; private set; }

        public void Draw(CompositeSurface surface, IEngineTexture texture, int x, int y)
        {
            var surf = (CompositeSurface)surface;
            var tex = (EngineTexture)texture;

            int sw = surf.Width;
            int sh = surf.Height;

            int tw = tex.Width;
            int th = tex.Height;

            // EngineTexture.Pixels is byte[] RGBA8
            srcPixels = tex.Pixels;
            dstPixels = surf.Pixels;

            const int BPP = 4; // RGBA8

            for (int ty = 0; ty < th; ty++)
            {
                int sy = y + ty;
                if (sy < 0 || sy >= sh) continue;

                int srcRow = ty * tw * BPP;
                int dstRow = sy * sw * BPP;

                for (int tx = 0; tx < tw; tx++)
                {
                    int sx = x + tx;
                    if (sx < 0 || sx >= sw) continue;

                    int srcIndex = srcRow + (tx * BPP);
                    byte a = srcPixels[srcIndex + 3];

                    // Skip transparent pixels
                    if (a == 0) continue;

                    int dstIndex = dstRow + (sx * BPP);

                    dstPixels[dstIndex + 0] = srcPixels[srcIndex + 0]; // R
                    dstPixels[dstIndex + 1] = srcPixels[srcIndex + 1]; // G
                    dstPixels[dstIndex + 2] = srcPixels[srcIndex + 2]; // B
                    dstPixels[dstIndex + 3] = a;                       // A
                }
            }
        }

        public void Draw(ITextureSurface surface, IEngineTexture texture, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deterministic helper used by CompositePipeline.
        /// </summary>
        internal void DrawTexture(ITextureSurface surface, EngineTexture texture, int x, int y)
        {
            Draw(surface, texture, x, y);
        }

        void ICompositeMergeTool.DrawTexture(ITextureSurface surface, EngineTexture texture, int x, int y)
        {
            DrawTexture(surface, texture, x, y);
        }
    }
}

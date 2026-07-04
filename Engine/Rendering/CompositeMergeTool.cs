////Program Name: CompositeMergeTool.cs
////File Path: Engine/Rendering/CompositeMergeTool.cs
////Program Purpose: The program performs pixel-level merge operations to draw textures onto composite surfaces.
////Program Features:
////- Draws textures onto composite surfaces at specified coordinates
////- Skips transparent pixels during merge
////- Writes opaque pixels to surface pixel array
////- Handles boundary checking for surface dimensions

//

using Engine.Rendering.Interfaces;
using SASZombieAssaultTD.Engine.Diagnostics;

public sealed class CompositeMergeTool : ICompositeMergeTool
{
    public void Draw(ICompositeSurface surface, IEngineTexture texture, int x, int y)
    {
        CompositeSurface surf = (CompositeSurface)surface;
        EngineTexture tex = (EngineTexture)texture;

        int sw = surf.Width;
        int sh = surf.Height;

        int tw = tex.Width;
        int th = tex.Height;

        for (int ty = 0; ty < th; ty++)
        {
            int sy = y + ty;
            if (sy < 0 || sy >= sh) continue;

            for (int tx = 0; tx < tw; tx++)
            {
                int sx = x + tx;
                if (sx < 0 || sx >= sw) continue;

                int src = tex.Pixels[ty * tw + tx];

                //Skip transparent pixels
                if ((src >> 24) == 0) continue;

                surf.Pixels[sy * sw + sx] = src;
            }
        }
    }

    internal void DrawTexture(CompositeSurface surface, EngineTexture hudTexture, int hudX, int hudY)
    {
        NI.Hit();
    }

    internal void DrawTexture(ICompositeSurface surface, EngineTexture mapTexture, int v1, int v2)
    {
        NI.Hit();
    }
}

// ====================================================================================================
//  FILE: ITextureSurface.cs
//  PATH: Engine/Render/Textures/ITextureSurface.cs
//  SUBSYSTEM: Render Textures
//
//  ROLE:
//      GPU-side texture metadata container used for upload, sampling, and render operations.
//
//  RESPONSIBILITIES:
//      - Provide immutable metadata (width, height, format).
//      - Expose GPU texture handles (Texture2D).
//      - Provide deterministic mip-level layout information.
//      - Integrate cleanly with EngineTexture and the D3D11 backend.
//
//  NON-RESPONSIBILITIES:
//      - CPU-side pixel storage (handled by CompositeSurface).
//      - Compositing or pixel manipulation (handled by CompositeMergeTool).
//      - File encoding or serialization.
//
//  ARCHITECTURAL NOTES:
//      - Strict Option‑B architecture.
//      - TextureSurface is GPU-side only.
//      - CompositeSurface is CPU-side only.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Render.Composite;
using SASZombieAssaultTD.Engine.TextRendering;
using SASZombieAssaultTD.Engine.TextureRendering;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface ITextureSurface
    {
        int Width { get; }
        int Height { get; }

        GpuTextureFormat Format { get; }
        TextEnums.EngineTextureFlags Flags { get; }

        Texture2D Texture { get; }

        int MipLevelCount { get; }
        int[] MipLevelOffsets { get; }
        int[] MipLevelSizes { get; }
        int[] MipLevelStrides { get; }
        int[] MipLevelRowPitches { get; }
        int[] MipLevelSlicePitches { get; }
        byte[] Pixels { get; set; }

        int GetSlicePitch();

        public int SlicePitch { get; }
    }
}

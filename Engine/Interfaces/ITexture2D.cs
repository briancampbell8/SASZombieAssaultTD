// ====================================================================================================
//  FILE: ITexture2D.cs
//  PATH: Engine/Render/Textures
//  MODULE: Rendering Subsystem
//
//  ROLE:
//      Deterministic GPU texture descriptor used by the rendering backend.
//      Provides stable metadata and backend handles.
//
//  RESPONSIBILITIES:
//      - Expose immutable texture metadata (width, height, format, flags).
//      - Provide access to backend GPU handles (D3D11 SRV).
//      - Integrate cleanly with IGraphicsDevice and EngineTexture.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU upload operations.
//      - Performing rendering or sampling operations.
//      - Resource loading or caching.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Render.Composite;
using Vortice.Direct3D11;
using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface ITexture2D
    {
        int Width { get; }
        int Height { get; }

        GpuTextureFormat Format { get; }
        Texture2DFlags Flags { get; }

        /// <summary>
        /// Backend GPU shader resource view (D3D11).
        /// </summary>
        ID3D11ShaderResourceView ShaderResourceView { get; }
        ID3D11Texture2D NativeTexture { get; set; }

        object GetService<T>();
    }

}

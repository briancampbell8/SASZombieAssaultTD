//
// * File:    BGFXHandles.cs
// * Path:    Engine/Rendering/BGFX/BGFXHandles.cs
// * Purpose: Minimal BGFX handle placeholders for interface compliance.
// *          These structs match BGFX handle types without requiring the actual library.
// //
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering.BGFX
{
    /// <summary>
    /// BGFX vertex buffer handle placeholder.
    /// </summary>
    public struct VertexBufferHandle
    { }

    /// <summary>
    /// BGFX index buffer handle placeholder.
    /// </summary>
    public struct IndexBufferHandle
    { }

    /// <summary>
    /// BGFX texture handle placeholder.
    /// </summary>
    public struct TextureHandle
    { }

    /// <summary>
    /// BGFX shader program handle placeholder.
    /// </summary>
    public struct ProgramHandle
    { }

    /// <summary>
    /// BGFX reset flags placeholder.
    /// </summary>
    public struct BGFX_RESET_FLAGS
    { }
}

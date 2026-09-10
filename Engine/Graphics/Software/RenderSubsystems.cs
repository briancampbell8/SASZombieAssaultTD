// =====================================================================================================
//  FILE: RenderSubsystems.cs
//  PATH: Engine/Graphics/Software/RenderSubsystems.cs
//  SUBSYSTEM: Graphics Software / Deterministic Framebuffer Routing
//
//  ROLE:
//      Defines the subsystem capability flags used by FramebufferContext partials to conditionally
//      activate or bypass rendering operations. This enables partial classes to implement only the
//      operations they own while maintaining a unified D3D11Adapter_Core surface.
//
//  NOTES:
//      - Each partial sets the flags for the subsystem it implements.
//      - Conditionalized explicit interface routing checks these flags.
//      - Prevents pollution of Partial 1 with shape/text/texture/command stubs.
// =====================================================================================================

using System;

namespace SASZombieAssaultTD.Engine.Graphics.Software
{

    /// <summary>
    /// The render subsystems.
    /// </summary>
    [Flags]
    public enum RenderSubsystems
    {
        None = 0,
        Core = 1 << 0,
        Shapes = 1 << 1,
        Text = 1 << 2,
        Textures = 1 << 3,
        Commands = 1 << 4
    }
}

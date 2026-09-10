// =====================================================================================================
//  FILE: PixelFormat.cs
//  PATH: Engine/UI/Rendering/PixelFormat.cs
//  SUBSYSTEM: Platform Abstraction Layer / Texture & Surface Format Definitions
//
//  ROLE:
//      Defines the canonical pixel formats supported by the engine's rendering pipeline. This enum
//      provides a deterministic, platform‑agnostic representation of texture and framebuffer formats
//      used across UI rendering, texture loading, GPU upload, and surface composition.
//
//  RESPONSIBILITIES:
//      - Enumerate all pixel formats recognized by the engine.
//      - Provide stable identifiers for CPU‑side decoding (PNG, etc.).
//      - Provide stable identifiers for GPU‑side formats (BGRA, UNorm, ARGB).
//      - Support legacy formats for transitional compatibility.
//      - Serve as the authoritative format map for RenderSystem, TextureManager, and adapters.
//
//  NON-RESPONSIBILITIES:
//      - Performing pixel conversion or swizzling.
//      - Managing GPU resources or texture allocation.
//      - Handling compression formats or mipmap generation.
//      - Encoding or decoding image data.
//
//  ARCHITECTURAL NOTES:
//      - Minimal deterministic set required by the engine.
//      - Extended formats (UNorm, ARGB) included for adapter‑level compatibility.
//      - Enum values are stable and must not be reordered.
// =====================================================================================================


namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    public enum PixelFormat
    {
        Unknown = 0,

        //32-bit formats
        Rgba32 = 1,   //CPU PNG decode (RGBA)
        Bgra32 = 2,   //FramebufferDrawing + GPU texture format (BGRA)

        //24-bit formats
        Rgb24 = 3,    //Legacy PNGs with no alpha

        //8-bit formats
        Alpha8 = 4,   //Single-channel alpha mask

        //Error / invalid
        Invalid = 255,
        R8G8B8A8_UNorm = 256,
        Format32bppArgb = 257,
        R8G8B8A8 = 258
    }
}

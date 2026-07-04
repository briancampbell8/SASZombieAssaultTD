/*
//*File:    PixelFormat.cs
//*
//* Path:    Engine / Rendering / PixelFormat.cs
//*
//*Purpose: Refactored D3D11 graphics device using modular components.
//*
//*This is a facade that coordinates all D3D11 subsystems.
//*
//*
//* Role:    -Coordinates all D3D11 components
//*
//*-Provides public API for graphics device
//*
//*-Maintains backward compatibility with existing code
//*
//*-Routes calls to appropriate specialized components
//*
//*
//*
//*Notes:   This replaces the 959 - line monolithic D3D11GraphicsDevice.cs
//*
//*with a clean, modular architecture using composition.
//*
//

 */

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    public enum PixelFormat
    {
        Unknown = 0,

        //32-bit formats
        Rgba32 = 1,   //CPU PNG decode (RGBA)
        Bgra32 = 2,   //Framebuffer + GPU texture format (BGRA)

        //24-bit formats
        Rgb24 = 3,    //Legacy PNGs with no alpha

        //8-bit formats
        Alpha8 = 4,   //Single-channel alpha mask

        //Error / invalid
        Invalid = 255,
        R8G8B8A8_UNorm = 256
    }
}

//
//* File:    IRenderTarget.cs
//* Path:    Engine/Rendering/IRenderTarget.cs
//* Purpose: Minimal abstraction for GPU render targets.
//*
//* Role:    - Represents a GPU-backed renderable surface
//*          - Used by ModernUIRenderer and other GPU renderers
//*
//* Notes:   This interface intentionally exposes only logical properties.
//*          Backend-specific details (D3D11 textures, views, etc.) are kept in
//*          implementation types (e.g., D3D11RenderTarget).
//*

//

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Minimal abstraction for a GPU render target surface.
    ///</summary>
    ///<remarks>
    ///Render targets are created by <see cref="IGraphicsDevice"/> and used by
    ///renderers such as ModernUIRenderer to perform off-screen rendering,
    ///post-processing, and compositing.
    ///
    ///Implementations must:
    ///- Own any underlying GPU resources (textures, views, etc.)
    ///- Ensure proper disposal of GPU resources when disposed
    ///- Be immutable in size for their lifetime (unless explicitly recreated)
    ///</remarks>
    public interface IRenderTarget : IDisposable
    {
        ///<summary>
        ///Gets the logical name of the render target.
        ///</summary>
        string Name { get; }

        ///<summary>
        ///Gets the width of the render target in pixels.
        ///</summary>
        int Width { get; }

        ///<summary>
        ///Gets the height of the render target in pixels.
        ///</summary>
        int Height { get; }
    }
}

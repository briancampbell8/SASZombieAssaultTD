// =====================================================================================================
//  FILE: CompositeSurface.cs
//  PATH: Engine/Render/Composite/CompositeSurface.cs
//  SUBSYSTEM: Render / Composite
//
//  ROLE:
//      Deterministic CPU‑side RGBA8 surface for compositing operations.
//
//  RESPONSIBILITIES:
//      - Store pixel data in RGBA8 format.
//      - Expose width, height, and format metadata.
//      - Provide deterministic access to raw pixel buffer.
//
//  NON‑RESPONSIBILITIES:
//      - GPU upload (handled by D3D11Adapter_Manager).
//      - Rendering (handled by RenderManager).
//      - Memory pooling (handled by SurfacePool).
//
//  ARCHITECTURAL NOTES:
//      - Strict Option‑B architecture.
//      - Immutable dimensions and format.
// =====================================================================================================

using System;
using SASZombieAssaultTD.Engine.Interfaces;

namespace SASZombieAssaultTD.Engine.Render.Composite
{
    public sealed class CompositeSurface : ICompositeSurface
    {
        public int Width { get; }
        public int Height { get; }

        /// <summary>Bytes per pixel (RGBA8 = 4).</summary>
        public int BytesPerPixel { get; }

        /// <summary>Number of bytes per row.</summary>
        public int RowPitch { get; }

        /// <summary>Number of bytes in the entire surface.</summary>
        public int SlicePitch { get; }

        /// <summary>Raw RGBA8 pixel data.</summary>
        public byte[] Pixels { get; }

        /// <summary>Surface format (always RGBA8).</summary>
        public GpuTextureFormat Format => GpuTextureFormat.RGBA8;

        public CompositeSurface(int width, int height)
        {
            Width = width;
            Height = height;

            BytesPerPixel = 4; // RGBA8
            RowPitch = width * BytesPerPixel;
            SlicePitch = RowPitch * height;

            Pixels = new byte[SlicePitch];
        }

        // =====================================================================================================
        //  LEGAL REPLACEMENT FOR THE FORBIDDEN INTERFACE CAST OPERATOR
        // =====================================================================================================
        public static CompositeOperator ToOperator(ITextureSurface surface)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            return CompositeOperator.FromSurface(surface);
        }
    }

    // Stub TextureFormat to match your existing structure
    public class GpuTextureFormat
    {
        public static GpuTextureFormat RGBA8 { get; internal set; }
        public static GpuTextureFormat RGBA32 { get; internal set; }
        public static GpuTextureFormat BGRA32 { get; internal set; }

        public static explicit operator GpuTextureFormat(long v)
        {
            // Convert the provided integral value to the GpuTextureFormat enum/struct.
            // An explicit cast is appropriate here to allow callers to provide boxed integral types (e.g., long).
            return (GpuTextureFormat)v;
        }
    }
}

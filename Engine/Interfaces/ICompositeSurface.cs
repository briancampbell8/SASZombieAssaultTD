// ====================================================================================================
//  FILE: ICompositeSurface.cs
//  PATH: Engine/Interfaces/ICompositeSurface.cs
//  MODULE: Resource Management Framework
//  SUBSYSTEMS INTERFACES: Composite Surface Management
//  ROLE:
//      Deterministic CPU-side pixel buffer used for composite operations prior to GPU upload.
//
//  RESPONSIBILITIES:
//      - Provide immutable metadata (width, height, stride).
//      - Expose a deterministic pixel buffer for composition.
//      - Integrate cleanly with CompositeCreator and Texture2D.
//
//  NON-RESPONSIBILITIES:
//      - Performing GPU upload operations.
//      - Managing texture flags or formats.
//      - Logging, diagnostics, or performance metrics.
//      - Encoding or authoring texture files.
//
//  ARCHITECTURAL NOTES:
//      - Composite surfaces are pure CPU-side buffers.
//      - GPU upload is delegated to Texture2D.
//      - Resource identity must remain stable and deterministic.
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.Interfaces
{
    public interface ICompositeSurface
    {
        int Width { get; }
        int Height { get; }

        /// <summary>
        /// Bytes per pixel (always 4 for RGBA8).
        /// </summary>
        int BytesPerPixel { get; }

        /// <summary>
        /// Number of bytes per row.
        /// </summary>
        int RowPitch { get; }

        /// <summary>
        /// Number of bytes in the entire surface.
        /// </summary>
        int SlicePitch { get; }

        /// <summary>
        /// Raw pixel buffer in RGBA8 format.
        /// </summary>
        byte[] Pixels { get; }
    }
}
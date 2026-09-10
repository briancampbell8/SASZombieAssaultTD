// ====================================================================================================
//  FILE: ICompositeImageLoader.cs
//  PATH: Engine/Interfaces/ICompositeImageLoader.cs
//  MODULE: Rendering Subsystem – Composite Pipeline
//
//  ROLE:
//      Defines the interface for deterministic CPU-side image loading.
//      Provides RGBA8 decode operations for PNG/JPG/TGA/BMP/GIF used by CompositePipeline.
//
//  RESPONSIBILITIES:
//      - Load image files from disk deterministically.
//      - Decode into RGBA8 pixel buffers (byte[]).
//      - Provide stable metadata (width, height, format, name, path).
//      - Serve as the CPU entry point for EngineTexture creation.
//
//  NON-RESPONSIBILITIES:
//      - GPU upload or rendering (handled by Texture2D / IGraphicsDevice).
//      - Resource caching or lookup (handled by Resource Management Framework).
//      - Diagnostics, logging, or fallback behavior.
//      - Gameplay logic or UI layout.
//
//  ARCHITECTURAL NOTES:
//      - Implementations must be deterministic and side‑effect‑free.
//      - All decoded images must be RGBA8 (4 bytes per pixel).
//      - No dynamic dictionaries or magic strings.
// ====================================================================================================

using static SASZombieAssaultTD.Engine.Render.RenderEnums;

namespace SASZombieAssaultTD.Engine.Interfaces
{


    /// <summary>
    /// Deterministic metadata for decoded images.
    /// </summary>
    public readonly struct ImageInfo
    {
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }
        public string Name { get; }
        public string Path { get; }
        public ImageFormat Format { get; }

        public ImageInfo(int width, int height, int depth, string name, string path, ImageFormat format)
        {
            Width = width;
            Height = height;
            Depth = depth;
            Name = name;
            Path = path;
            Format = format;
        }
    }

    /// <summary>
    /// Interface for deterministic CPU-side image loading.
    /// </summary>
    public interface ICompositeImageLoader
    {
        /// <summary>
        /// Returns deterministic metadata for an image file.
        /// </summary>
        ImageInfo GetImageInfo(string path);

        /// <summary>
        /// Decodes an image file into RGBA8 pixel data.
        /// </summary>
        byte[] LoadPixels(string path);
    }
}

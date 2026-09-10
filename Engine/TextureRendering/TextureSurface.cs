// ====================================================================================================
//  FILE: TextureSurface.cs
//  PATH: /Engine/Render/Texture
//  MODULE: Render Texture
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.Composite;
using SASZombieAssaultTD.Engine.TextRendering;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    /// <summary>
    /// Concrete texture surface implementation backing ITextureSurface.
    /// </summary>
    public sealed class TextureSurface : ITextureSurface
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _bytesPerPixel;
        private readonly byte[] _pixels;

        private readonly int[] _mipOffsets;
        private readonly int[] _mipSizes;
        private readonly int[] _mipStrides;
        private readonly int[] _mipRowPitches;
        private readonly int[] _mipSlicePitches;

        private readonly int _rowPitch;
        private readonly int _slicePitch;

        public int Width => _width;
        public int Height => _height;

        /// <summary>
        /// Convenience integer pixel buffer (ARGB/RGBA packed).
        /// </summary>
        public int[] Pixels { get; }

        public int BytesPerPixel => _bytesPerPixel;
        public int RowPitch => _rowPitch;

        public int GetSlicePitch()
        {
            return _slicePitch;
        }

        int ITextureSurface.SlicePitch => _slicePitch;

        public TextureSurface(int slicePitch)
        {
            _slicePitch = slicePitch;
        }

        public EngineTextureFlags Flags { get; }
        public Texture2D Texture { get; }
        public GpuTextureFormat Format { get; }

        public int MipLevelCount { get; }
        public int[] MipLevelOffsets => _mipOffsets;
        int[] ITextureSurface.MipLevelOffsets => _mipOffsets;

        public int[] MipLevelSizes => _mipSizes;
        int[] ITextureSurface.MipLevelSizes => _mipSizes;

        public int[] MipLevelStrides => _mipStrides;
        int[] ITextureSurface.MipLevelStrides => _mipStrides;

        public int[] MipLevelRowPitches => _mipRowPitches;
        int[] ITextureSurface.MipLevelRowPitches => _mipRowPitches;

        public int[] MipLevelSlicePitches => _mipSlicePitches;
        int[] ITextureSurface.MipLevelSlicePitches => _mipSlicePitches;

        /// <summary>
        /// Raw byte pixel buffer required by ITextureSurface.
        /// </summary>
        byte[] ITextureSurface.Pixels
        {
            get => _pixels;
            set
            {
                if (value == null || value.Length != _pixels.Length)
                    return;
                System.Buffer.BlockCopy(value, 0, _pixels, 0, _pixels.Length);
            }
        }

        TextEnums.EngineTextureFlags ITextureSurface.Flags => throw new System.NotImplementedException();

        public TextureSurface(int width, int height)
        {
            _width = width;
            _height = height;

            _bytesPerPixel = 4; // RGBA32
            _rowPitch = _width * _bytesPerPixel;
            _slicePitch = _rowPitch * _height;

            _pixels = new byte[_slicePitch];
            Pixels = new int[_width * _height];

            MipLevelCount = 1;

            _mipOffsets = new[] { 0 };
            _mipSizes = new[] { _width * _height };
            _mipStrides = new[] { _rowPitch };
            _mipRowPitches = new[] { _rowPitch };
            _mipSlicePitches = new[] { _slicePitch };

            Flags = EngineTextureFlags.None;
            Texture = null;
            Format = GpuTextureFormat.RGBA32;
        }

        /// <summary>
        /// Parameterless constructor retained for tooling/serialization.
        /// </summary>
        public TextureSurface()
        {
            _width = 0;
            _height = 0;
            _bytesPerPixel = 4;

            _rowPitch = 0;
            _slicePitch = 0;

            _pixels = System.Array.Empty<byte>();
            Pixels = System.Array.Empty<int>();

            MipLevelCount = 0;

            _mipOffsets = System.Array.Empty<int>();
            _mipSizes = System.Array.Empty<int>();
            _mipStrides = System.Array.Empty<int>();
            _mipRowPitches = System.Array.Empty<int>();
            _mipSlicePitches = System.Array.Empty<int>();

            Flags = EngineTextureFlags.None;
            Texture = null;
            Format = GpuTextureFormat.RGBA32;
        }
    }
}

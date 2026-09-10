// ====================================================================================================
//  FILE: EngineTexture.cs
//  PATH: Engine/Render/Textures/
//  MODULE: Rendering – Texture Abstraction Layer
//
//  ROLE:
//      Deterministic engine texture wrapper providing immutable metadata and GPU resource binding.
//
//  RESPONSIBILITIES:
//      - Hold immutable texture metadata (width, height, format, flags).
//      - Provide deterministic construction from decoded pixel data.
//      - Provide GPUTexture reference (assigned externally by the GPU upload subsystem).
//      - Track disposal state deterministically.
//
//  NON-RESPONSIBILITIES:
//      - PNG decoding (handled by CompositeImageLoader).
//      - GPU upload (handled by Texture2D / GPU subsystem).
//      - Logging, diagnostics, or sampling operations.
//
//  ARCHITECTURAL NOTES:
//      - Pure descriptor + resource holder.
//      - No mutation except deterministic disposal.
//      - Stable identity for Resource Management Framework.
// ====================================================================================================

using System;
using System.Drawing.Imaging;
using SASZombieAssaultTD.Engine.Interfaces;
using SASZombieAssaultTD.Engine.Render.Composite;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    public sealed class EngineTexture : IEngineTexture
    {
        // Immutable metadata
        public int Width { get; }
        public byte[] Pixels { get; }
        public int Height { get; }
        public GpuTextureFormat Format { get; internal set; }
        public EngineTextureFlags Flags { get; }

        // GPU resource (assigned by GPU upload subsystem)
        public Texture2D GPUTexture { get; private set; }

        // Disposal state
        public bool IsDisposed { get; private set; }

        // Raw pixel data (CPU-side)
        public byte[] PixelData { get; } = null;
        object IEngineTexture.Format
        {
            get => Format;
            set
            {
                // Accept the strongly-typed enum directly
                if (value is GpuTextureFormat enumValue)
                {
                    Format = enumValue;
                    return;
                }

                // null is not a valid value for the format
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                // Accept a numeric representation (common when boxed from integral types)
                if (value is int i)
                {
                    Format = (GpuTextureFormat)i;
                    return;
                }

                if (value is long l)
                {
                    Format = (GpuTextureFormat)l;
                    return;
                }

                // Accept a string representation of the enum name
                if (value is string s)
                {
                    if (Enum.TryParse(typeof(GpuTextureFormat), s, ignoreCase: true, out var parsed) && parsed is GpuTextureFormat parsedEnum)
                    {
                        Format = parsedEnum;
                        return;
                    }
                }

                // If we reach here, the provided value cannot be converted to the expected enum
                throw new ArgumentException("Unsupported value for Format; expected a SASZombieAssaultTD.Engine.Render.Composite.GpuTextureFormat (or equivalent).", nameof(value));
            }
        }

        // Deterministic constructor
        public EngineTexture(int width, int height, GpuTextureFormat format, EngineTextureFlags flags, byte[] pixelData)
        {
            Width = width;
            Height = height;
            Format = format;
            Flags = flags;
            PixelData = pixelData ?? throw new ArgumentNullException(nameof(pixelData));
        }

        // GPU binding performed by the upload subsystem
        public void BindGPUTexture(Texture2D gpuTexture)
        {
            GPUTexture = gpuTexture;
        }

        // Deterministic disposal
        public void Dispose()
        {
            IsDisposed = true;
        }

        // REQUIRED BY CompositeImageLoader.cs
        public static EngineTexture FromPng(byte[] pngBytes)
        {
            if (pngBytes == null)
                throw new ArgumentNullException(nameof(pngBytes));

            // Decode PNG bytes into a 32bpp ARGB bitmap and extract pixel data as BGRA (byte per channel)
            using (var ms = new System.IO.MemoryStream(pngBytes))
            using (var srcBmp = new System.Drawing.Bitmap(ms))
            using (var bmp = new System.Drawing.Bitmap(srcBmp.Width, srcBmp.Height, PixelFormat.Format32bppArgb))
            {
                // Draw the source into a known pixel format to simplify copying
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.DrawImage(srcBmp, 0, 0, srcBmp.Width, srcBmp.Height);
                }

                var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    int width = bmp.Width;
                    int height = bmp.Height;
                    int stride = System.Math.Abs(bmpData.Stride);

                    // Allocate tightly-packed pixel buffer (width * height * 4)
                    byte[] pixels = new byte[width * height * 4];

                    // If stride equals width*4 we can copy in one shot, otherwise copy row by row
                    if (stride == width * 4)
                    {
                        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, pixels.Length);
                    }
                    else
                    {
                        // Copy each row (skip any padding bytes at the end of each scanline)
                        byte[] row = new byte[stride];
                        nint basePtr = bmpData.Scan0;
                        for (int y = 0; y < height; y++)
                        {
                            nint rowPtr = nint.Add(basePtr, y * bmpData.Stride);
                            System.Runtime.InteropServices.Marshal.Copy(rowPtr, row, 0, stride);
                            Buffer.BlockCopy(row, 0, pixels, y * width * 4, width * 4);
                        }
                    }

                    // Bitmap.Format32bppArgb stores bytes in memory as BGRA on little-endian systems.
                    var format = GpuTextureFormat.BGRA32;
                    var flags = EngineTextureFlags.None;

                    return new EngineTexture(width, height, format, flags, pixels);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }
        }
    }
}
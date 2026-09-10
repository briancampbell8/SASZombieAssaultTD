// =====================================================================================================
//  FILE: TextureLoaderGPU.cs
//  PATH: Engine/TextureRendering/TextureLoaderGPU.cs
//  SUBSYSTEM: Engine Texture Rendering (GPU)
//
//  ROLE:
//      GPU texture loading utility that creates ID3D11Texture2D instances from PNG byte buffers using
//      ImageSharp, backed by the D3D11DeviceCore hardware abstraction.
//
//  RESPONSIBILITIES:
//      - Validate D3D11DeviceCore and PNG byte buffer inputs.
//      - Ensure the underlying D3D11Device is initialized before issuing GPU resource creation calls.
//      - Decode PNG data into RGBA32 using ImageSharp.
//      - Convert RGBA → BGRA for DXGI_FORMAT.B8G8R8A8_UNorm compatibility.
//      - Create a single‑mip, non‑array ID3D11Texture2D with deterministic texture description.
//
//  NON-RESPONSIBILITIES:
//      - Swap chain creation, backbuffer RTV management, or resize behavior.
//      - Shader resource view (SRV) creation or binding into any pipeline state.
//      - Lifetime management of textures beyond initial creation.
//      - Any draw calls or render pass orchestration.
//
//  ARCHITECTURAL NOTES:
//      - Relies on D3D11DeviceCore as the authoritative GPU device owner.
//      - Throws a clear InvalidOperationException if the GPU device is not initialized.
//      - Intended to be used by higher‑level systems (e.g., GameRootInitialization, TextureManager,
//        UITextureAtlasManager) for deterministic GPU texture creation from PNG assets.
// =====================================================================================================

using System;
using System.Runtime.InteropServices;
using SASZombieAssaultTD.Engine.Render.D3D11.DeviceCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Vortice.Direct3D11;
using Vortice.DXGI;
using D3D11Device = Vortice.Direct3D11.ID3D11Device;
using D3DTexture2D = Vortice.Direct3D11.ID3D11Texture2D;

namespace SASZombieAssaultTD.Engine.TextureRendering
{
    internal static class TextureLoaderGPU
    {
        /// <summary>
        /// Creates a GPU-resident ID3D11Texture2D from PNG byte data using ImageSharp. The texture is created with a
        /// single mip level and no array slices.
        /// </summary>
        public static D3DTexture2D CreateTextureFromPng(D3D11DeviceCore deviceCore, byte[] pngBytes)
        {
            if (deviceCore == null)
                throw new ArgumentNullException(nameof(deviceCore));

            if (pngBytes == null || pngBytes.Length == 0)
                throw new ArgumentException("PNG byte buffer is null or empty.", nameof(pngBytes));

            D3D11Device device = deviceCore.Device;
            if (device == null)
                throw new InvalidOperationException("D3D11DeviceCore.Device is null. GPU device not initialized.");

            // Decode PNG into RGBA32 using ImageSharp.
            using Image<Rgba32> image = Image.Load<Rgba32>(pngBytes);

            int width = image.Width;
            int height = image.Height;

            // Convert RGBA → BGRA for DXGI_FORMAT.B8G8R8A8_UNorm.
            int bytesPerPixel = 4;
            int stride = width * bytesPerPixel;
            int bufferSize = stride * height;
            byte[] pixelBuffer = new byte[bufferSize];

            int offset = 0;
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < height; y++)
                {
                    var rowSpan = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        Rgba32 p = rowSpan[x];
                        pixelBuffer[offset++] = p.B;
                        pixelBuffer[offset++] = p.G;
                        pixelBuffer[offset++] = p.R;
                        pixelBuffer[offset++] = p.A;
                    }
                }
            });

            var texDesc = new Texture2DDescription
            {
                Width = (uint)width,
                Height = (uint)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CPUAccessFlags = CpuAccessFlags.None,
                MiscFlags = ResourceOptionFlags.None
            };

            GCHandle handle = GCHandle.Alloc(pixelBuffer, GCHandleType.Pinned);

            try
            {
                var initialData = new SubresourceData
                {
                    DataPointer = handle.AddrOfPinnedObject(),
                    RowPitch = (uint)stride,
                    SlicePitch = (uint)bufferSize
                };

                return device.CreateTexture2D(texDesc, new[] { initialData });
            }
            finally
            {
                handle.Free();
            }
        }
    }
}

//
//    File:      EngineTexture.cs
//  Program:    Engine Texture Module
//  Purpose:    Engine-native texture storage with PNG import/export capabilities.
//              Provides texture management with pixel data manipulation and GPU upload.
//  Author:     BDC
//  Created:    2026-02-10
//
//  Dependencies:
//    - Engine.Rendering.Interfaces (IEngineTexture)
//    - Vortice.Direct3D11
//    - Vortice.DXGI
//    - SixLabors.ImageSharp (v3.1.3 - MIT licensed)
//    - SixLabors.ImageSharp.PixelFormats
//    - SixLabors.ImageSharp.Formats.Png
//
//  Thread Safety:
//    - Instance members require external synchronization for thread safety.
//
//  Notes:
//    - Stores texture width, height, and pixel data in engine format (int[] ARGB32).
//    - Uses ImageSharp for PNG import/export (RGBA32).
//    - Converts between ARGB32 (engine) and RGBA32 (ImageSharp / GPU).
//    - Provides integrated D3D11 ShaderResourceView creation via CreateNativeHandle.
//    - No System.Drawing / GDI+ usage; fully .NET 8 compatible.
//

using Engine.Rendering.Interfaces;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using Vortice.Direct3D11;
using Vortice.DXGI;

// Update line 39 in EngineTexture.cs:
public sealed class EngineTexture : IEngineTexture, ITexture2D

{
    private readonly int _width;
    private readonly int _height;
    private readonly int[] _pixels;

    public ID3D11ShaderResourceView? NativeHandle { get; set; }

    public int Width => _width;
    public int Height => _height;
    public int[] Pixels => _pixels;

    public EngineTexture(int width, int height, int[] pixels)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (pixels == null) throw new ArgumentNullException(nameof(pixels));
        if (pixels.Length != width * height)
            throw new ArgumentException("Pixel buffer length does not match width * height.", nameof(pixels));

        _width = width;
        _height = height;
        _pixels = pixels;
    }

    // ------------------------------------------------------------
    // PNG LOAD
    // ------------------------------------------------------------
    public static EngineTexture FromPng(byte[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        using var image = Image.Load<Rgba32>(data);

        int width = image.Width;
        int height = image.Height;
        var pixels = new int[width * height];

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < height; y++)
            {
                var rowSpan = accessor.GetRowSpan(y);
                for (int x = 0; x < width; x++)
                {
                    Rgba32 rgba = rowSpan[x];
                    pixels[y * width + x] = Rgba32ToArgb(rgba);
                }
            }
        });

        return new EngineTexture(width, height, pixels);
    }

    // ------------------------------------------------------------
    // PNG SAVE
    // ------------------------------------------------------------
    public byte[] ToPng()
    {
        using var image = new Image<Rgba32>(_width, _height);

        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < _height; y++)
            {
                var rowSpan = accessor.GetRowSpan(y);
                for (int x = 0; x < _width; x++)
                {
                    int argb = _pixels[y * _width + x];
                    ArgbToRgba32(argb, out Rgba32 rgba);
                    rowSpan[x] = rgba;
                }
            }
        });

        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, new PngEncoder());
        return memoryStream.ToArray();
    }

    // ------------------------------------------------------------
    // GPU UPLOAD (D3D11)
    // ------------------------------------------------------------
    public void CreateNativeHandle(ID3D11Device device)
    {
        if (device == null) throw new ArgumentNullException(nameof(device));

        int pixelCount = _width * _height;
        var gpuData = new byte[pixelCount * 4];

        // Convert ARGB32 → RGBA8
        for (int i = 0; i < pixelCount; i++)
        {
            int argb = _pixels[i];

            byte a = (byte)((argb >> 24) & 0xFF);
            byte r = (byte)((argb >> 16) & 0xFF);
            byte g = (byte)((argb >> 8) & 0xFF);
            byte b = (byte)(argb & 0xFF);

            int baseIndex = i * 4;
            gpuData[baseIndex + 0] = r;
            gpuData[baseIndex + 1] = g;
            gpuData[baseIndex + 2] = b;
            gpuData[baseIndex + 3] = a;
        }

        var textureDesc = new Texture2DDescription
        {
            Width = (uint)_width,
            Height = (uint)_height,
            MipLevels = 1u,
            ArraySize = 1u,
            Format = Format.R8G8B8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Immutable,
            BindFlags = BindFlags.ShaderResource,
            CPUAccessFlags = CpuAccessFlags.None,
            MiscFlags = ResourceOptionFlags.None
        };

        int rowPitch = _width * 4;

        unsafe
        {
            fixed (byte* ptr = gpuData)
            {
                var subresource = new SubresourceData((nint)ptr, (uint)rowPitch, 0);

                using var texture = device.CreateTexture2D(textureDesc, new[] { subresource });

                NativeHandle?.Dispose();
                NativeHandle = device.CreateShaderResourceView(texture);
            }
        }
    }

    // ------------------------------------------------------------
    // Pixel Conversion Helpers
    // ------------------------------------------------------------
    private static int Rgba32ToArgb(Rgba32 rgba)
    {
        return (rgba.A << 24) | (rgba.R << 16) | (rgba.G << 8) | rgba.B;
    }

    private static void ArgbToRgba32(int argb, out Rgba32 rgba)
    {
        byte a = (byte)((argb >> 24) & 0xFF);
        byte r = (byte)((argb >> 16) & 0xFF);
        byte g = (byte)((argb >> 8) & 0xFF);
        byte b = (byte)(argb & 0xFF);

        rgba = new Rgba32(r, g, b, a);
    }

    internal void CreateNativeHandle(object device)
    {
        NI.Hit();
    }

    //internal void CreateNativeHandle(object deviceCore) : Already defined above with ID3D11Device parameter
    // {
    //    NI.Hit();
    // }
}

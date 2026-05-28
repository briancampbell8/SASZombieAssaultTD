//// Program Name: CompositeImageLoader.cs
//// File Path: Engine/Rendering/CompositeImageLoader.cs
//// Program Purpose: The program loads PNG files from disk and converts them into engine-native texture format.
//// Program Features:
//// - Loads PNG files from disk as byte arrays
//// - Converts PNG byte data to engine texture format via EngineTexture.FromPng

using SASZombieAssaultTD.Engine.Diagnostics;

using Engine.Rendering.Interfaces;
using System;
using System.IO;

public sealed class CompositeImageLoader : ICompositeImageLoader
{
    public IEngineTexture LoadTexture(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        return EngineTexture.FromPng(data);
    }

    internal EngineTexture LoadPng(string mapPath)
    {
        return NI.Hit<EngineTexture>();
    }
}

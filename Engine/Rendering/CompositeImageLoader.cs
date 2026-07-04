// ====================================================================================================
//  FILE: CompositeImageLoader.cs
//  PATH: Engine/Rendering/ 
//  PROGRAM: CompositeImageLoader.cs
//  MODULE: Resource Management Framework
//  ROLE:
//      Defines the structures, loaders, and integration points responsible for discovering, validating, and providing engine resources in a deterministic manner.
//
//  RESPONSIBILITIES:
//      - Provide a unified API for loading, caching, and resolving engine resources.
//      - Enforce deterministic resource lookup and lifecycle rules.
//      - Abstract file formats, storage locations, and integration layers behind a stable interface.
//      - Ensure resource availability for all engine subsystems (Rendering, Audio, Gameplay, UI).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing gameplay logic or scene entities.
//      - Handling diagnostics, logging, or performance metrics.
//      - Encoding or authoring resource files.
//
//  ARCHITECTURAL NOTES:
//      - The Resource Management Framework acts as the central authority for all asset retrieval.
//      - Resource modules must remain pure: no side effects outside resource acquisition and validation.
//      - All resource types (textures, data files, definitions, metadata) must follow deterministic load rules.
//  ====================================================================================================

using System.IO;
using Engine.Rendering.Interfaces;
using SASZombieAssaultTD.Engine.Diagnostics;

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

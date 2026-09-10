// ====================================================================================================
//  FILE: AssetPipeline_Runtime.cs
//  PATH: ./Engine/Resources/Assets/Pipeline/
//  MODULE: Runtime Loader
//
//  ROLE:
//      Provides runtime‑safe, deterministic access to processed asset bytes.
//      This subsystem is intentionally isolated from the editor/import pipeline.
//
//  RESPONSIBILITIES:
//      - Resolve logical asset paths to physical processed asset paths.
//      - Load raw bytes for textures, audio, data, etc.
//      - Provide a stable entry point for the rendering subsystem.
//
//  NOTES:
//      Updated to resolve paths relative to AppContext.BaseDirectory and support
//      runtime asset copying performed in EngineBootstrap.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;

namespace SASZombieAssaultTD.Engine.Resources.AssetPipeline
{
    public static class AssetPipeline_Runtime
    {
        /// <summary>
        /// Loads raw bytes for a logical asset path at runtime.
        /// Example: "Displays/MainMenu.png"
        /// </summary>
        public static byte[] LoadBytes(string logicalPath)
        {
            if (string.IsNullOrWhiteSpace(logicalPath))
                throw new ArgumentException("Logical asset path is null or empty.", nameof(logicalPath));

            // Resolve logical path relative to the runtime directory.
            string physicalPath = Path.Combine(AppContext.BaseDirectory, "Assets", logicalPath);

            if (!File.Exists(physicalPath))
                throw new FileNotFoundException($"Asset not found: {physicalPath}");

            return File.ReadAllBytes(physicalPath);
        }
    }
}

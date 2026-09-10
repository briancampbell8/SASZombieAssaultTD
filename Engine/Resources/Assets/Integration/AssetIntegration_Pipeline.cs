// ====================================================================================================
//  FILE: AssetIntegration_Pipeline.cs
//  PATH: ./Engine/Resources/Assets/Integration/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetIntegration_Pipeline module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        AssetIntegration_Pipeline.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Integration\AssetIntegration_Pipeline.cs
//Program:     AssetIntegration (Pipeline)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Provides the integration layer between the AssetManager subsystem and the
//    AssetPipeline subsystem. Handles raw asset processing, metadata conversion,
//    bundle creation coordination, and stream‑based asset loading. Acts as the
//    façade that exposes pipeline capabilities to the unified Asset System.
//
//Responsibilities:
//    • Process raw assets through the AssetPipeline subsystem.
//    • Coordinate bundle creation using processed asset outputs.
//    • Provide stream‑based asset loading for pipeline‑generated data.
//    • Expose metadata conversion and pipeline diagnostics.
//    • Provide unified error handling and logging for pipeline operations.
//    • Serve as the authoritative integration point for pipeline workflows.
//
//Architecture:
//    • Facade pattern simplifying access to pipeline subsystems.
//    • Thread‑safe initialization and lifecycle management.
//    • Event‑driven notifications for pipeline processing events.
//    • Configurable runtime behavior via AssetManager_Config.
//    • Integrated with AssetPipeline, AssetBundle, and AssetManager subsystems.
//
//Integration Points:
//    • AssetManager for unified asset lifecycle management.
//    • AssetPipeline subsystem for preprocessing and optimization.
//    • AssetBundle subsystem for bundle creation and distribution.
//    • RS subsystem (legacy) during migration.
//    • UI, Rendering, Audio, and Gameplay systems for unified asset access.
//
//Performance Characteristics:
//    • Minimal overhead through subsystem delegation.
//    • Lazy initialization where appropriate.
//    • Efficient resource cleanup and pipeline disposal.
//    • Thread‑safe operations with minimal contention.
//    • Background processing to avoid UI thread blocking.
//    • Intelligent caching with hash‑based change detection.
//
//Usage Example:
//    //Process raw assets
//    var results = await AssetSystem.ProcessRawAssetsAsync("raw_assets/");
//
//    //Create bundle from processed assets
//    var bundleResult = await AssetSystem.CreateBundleAsync("game_assets.bundle", processedAssets);
//
//    //Load assets from pipeline stream
//    var texture = await AssetSystem.LoadAssetFromPipelineAsync<Texture2D>("player.png");
//    var audio   = await AssetSystem.LoadAssetFromPipelineAsync<AudioClip>("explosion.wav");
//
//    //Monitor pipeline performance
//    var stats = AssetSystem.GetPipelineStats();
//    DLogger.Log($"Processed {stats.ProcessedAssets} assets, saved {stats.SpaceSaved} bytes");
//
//    //Validate pipeline integrity
//    var validation = await AssetSystem.ValidatePipelineAsync();
//    if (!validation.IsValid)
//        DLogger.Log($"Pipeline validation failed: {string.Join(", ", validation.Errors)}");
//============================================================================

//

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Threading;
using System.Threading.Tasks;



using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Resources
{
    public static partial class AssetSystem
    {
        //---------------------------------------------------------------------
        //RAW ASSET PROCESSING
        //---------------------------------------------------------------------

        public static async Task<IEnumerable<AssetProcessResult>> ProcessAssetsAsync(string inputDirectory, bool recursive = true)
        {
            EnsureInitialized();
            DLogger.Log(LogSubsystems.ResourcesAssetsIntegration,
                "Info", $"AssetSystem: Processing assets in '{inputDirectory}' (Recursive={recursive})");

            return await _pipeline.ProcessDirectoryAsync(inputDirectory, recursive);
        }

        //---------------------------------------------------------------------
        //BUNDLE CREATION
        //---------------------------------------------------------------------

        public static async Task<BundleCreationResult> CreateBundleAsync(
            string bundleName,
            IEnumerable<string> assetPaths,
            string outputPath,
            BundleCreationOptions options = null)
        {
            EnsureInitialized();

            DLogger.Log(LogSubsystems.ResourcesAssetsIntegration,
                "Info", $"AssetSystem: Creating bundle '{bundleName}' → {outputPath}");

            var assets = new Dictionary<string, string>();
            foreach (var path in assetPaths)
                assets[path] = Path.Combine("Assets/Processed", path);

            return await AssetBundle.CreateBundleAsync(assets, outputPath, options);
        }

        //---------------------------------------------------------------------
        //STREAM LOADING
        //---------------------------------------------------------------------

        private static async Task<T> LoadFromStreamAsync<T>(Stream stream, string key) where T : class
        {
            try
            {
                DLogger.Log(LogSubsystems.ResourcesAssetsIntegration,
                    "Debug", $"AssetSystem: Async stream load for '{key}'");
                await Task.Delay(1); //Placeholder
                return null;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesAssetsIntegration,
                    "Error", $"AssetSystem: Stream load failed for '{key}': {ex.Message}");
                return null;
            }
        }

        private static T LoadFromStreamSync<T>(Stream stream, string key) where T : class
        {
            try
            {
                DLogger.Log(LogSubsystems.ResourcesAssetsIntegration,
                    "Debug", $"AssetSystem: Sync stream load for '{key}'");
                return null;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.ResourcesAssetsIntegration,
                    "Error", $"AssetSystem: Stream load failed for '{key}': {ex.Message}");
                return null;
            }
        }

        //---------------------------------------------------------------------
        //METADATA CONVERSION
        //---------------------------------------------------------------------

        private static AssetMetadata ConvertBundleEntryToMetadata(BundleEntry entry)
        {
            return new AssetMetadata
            {
                Key = entry.Path,
                Name = Path.GetFileNameWithoutExtension(entry.Path),
                Path = entry.Path,
                Type = DetermineAssetType.FromPath(entry.Path),
                Format = Path.GetExtension(entry.Path).TrimStart('.'),
                SizeBytes = entry.OriginalSize,
                LastModified = entry.LastModified,
                Checksum = entry.Hash,
                IsCritical = false
            };
        }

        private static AssetType DetermineAssetTypeFromPath(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();

            return ext switch
            {
                ".png" or ".jpg" or ".jpeg" or ".bmp" or ".tga" or ".dds" => AssetType.Texture,
                ".wav" or ".mp3" or ".ogg" or ".flac" => AssetType.Audio,
                ".fbx" or ".obj" or ".dae" or ".3ds" => AssetType.Model,
                ".ttf" or ".otf" => AssetType.Font,
                ".json" or ".xml" or ".csv" => AssetType.Data,
                ".cs" or ".lua" => AssetType.Script,
                _ => AssetType.Unknown
            };
        }

        internal static async Task LoadAssetAsync<T>(string asset, CancellationToken cancellationToken)
        {
            NI.Hit();
        }

        internal static async Task LoadAssetAsync<T>(string asset, AssetPriority critical, CancellationToken cancellationToken)
        {
            NI.Hit();
        }
    }

    internal class DetermineAssetType
    {
        internal static AssetType FromPath(string path)
        {
            NI.Hit();
            return default(AssetType);
        }
    }
}


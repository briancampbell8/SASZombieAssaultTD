// ====================================================================================================
//  FILE: AssetPipeline_Import.cs
//  PATH: ./Engine/Resources/Assets/Pipeline/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetPipeline_Import module.
//
//  RESPONSIBILITIES:
//      - Provide ProcessAssetAsync() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//============================================================================
//File:        AssetPipeline_Import.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Pipeline\AssetPipeline_Import.cs
//Program:     AssetPipeline (Import)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Implements the primary asset import and processing workflow for the
//    AssetPipeline subsystem. Handles format detection, processor selection,
//    configuration resolution, caching, and execution of asset processing.
//    Converts raw source assets into optimized, game‑ready formats.
//
//Responsibilities:
//    • Process a single asset file from raw input to optimized output.
//    • Detect asset type from file extension and path.
//    • Select the appropriate processor for the asset type.
//    • Apply configuration (default or custom).
//    • Use hash‑based caching to avoid redundant processing.
//    • Produce detailed processing results and metadata.
//    • Serve as the authoritative import entry point for the pipeline.
//
//Architecture:
//    • Partial class — complements AssetPipeline_Core and other pipeline files.
//    • Modular processor system (TextureProcessor, AudioProcessor, ModelProcessor, etc.).
//    • Hash‑based caching for change detection.
//    • Async processing for concurrency and scalability.
//    • Stream‑based processing for large assets.
//
//Integration Points:
//    • AssetManager for asset lifecycle and loading.
//    • AssetBundle subsystem for packaging processed assets.
//    • AssetPipeline processors for type‑specific transformations.
//    • AssetIntegration_Pipeline for unified system‑level coordination.
//
//Performance Notes:
//    • First‑time processing: full conversion.
//    • Subsequent processing: near‑instant (cache hit).
//    • Supports concurrent processing of multiple assets.
//    • Designed for deterministic, reproducible output.
//
//Usage Example:
//    var result = await pipeline.ProcessAssetAsync("textures/player.png");
//    var model  = await pipeline.ProcessAssetAsync("models/character.fbx",
//                                                  "optimized/character.fbx");
//============================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.IO;
using System.Threading.Tasks;
//

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Resources.AssetPipeline
{
    public partial class AssetPipeline
    {
        //--------------------------------------------------------------------
        //PROCESS SINGLE ASSET
        //--------------------------------------------------------------------

        ///<summary>
        ///Processes a single asset file according to its type and configuration.
        ///Handles format detection, processor selection, caching, and execution.
        ///</summary>
        public async Task<AssetProcessResult> ProcessAssetAsync(
            string inputPath,
            string outputPath = null,
            AssetProcessorConfig config = null)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
                throw new ArgumentException("Input path cannot be null or empty", nameof(inputPath));

            var fullInputPath = Path.Combine(_inputPath, inputPath);

            if (!File.Exists(fullInputPath))
                throw new FileNotFoundException($"Input file not found: {fullInputPath}");

            //Default output path = same as input
            outputPath ??= inputPath;
            var fullOutputPath = Path.Combine(_outputPath, outputPath);

            //Ensure output directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));

            //Determine asset type
            var type = DetermineAssetManagerType(inputPath);

            //Resolve configuration
            config ??= _configs.GetValueOrDefault(type, new AssetProcessorConfig());

            //Cache key
            var cacheKey = GenerateCacheKey(inputPath, config);

            //Cache lookup
            if (_processingCache.TryGet(cacheKey, out var cachedObj) &&
                cachedObj is AssetProcessResult cached)
            {
                DLogger.Log(LogSubsystems.ResourcesAssetsPipeline,
                    "Debug",
                    $"AssetPipeline: Retrieved cached result for '{inputPath}'");
                return cached;
            }

            //Select processor
            var processor = GetProcessor(type);

            //Execute processing
            var result = await processor.ProcessAsync(fullInputPath, fullOutputPath, config);

            //Cache result
            _processingCache.Set(cacheKey, result, EstimateResultSize(result));

            DLogger.Log(LogSubsystems.ResourcesAssetsPipeline,
                "Info",
                $"AssetPipeline: Processed '{inputPath}' -> '{outputPath}' ({result.Status})");

            return result;
        }

        //--------------------------------------------------------------------
        //CACHE KEY GENERATION
        //--------------------------------------------------------------------

        ///<summary>
        ///Generates a cache key based on file timestamp and configuration.
        ///</summary>
        private string GenerateCacheKey(string inputPath, AssetProcessorConfig config)
        {
            var fileInfo = new FileInfo(Path.Combine(_inputPath, inputPath));
            var configHash = config.GetHashCode().ToString("X");

            return $"{inputPath}_{fileInfo.LastWriteTime.Ticks}_{configHash}";
        }

        //--------------------------------------------------------------------
        //RESULT SIZE ESTIMATION
        //--------------------------------------------------------------------

        ///<summary>
        ///Estimates the size of the processed asset for caching purposes.
        ///</summary>
        private long EstimateResultSize(AssetProcessResult result)
        {
            if (File.Exists(result.OutputPath))
                return new FileInfo(result.OutputPath).Length;

            return 1024; //Default estimate
        }
    }
}


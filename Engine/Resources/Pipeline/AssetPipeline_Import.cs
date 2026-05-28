// ============================================================================
// File Path: Engine/Resources/Pipeline/AssetPipeline_Import.cs
// File: AssetPipeline_Import.cs
// Program: AssetPipeline (Import)
// Subsystem: Resources / Asset Processing
//
// Purpose:
//     Implements the primary asset import and processing workflow for the
//     AssetPipeline subsystem. Handles format detection, processor selection,
//     configuration resolution, caching, and execution of asset processing.
//
// Responsibilities:
//     - Process a single asset file from raw input to optimized output
//     - Detect asset type from file extension and path
//     - Select appropriate processor for the asset type
//     - Apply configuration (default or custom)
//     - Use cache to avoid redundant processing
//     - Produce detailed processing results and metadata
//
// Architecture:
//     - Partial class: complements AssetPipeline_Core and other pipeline files
//     - Modular processor system (TextureProcessor, AudioProcessor, etc.)
//     - Hash‑based caching for change detection
//     - Async processing for concurrency and scalability
//
// Integration Points:
//     - AssetManager for asset lifecycle and loading
//     - AssetBundle for packaging processed assets
//     - AssetPipeline processors for type‑specific transformations
//
// Performance Notes:
//     - First‑time processing: full conversion
//     - Subsequent processing: near‑instant (cache hit)
//     - Supports concurrent processing of multiple assets
//
// Usage Example:
//     var result = await pipeline.ProcessAssetAsync("textures/player.png");
//     var model = await pipeline.ProcessAssetAsync("models/character.fbx", "optimized/character.fbx");
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources.AssetPipeline
{
    public partial class AssetPipeline
    {
        // --------------------------------------------------------------------
        // PROCESS SINGLE ASSET
        // --------------------------------------------------------------------

        /// <summary>
        /// Processes a single asset file according to its type and configuration.
        /// Handles format detection, processor selection, caching, and execution.
        /// </summary>
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

            // Default output path = same as input
            outputPath ??= inputPath;
            var fullOutputPath = Path.Combine(_outputPath, outputPath);

            // Ensure output directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));

            // Determine asset type
            var type = DetermineAssetManagerType(inputPath);

            // Resolve configuration
            config ??= _configs.GetValueOrDefault(type, new AssetProcessorConfig());

            // Cache key
            var cacheKey = GenerateCacheKey(inputPath, config);

            // Cache lookup
            if (_processingCache.TryGet(cacheKey, out var cachedObj) &&
                cachedObj is AssetProcessResult cached)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Debug",
                    $"AssetPipeline: Retrieved cached result for '{inputPath}'");
                return cached;
            }

            // Select processor
            var processor = GetProcessor(type);

            // Execute processing
            var result = await processor.ProcessAsync(fullInputPath, fullOutputPath, config);

            // Cache result
            _processingCache.Set(cacheKey, result, EstimateResultSize(result));

            System.Diagnostics.Debug.WriteLine(
                "Info",
                $"AssetPipeline: Processed '{inputPath}' -> '{outputPath}' ({result.Status})");

            return result;
        }

        // --------------------------------------------------------------------
        // CACHE KEY GENERATION
        // --------------------------------------------------------------------

        /// <summary>
        /// Generates a cache key based on file timestamp and configuration.
        /// </summary>
        private string GenerateCacheKey(string inputPath, AssetProcessorConfig config)
        {
            var fileInfo = new FileInfo(Path.Combine(_inputPath, inputPath));
            var configHash = config.GetHashCode().ToString("X");

            return $"{inputPath}_{fileInfo.LastWriteTime.Ticks}_{configHash}";
        }

        // --------------------------------------------------------------------
        // RESULT SIZE ESTIMATION
        // --------------------------------------------------------------------

        /// <summary>
        /// Estimates the size of the processed asset for caching purposes.
        /// </summary>
        private long EstimateResultSize(AssetProcessResult result)
        {
            if (File.Exists(result.OutputPath))
                return new FileInfo(result.OutputPath).Length;

            return 1024; // Default estimate
        }
    }
}

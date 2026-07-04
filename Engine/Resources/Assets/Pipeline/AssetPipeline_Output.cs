//============================================================================
//File:        AssetPipeline_Output.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Pipeline\AssetPipeline_Output.cs
//Program:     AssetPipeline (Output)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Implements optimization and output‑stage operations for processed assets.
//    Provides optimization workflows, quality adjustments, and extension
//    queries. Acts as the final transformation stage before assets are packaged
//    into bundles or consumed directly by the Asset System.
//
//Responsibilities:
//    • Optimize existing processed assets using configurable optimization levels.
//    • Apply processor‑specific optimization strategies.
//    • Provide supported extension queries for each asset type.
//    • Integrate with caching, configuration, and processor registry.
//    • Produce deterministic, reproducible optimized outputs.
//
//Architecture:
//    • Partial class — complements Core, Import, Discovery, Validation, and Types.
//    • Delegates optimization to asset‑specific processors.
//    • Uses AssetProcessorConfig for quality and optimization parameters.
//    • Stream‑based processing for large assets.
//    • Designed for deterministic, stable output across pipeline runs.
//
//Integration Points:
//    • AssetManager for asset lifecycle and loading.
//    • AssetBundle subsystem for packaging optimized assets.
//    • AssetPipeline processors for type‑specific optimization.
//    • AssetIntegration_Pipeline for unified system‑level coordination.
//
//Performance Notes:
//    • Optimization level affects processing time and output quality.
//    • Supports async operations for concurrency and scalability.
//    • Uses existing processor registry and configuration system.
//    • Hash‑based caching prevents redundant optimization work.
//
//Usage Example:
//    await pipeline.OptimizeAssetAsync("textures/player.png", OptimizationLevel.Balanced);
//    await pipeline.OptimizeAssetAsync("models/character.fbx", OptimizationLevel.Maximum);
//============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources.AssetPipeline
{
    public partial class AssetPipeline
    {
        //--------------------------------------------------------------------
        //OPTIMIZE EXISTING ASSET
        //--------------------------------------------------------------------

        ///<summary>
        ///Optimizes an existing processed asset using the specified optimization level.
        ///</summary>
        public async Task<AssetProcessResult> OptimizeAssetAsync(
            string assetPath,
            OptimizationLevel optimizationLevel = OptimizationLevel.Balanced)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(assetPath));

            var fullAssetPath = Path.Combine(_outputPath, assetPath);

            if (!File.Exists(fullAssetPath))
                throw new FileNotFoundException($"Asset not found: {fullAssetPath}");

            //Determine asset type
            var type = DetermineAssetManagerType(assetPath);

            //Select processor
            var processor = GetProcessor(type);

            //Clone or retrieve config
            var config = _configs.GetValueOrDefault(type, new AssetProcessorConfig());
            config.OptimizationLevel = optimizationLevel;

            //Execute optimization
            var result = await processor.OptimizeAsync(fullAssetPath, config);

            System.Diagnostics.Debug.WriteLine(
                "Info",
                $"AssetPipeline: Optimized '{assetPath}' ({optimizationLevel})");

            return result;
        }

        //--------------------------------------------------------------------
        //SUPPORTED EXTENSIONS
        //--------------------------------------------------------------------

        ///<summary>
        ///Gets supported file extensions for a given asset type.
        ///</summary>
        public IEnumerable<string> GetSupportedExtensions(AssetType type)
        {
            if (_processors.TryGetValue(type.ToString(), out var processor))
                return processor.SupportedExtensions;

            return Enumerable.Empty<string>();
        }
    }
}

// ============================================================================
// File Path: Engine/Resources/Pipeline/AssetPipeline_Output.cs
// File: AssetPipeline_Output.cs
// Program: AssetPipeline (Output)
// Subsystem: Resources / Asset Processing
//
// Purpose:
//     Implements optimization and output‑stage operations for processed assets.
//     Provides optimization workflows, quality adjustments, and extension queries.
//
// Responsibilities:
//     - Optimize existing processed assets using configurable optimization levels
//     - Apply processor‑specific optimization strategies
//     - Provide supported extension queries for each asset type
//     - Integrate with caching, configuration, and processor registry
//
// Architecture:
//     - Partial class: complements Core, Import, Validation, and Types files
//     - Delegates optimization to asset‑specific processors
//     - Uses AssetProcessorConfig for quality and optimization parameters
//
// Integration Points:
//     - AssetManager for asset lifecycle and loading
//     - AssetBundle for packaging optimized assets
//     - AssetPipeline processors for type‑specific optimization
//
// Performance Notes:
//     - Optimization level affects processing time and output quality
//     - Supports async operations for concurrency
//     - Uses existing processor registry and configuration system
//
// Usage Example:
//     await pipeline.OptimizeAssetAsync("textures/player.png", OptimizationLevel.Balanced);
//     await pipeline.OptimizeAssetAsync("models/character.fbx", OptimizationLevel.Maximum);
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources.AssetPipeline
{
    public partial class AssetPipeline
    {
        // --------------------------------------------------------------------
        // OPTIMIZE EXISTING ASSET
        // --------------------------------------------------------------------

        /// <summary>
        /// Optimizes an existing processed asset using the specified optimization level.
        /// </summary>
        public async Task<AssetProcessResult> OptimizeAssetAsync(
            string assetPath,
            OptimizationLevel optimizationLevel = OptimizationLevel.Balanced)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException("Asset path cannot be null or empty", nameof(assetPath));

            var fullAssetPath = Path.Combine(_outputPath, assetPath);

            if (!File.Exists(fullAssetPath))
                throw new FileNotFoundException($"Asset not found: {fullAssetPath}");

            // Determine asset type
            var type = DetermineAssetManagerType(assetPath);

            // Select processor
            var processor = GetProcessor(type);

            // Clone or retrieve config
            var config = _configs.GetValueOrDefault(type, new AssetProcessorConfig());
            config.OptimizationLevel = optimizationLevel;

            // Execute optimization
            var result = await processor.OptimizeAsync(fullAssetPath, config);

            System.Diagnostics.Debug.WriteLine(
                "Info",
                $"AssetPipeline: Optimized '{assetPath}' ({optimizationLevel})");

            return result;
        }

        // --------------------------------------------------------------------
        // SUPPORTED EXTENSIONS
        // --------------------------------------------------------------------

        /// <summary>
        /// Gets supported file extensions for a given asset type.
        /// </summary>
        public IEnumerable<string> GetSupportedExtensions(AssetType type)
        {
            if (_processors.TryGetValue(type.ToString(), out var processor))
                return processor.SupportedExtensions;

            return Enumerable.Empty<string>();
        }
    }
}

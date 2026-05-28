// ============================================================================
// File Path: Engine/Resources/Pipeline/AssetPipeline_Core.cs
// File: AssetPipeline_Core.cs
// Program: AssetPipeline (Core)
// Subsystem: Resources / Asset Processing
//
// Purpose:
//     Core initialization, configuration, processor registration, and lifecycle
//     management for the AssetPipeline subsystem. Provides the foundational
//     processing environment used to convert raw source assets into optimized,
//     game‑ready formats.
//
// Responsibilities:
//     - Initialize pipeline directories and processing cache
//     - Register built‑in asset processors (texture, audio, model, font, data)
//     - Maintain processor configurations per asset type
//     - Provide update mechanisms for processor settings
//     - Manage disposal and cleanup of processors and cache
//
// Architecture:
//     - Partial class: functionality split across multiple files
//         • AssetPipeline_Core.cs       — initialization, config, disposal
//         • AssetPipeline_Processing.cs — actual asset processing logic
//         • AssetPipeline_Validation.cs — validation and reporting
//         • AssetPipeline_Types.cs      — supporting types and enums
//
// Integration Points:
//     - AssetManager for asset lifecycle and loading
//     - AssetBundle for packaging processed assets
//     - AssetPipeline processors for type‑specific transformations
//     - RSManager for low‑level resource operations
//
// Performance Notes:
//     - Hash‑based caching prevents redundant processing
//     - Parallelizable architecture for batch operations
//     - Configurable quality vs. performance trade‑offs
//
// Usage Example:
//     var pipeline = new AssetPipeline("Assets/Raw", "Assets/Processed");
//     pipeline.UpdateConfig(AssetManagerType.Texture, new AssetProcessorConfig { Quality = 0.9f });
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.Resources.AssetPipeline
{
    public partial class AssetPipeline : IDisposable
    {
        // --------------------------------------------------------------------
        // FIELDS
        // --------------------------------------------------------------------

        /// <summary>
        /// Registry of asset processors indexed by asset type name.
        /// </summary>
        private readonly Dictionary<string, IAssetProcessor> _processors = new();

        /// <summary>
        /// Configuration settings for each asset type.
        /// </summary>
        private readonly Dictionary<AssetType, AssetProcessorConfig> _configs = new();

        /// <summary>
        /// Directory containing raw source assets.
        /// </summary>
        private readonly string _inputPath;

        /// <summary>
        /// Directory containing processed, optimized assets.
        /// </summary>
        private readonly string _outputPath;

        /// <summary>
        /// Cache system for avoiding redundant processing.
        /// </summary>
        private AssetCache _processingCache;

        /// <summary>
        /// Indicates whether this instance has been disposed.
        /// </summary>
        private bool _disposed;

        // --------------------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the AssetPipeline class.
        /// </summary>
        public AssetPipeline(string inputPath = "Assets/Raw", string outputPath = "Assets/Processed")
        {
            _inputPath = inputPath ?? throw new ArgumentNullException(nameof(inputPath));
            _outputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));

            Directory.CreateDirectory(_inputPath);
            Directory.CreateDirectory(_outputPath);

            _processingCache = new AssetCache();

            InitializeProcessors();
            InitializeConfigs();

            System.Diagnostics.Debug.WriteLine(
                "Info",
                $"AssetPipeline: Initialized (Input: {_inputPath}, Output: {_outputPath})");
        }

        // --------------------------------------------------------------------
        // CONFIGURATION
        // --------------------------------------------------------------------

        /// <summary>
        /// Updates processor configuration for an asset type.
        /// </summary>
        public void UpdateConfig(AssetType type, AssetProcessorConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _configs[type] = config;

            System.Diagnostics.Debug.WriteLine(
                "Debug",
                $"AssetPipeline: Updated config for {type}");
        }

        // --------------------------------------------------------------------
        // PROCESSOR INITIALIZATION
        // --------------------------------------------------------------------

        /// <summary>
        /// Registers all built‑in asset processors.
        /// </summary>
        private void InitializeProcessors()
        {
            RegisterProcessor(new TextureProcessor());
            RegisterProcessor(new AudioProcessor());
            RegisterProcessor(new ModelProcessor());
            RegisterProcessor(new FontProcessor());
            RegisterProcessor(new DataProcessor());
        }

        /// <summary>
        /// Initializes default processor configurations.
        /// </summary>
        private void InitializeConfigs()
        {
            _configs[AssetType.Texture] = new AssetProcessorConfig
            {
                CompressionEnabled = true,
                Quality = 0.8f,
                OptimizationLevel = OptimizationLevel.Balanced,
                GenerateMipmaps = true,
                MaxTextureSize = new Vector3Int(2048, 2048, 1)
            };

            _configs[AssetType.Audio] = new AssetProcessorConfig
            {
                CompressionEnabled = true,
                Quality = 0.7f,
                OptimizationLevel = OptimizationLevel.Balanced,
                SampleRate = 44100,
                BitDepth = 16
            };

            _configs[AssetType.Model] = new AssetProcessorConfig
            {
                CompressionEnabled = true,
                Quality = 0.9f,
                OptimizationLevel = OptimizationLevel.Balanced,
                OptimizeMesh = true,
                GenerateLODs = true
            };

            _configs[AssetType.Font] = new AssetProcessorConfig
            {
                CompressionEnabled = false,
                Quality = 1.0f,
                OptimizationLevel = OptimizationLevel.Quality,
                GenerateMipmaps = true,
                CharacterSet =
                    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;':\",./<>?"
            };
        }

        /// <summary>
        /// Registers a processor instance.
        /// </summary>
        private void RegisterProcessor(IAssetProcessor processor)
        {
            _processors[processor.AssetManagerType.ToString()] = processor;

            System.Diagnostics.Debug.WriteLine(
                "Debug",
                $"AssetPipeline: Registered processor for {processor.AssetManagerType}");
        }

        // --------------------------------------------------------------------
        // DISPOSAL
        // --------------------------------------------------------------------

        /// <summary>
        /// Disposes the pipeline and all associated processors.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _processingCache.Dispose();

            foreach (var processor in _processors.Values)
            {
                if (processor is IDisposable disposable)
                    disposable.Dispose();
            }

            _processors.Clear();

            System.Diagnostics.Debug.WriteLine("Info", "AssetPipeline: Disposed");
        }
    }

    // ------------------------------------------------------------------------
    // INTERNAL SUPPORTING TYPES
    // ------------------------------------------------------------------------

    internal class AssetCache
    {
        internal void Dispose()
        {
            NI.Hit();
        }

        internal void Set(string cacheKey, AssetProcessResult result, long timestamp)
        {
            NI.Hit();
        }

        internal bool TryGet(string cacheKey, out object cachedResult)
        {
            cachedResult = null;
            NI.Hit();
            return false;
        }
    }
}

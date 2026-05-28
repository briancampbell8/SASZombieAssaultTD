/* 
// PROGRAM: AssetPipeline_Types
// FILE PATH: Engine/Resources/Pipeline/AssetPipeline_Types.cs
// PURPOSE:
//   Supporting types and configurations for the asset processing pipeline.
//
// RESPONSIBILITIES:
//   - Define optimization levels, processing status, and asset categories.
//   - Provide configuration, result, and validation types for processors.
//   - Serve as the shared type system for all pipeline components.
using SASZombieAssaultTD.Engine.Diagnostics;

*/

using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Asset processing status for tracking operation states.
    /// </summary>
    public enum AssetProcessStatus
    {
        Pending,
        InProgress,
        Success,
        Failed
    }

    /// <summary>
    /// Asset categories for processing.
    /// </summary>
    public enum AssetManagerType
    {
        Unknown,
        Texture,
        Audio,
        Model,
        Font,
        Data,
        Script
    }

    /// <summary>
    /// Optimization levels for asset processing.
    /// </summary>
    public enum OptimizationLevel
    {
        Fastest,
        Fast,
        Balanced,
        Quality,
        Maximum
    }

    /// <summary>
    /// Asset processor configuration.
    /// </summary>
    public class AssetProcessorConfig
    {
        public bool CompressionEnabled { get; set; } = true;
        public float Quality { get; set; } = 0.8f;
        public OptimizationLevel OptimizationLevel { get; set; } = OptimizationLevel.Balanced;
        public bool GenerateMipmaps { get; set; } = false;
        public bool OptimizeMesh { get; set; } = false;
        public bool GenerateLODs { get; set; } = false;
        public int SampleRate { get; set; } = 44100;
        public int BitDepth { get; set; } = 16;
        public string CharacterSet { get; set; } = string.Empty;
        public SASZombieAssaultTD.Engine.VectorMath.Vector3Int MaxTextureSize { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Asset processing result.
    /// </summary>
    public class AssetProcessResult
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public AssetProcessStatus Status { get; set; }
        public string Error { get; set; } = string.Empty;
        public TimeSpan ProcessingTime { get; set; }
        public long OriginalSize { get; set; }
        public long ProcessedSize { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Asset validation result.
    /// </summary>
    public class AssetValidationResult
    {
        public bool IsValid { get; set; }
        public string AssetPath { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public TimeSpan ValidationTime { get; set; }
    }
}

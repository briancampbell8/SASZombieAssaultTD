// ====================================================================================================
//  FILE: AssetPipeline_Types.cs
//  PATH: ./Engine/Resources/Assets/Pipeline/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the AssetPipeline_Types module.
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
/*
//============================================================================
//File:        AssetPipeline_Types.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Pipeline\AssetPipeline_Types.cs
//Program:     AssetPipeline (Types)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Defines all supporting data structures, configuration types, processing
//    results, validation results, statistics objects, and enums used by the
//    AssetPipeline subsystem. Serves as the shared type system for all pipeline
//    components, ensuring deterministic processing and stable serialization.
//
//Responsibilities:
//    • Define optimization levels, processing status enums, and asset categories.
//    • Provide configuration types for processors (quality, compression, flags).
//    • Provide result types for import, optimization, and validation workflows.
//    • Provide diagnostic and statistics types for pipeline reporting.
//    • Serve as the authoritative shared type library for all pipeline partials.
//
//Architecture:
//    • Pure type definitions — no processing logic.
//    • Consumed by Core, Import, Discovery, Output, Processors, and Validation.
//    • Stable serialization contract — changes require migration planning.
//    • Designed for minimal memory overhead and fast lookup.
//
//Integration Points:
//    • AssetManager for asset lifecycle and loading.
//    • AssetBundle subsystem for packaging processed assets.
//    • AssetPipeline processors for type‑specific transformations.
//    • AssetIntegration_Pipeline for unified system‑level coordination.
//
//Notes:
//    • All types must remain deterministic and thread‑safe.
//    • No BGFX, no legacy backend references.
//    • Types must remain stable for caching and bundle compatibility.
//============================================================================
*/

//
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Asset processing status for tracking operation states.
    ///</summary>
    public enum AssetProcessStatus
    {
        Pending,
        InProgress,
        Success,
        Failed
    }

    ///<summary>
    ///Asset categories for processing.
    ///</summary>
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

    ///<summary>
    ///Optimization levels for asset processing.
    ///</summary>
    public enum OptimizationLevel
    {
        Fastest,
        Fast,
        Balanced,
        Quality,
        Maximum
    }

    ///<summary>
    ///Asset processor configuration.
    ///</summary>
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

    ///<summary>
    ///Asset processing result.
    ///</summary>
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

    ///<summary>
    ///Asset validation result.
    ///</summary>
    public class AssetValidationResult
    {
        public bool IsValid { get; set; }
        public string AssetPath { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public TimeSpan ValidationTime { get; set; }
    }
}


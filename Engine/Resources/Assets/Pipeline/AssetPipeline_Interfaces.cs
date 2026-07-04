/*
//============================================================================
//File:        AssetPipeline_Interfaces.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Pipeline\AssetPipeline_Interfaces.cs
//Program:     AssetPipeline (Interfaces)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Defines all public interface contracts used by the AssetPipeline subsystem,
//    including processor interfaces, validation interfaces, and configuration
//    contracts. These interfaces form the stable API surface for all pipeline
//    operations and type‑specific processing implementations.
//
//Responsibilities:
//    • Provide unified contracts for asset processors.
//    • Support asynchronous processing, optimization, and validation workflows.
//    • Define configuration and result interfaces for deterministic processing.
//    • Decouple pipeline logic from concrete processor implementations.
//    • Integrate cleanly with AssetManager, AssetBundle, and pipeline type system.
//
//Architecture:
//    • Pure interface definitions — no processing logic.
//    • Implemented by TextureProcessor, AudioProcessor, ModelProcessor,
//      FontProcessor, DataProcessor, and any custom processors.
//    • Stable serialization and processing contract — changes require migration.
//    • Consumed by AssetPipeline_Core, Import, Processing, and Validation units.
//
//Integration Points:
//    • AssetManager for asset lifecycle and loading.
//    • AssetBundle subsystem for packaging processed assets.
//    • AssetPipeline processors for type‑specific transformations.
//    • AssetIntegration_Pipeline for unified system‑level coordination.
//
//Notes:
//    • Interfaces must remain deterministic and thread‑safe.
//    • No BGFX, no legacy backend references.
//============================================================================
*/

//
using System.Collections.Generic;
using System.Threading.Tasks;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    ///<summary>
    ///Interface definitions for asset processing pipeline in SAS Zombie Assault TD.
    ///</summary>
    public interface IAssetProcessor
    {
        ///<summary>
        ///Gets the type of asset manager associated with this processor.
        ///</summary>
        AssetType AssetType { get; }

        ///<summary>
        ///Gets the list of file extensions supported by this processor.
        ///</summary>
        IEnumerable<string> SupportedExtensions { get; }
        object AssetManagerType { get; set; }

        ///<summary>
        ///Processes an asset asynchronously.
        ///</summary>
        Task<AssetProcessResult> ProcessAsync(string inputPath, string outputPath, AssetProcessorConfig config);

        ///<summary>
        ///Optimizes an asset asynchronously.
        ///</summary>
        Task<AssetProcessResult> OptimizeAsync(string assetPath, AssetProcessorConfig config);

        ///<summary>
        ///Validates an asset asynchronously.
        ///</summary>
        Task<AssetValidationResult> ValidateAsync(string assetPath);

        ///<summary>
        ///Updates the configuration for this processor.
        ///</summary>
        void UpdateConfig(AssetProcessorConfig config);
    }
}

/* 
// PROGRAM: AssetPipeline_Interfaces
// FILE PATH: Engine/Resources/AssetPipeline_Interfaces.cs
// PURPOSE:
//   Defines interface contracts for the asset processing pipeline.
//
// RESPONSIBILITIES:
//   - Provide unified contracts for asset processors.
//   - Support async processing, optimization, and validation workflows.
//   - Integrate with AssetManager and pipeline type system.
using SASZombieAssaultTD.Engine.Diagnostics;

*/

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Interface definitions for asset processing pipeline in SAS Zombie Assault TD.
    /// </summary>
    public interface IAssetProcessor
    {
        /// <summary>
        /// Gets the type of asset manager associated with this processor.
        /// </summary>
        AssetType AssetType { get; }

        /// <summary>
        /// Gets the list of file extensions supported by this processor.
        /// </summary>
        IEnumerable<string> SupportedExtensions { get; }
        object AssetManagerType { get; set; }

        /// <summary>
        /// Processes an asset asynchronously.
        /// </summary>
        Task<AssetProcessResult> ProcessAsync(string inputPath, string outputPath, AssetProcessorConfig config);

        /// <summary>
        /// Optimizes an asset asynchronously.
        /// </summary>
        Task<AssetProcessResult> OptimizeAsync(string assetPath, AssetProcessorConfig config);

        /// <summary>
        /// Validates an asset asynchronously.
        /// </summary>
        Task<AssetValidationResult> ValidateAsync(string assetPath);

        /// <summary>
        /// Updates the configuration for this processor.
        /// </summary>
        void UpdateConfig(AssetProcessorConfig config);
    }
}

/* Built by Copilot
 * File: AssetBundle_Interfaces.cs
 * Program: AssetBundle (Interfaces)
 * Subsystem: Bundles
 *
 * Purpose:
 *     Defines all public interfaces used by the AssetBundle subsystem,
 *     including creation, loading, processing, validation, metadata
 *     extraction, and factory construction.
 *
 * Responsibilities:
 *     - Provide stable contracts for all bundle operations
 *     - Decouple implementation from interface usage
 *     - Support synchronous and asynchronous workflows
 *     - Ensure deterministic behavior across all bundle components
 *
 * Architecture:
 *     These interfaces are implemented by the following classes:
 *         • DefaultAssetBundleCreator
 *         • DefaultAssetBundleLoader
 *         • DefaultAssetBundleProcessor
 *         • DefaultAssetBundleValidator
 *         • DefaultAssetBundleMetadata
 *         • AssetBundleFactory
 *
 * Integration Points:
 *     - Used by AssetManager for unified asset access
 *     - Used by AssetPipeline for bundle creation
 *     - Used by AssetBundle for loading and metadata access
 *
 * Notes:
 *     This file must remain stable. Interface changes require a full
 *     versioning review and migration plan.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    // ---------------------------------------------------------------------
    // CREATOR
    // ---------------------------------------------------------------------

    public interface IAssetBundleCreator
    {
        Task<BundleCreationResult> CreateBundleAsync(
            string bundlePath,
            Dictionary<string, string> assets,
            BundleCreationOptions options,
            CancellationToken cancellationToken = default);

        BundleCreationResult CreateBundle(
            string bundlePath,
            Dictionary<string, string> assets,
            BundleCreationOptions options);
    }

    // ---------------------------------------------------------------------
    // LOADER
    // ---------------------------------------------------------------------

    public interface IAssetBundleLoader
    {
        Task<AssetBundle> LoadBundleAsync(
            string bundlePath,
            CancellationToken cancellationToken = default);

        AssetBundle LoadBundle(string bundlePath);

        bool IsValidBundle(string bundlePath);
    }

    // ---------------------------------------------------------------------
    // PROCESSOR
    // ---------------------------------------------------------------------

    public interface IAssetBundleProcessor
    {
        Task<byte[]> CompressAsync(
            byte[] data,
            CompressionLevel level,
            CancellationToken cancellationToken = default);

        Task<byte[]> DecompressAsync(
            byte[] compressedData,
            CancellationToken cancellationToken = default);

        Task<byte[]> EncryptAsync(
            byte[] data,
            byte[] key,
            CancellationToken cancellationToken = default);

        Task<byte[]> DecryptAsync(
            byte[] encryptedData,
            byte[] key,
            CancellationToken cancellationToken = default);
    }

    // ---------------------------------------------------------------------
    // VALIDATOR
    // ---------------------------------------------------------------------

    public interface IAssetBundleValidator
    {
        Task<BundleValidationResult> ValidateBundleAsync(
            string bundlePath,
            CancellationToken cancellationToken = default);

        BundleValidationResult ValidateBundle(string bundlePath);

        Task<BundleHealthReport> AnalyzeBundleHealthAsync(
            AssetBundle bundle,
            CancellationToken cancellationToken = default);
    }

    public partial class BundleHealthReport
    {
        internal DateTime Timestamp;
        internal string BundlePath;
        internal bool IsLoaded;
    }

    public partial class BundleValidationResult
    {
        internal bool IsValid;
    }

    // ---------------------------------------------------------------------
    // METADATA
    // ---------------------------------------------------------------------

    public interface IAssetBundleMetadata
    {
        Task<BundleHeader> ExtractHeaderAsync(
            string bundlePath,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<BundleEntry>> GetAssetEntriesAsync(
            string bundlePath,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateHeaderAsync(
            string bundlePath,
            BundleHeader header,
            CancellationToken cancellationToken = default);
    }

    // ---------------------------------------------------------------------
    // FACTORY
    // ---------------------------------------------------------------------

    public interface IAssetBundleFactory
    {
        AssetBundle CreateBundle(string bundlePath);

        AssetBundle CreateBundle(string bundlePath, BundleCreationOptions options);

        IAssetBundleCreator GetCreator();

        IAssetBundleLoader GetLoader();

        IAssetBundleProcessor GetProcessor();

        IAssetBundleValidator GetValidator();

        IAssetBundleMetadata GetMetadata();
    }

    // ---------------------------------------------------------------------
    // FACTORY IMPLEMENTATION
    // ---------------------------------------------------------------------

    public class AssetBundleFactory : IAssetBundleFactory
    {
        private readonly IAssetBundleCreator _creator;
        private readonly IAssetBundleLoader _loader;
        private readonly IAssetBundleProcessor _processor;
        private readonly IAssetBundleValidator _validator;
        private readonly IAssetBundleMetadata _metadata;

        public AssetBundleFactory()
        {
            _creator = new DefaultAssetBundleCreator();
            _loader = new DefaultAssetBundleLoader();
            _processor = new DefaultAssetBundleProcessor();
            _validator = new DefaultAssetBundleValidator();
            _metadata = new DefaultAssetBundleMetadata();
        }

        public AssetBundleFactory(
            IAssetBundleCreator creator,
            IAssetBundleLoader loader,
            IAssetBundleProcessor processor,
            IAssetBundleValidator validator,
            IAssetBundleMetadata metadata)
        {
            _creator = creator ?? throw new ArgumentNullException(nameof(creator));
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        public AssetBundle CreateBundle(string bundlePath) =>
            new AssetBundle(bundlePath);

        public AssetBundle CreateBundle(string bundlePath, BundleCreationOptions options) =>
            new AssetBundle(bundlePath, options);

        public IAssetBundleCreator GetCreator() => _creator;
        public IAssetBundleLoader GetLoader() => _loader;
        public IAssetBundleProcessor GetProcessor() => _processor;
        public IAssetBundleValidator GetValidator() => _validator;
        public IAssetBundleMetadata GetMetadata() => _metadata;
    }
}

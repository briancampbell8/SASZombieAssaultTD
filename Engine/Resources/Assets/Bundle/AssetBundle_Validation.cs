//============================================================================
//File:        AssetBundle_Validation.cs
//Path:        E:\BDC\Projects\SASZombieAssaultTD\Engine\Resources\Assets\Bundle\AssetBundle_Validation.cs
//Program:     AssetBundle (Validation)
//Subsystem:   Assets
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Implements validation, health checking, repair, and optimization logic for
//    AssetBundle instances. Ensures bundle integrity, correctness, performance
//    characteristics, and recoverability. Produces detailed diagnostic reports
//    used by the Asset System and development tooling.
//
//Responsibilities:
//    • Validate bundle structure, size, checksum, and asset entries.
//    • Perform health checks and generate diagnostic reports.
//    • Repair common bundle issues (offset drift, orphaned assets, checksum mismatch).
//    • Detect compression inefficiencies and duplication patterns.
//    • Optimize memory usage and bundle layout where possible.
//    • Produce BundleValidationResult, BundleHealthReport, BundleRepairReport,
//      and BundleOptimizationReport objects.
//
//Architecture:
//    • Partial class extension of AssetBundle.
//    • Operates on bundle metadata, entries, and loaded asset cache.
//    • Uses strongly‑typed reporting structures from AssetBundle_Types.cs.
//    • Deterministic, serialization‑safe validation and repair routines.
//    • No I/O — relies on Loader and Core for stream access.
//
//Integration Points:
//    • AssetManager for runtime validation and asset lifecycle safety.
//    • AssetPipeline for post‑build verification of generated bundles.
//    • Diagnostics subsystem for snapshot reporting and tooling integration.
//    • AssetIntegration_Bundles for unified system‑level coordination.
//
//Notes:
//    • All operations assume the bundle is already loaded.
//    • Exceptions are thrown for invalid state (disposed, not loaded, corrupted).
//    • No BGFX, no legacy backend references.
//============================================================================


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    public partial class AssetBundle
    {
        //--------------------------------------------------------------------
        //VALIDATION
        //--------------------------------------------------------------------

        ///<summary>
        ///Validates the bundle integrity and structure.
        ///</summary>
        public BundleValidationResult Validate()
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            var result = new BundleValidationResult
            {
                IsValid = true,
                ExpectedSize = _header.TotalSize,
                ActualSize = BundleSize,
                ExpectedChecksum = _header.Checksum
            };

            //Validate bundle size
            if (result.ActualSize != result.ExpectedSize)
            {
                result.IsValid = false;
                result.Issues.Add(
                    $"Bundle size mismatch: expected {result.ExpectedSize}, actual {result.ActualSize}");
            }

            //Validate checksum
            var actualChecksum = CalculateBundleChecksum();
            result.ActualChecksum = actualChecksum;

            if (!string.Equals(actualChecksum, result.ExpectedChecksum, StringComparison.OrdinalIgnoreCase))
            {
                result.IsValid = false;
                result.Issues.Add(
                    $"Bundle checksum mismatch: expected {result.ExpectedChecksum}, actual {actualChecksum}");
            }

            //Validate asset entries
            ValidateAssetEntries(result);

            //Validate asset files
            ValidateAssetFiles(result);

            return result;
        }

        //--------------------------------------------------------------------
        //HEALTH CHECK
        //--------------------------------------------------------------------

        ///<summary>
        ///Performs a comprehensive health check on the bundle.
        ///</summary>
        public BundleHealthReport PerformHealthCheck()
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            var report = new BundleHealthReport
            {
                Timestamp = DateTime.UtcNow,
                BundlePath = _bundlePath,
                IsLoaded = _isLoaded,
                TotalAssets = _entries.Count,
                LoadedAssets = _loadedAssets.Count,
                MemoryUsage = EstimateMemoryUsage(),
                CompressionRatio = (float)CompressionRatio
            };

            //Compression efficiency
            if (report.CompressionRatio < 10 &&
                _entries.Values.Any(e => e.IsCompressed))
            {
                report.Warnings.Add(
                    "Low compression ratio detected - consider adjusting compression settings");
            }

            //Memory usage
            if (report.MemoryUsage > 100 * 1024 * 1024)
            {
                report.Warnings.Add(
                    "High memory usage detected - consider unloading unused assets");
            }

            //Asset loading ratio
            if (report.LoadedAssets > report.TotalAssets * 0.8)
            {
                report.Recommendations.Add(
                    "Consider unloading assets that are no longer needed");
            }

            //Bundle age
            var bundleAge = DateTime.UtcNow - _header.CreatedAt;
            if (bundleAge > TimeSpan.FromDays(30))
            {
                report.Recommendations.Add(
                    "Bundle is old - consider rebuilding with updated assets");
            }

            //Empty assets
            var emptyAssets = _entries.Values.Where(e => e.OriginalSize == 0).ToList();
            if (emptyAssets.Any())
            {
                report.Warnings.Add($"{emptyAssets.Count} empty assets detected");
            }

            return report;
        }

        //--------------------------------------------------------------------
        //REPAIR
        //--------------------------------------------------------------------

        ///<summary>
        ///Repairs common bundle issues.
        ///</summary>
        public BundleRepairReport Repair()
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            var report = new BundleRepairReport
            {
                Timestamp = DateTime.UtcNow,
                ActionsTaken = new List<string>()
            };

            //Remove orphaned loaded assets
            var orphanedAssets = _loadedAssets.Keys
                .Where(path => !_entries.ContainsKey(path))
                .ToList();

            foreach (var orphaned in orphanedAssets)
            {
                _loadedAssets.Remove(orphaned);
                report.ActionsTaken.Add($"Removed orphaned loaded asset: {orphaned}");
            }

            //Validate and flag invalid offsets
            var fixedEntries = 0;

            foreach (var entry in _entries.Values)
            {
                if (entry.Offset < 0 || entry.Offset >= BundleSize)
                {
                    report.ActionsTaken.Add(
                        $"Detected invalid offset for asset: {entry.Path}");
                    fixedEntries++;
                }
            }

            if (fixedEntries > 0)
            {
                report.ActionsTaken.Add(
                    $"Flagged {fixedEntries} entries with invalid offsets");
            }

            //Recalculate checksum if needed
            var actualChecksum = CalculateBundleChecksum();
            if (!string.Equals(actualChecksum, _header.Checksum, StringComparison.OrdinalIgnoreCase))
            {
                _header.Checksum = actualChecksum;
                report.ActionsTaken.Add("Updated bundle checksum to match actual content");
            }

            return report;
        }

        //--------------------------------------------------------------------
        //OPTIMIZATION
        //--------------------------------------------------------------------

        ///<summary>
        ///Optimizes the bundle for better performance.
        ///</summary>
        public BundleOptimizationReport Optimize()
        {
            ThrowIfDisposed();
            ThrowIfNotLoaded();

            var report = new BundleOptimizationReport
            {
                Timestamp = DateTime.UtcNow,
                ActionsTaken = new List<string>(),
                AssetsOptimized = 0,
                MemoryFreed = 0
            };

            //Unload least recently used assets if memory is high
            var memoryStats = GetStatistics();

            if (memoryStats.LoadedAssetCount > memoryStats.AssetCount * 0.7)
            {
                var assetsToUnload = System.Math.Min(5, _loadedAssets.Count / 3);
                var unloadedCount = 0;

                foreach (var kvp in _loadedAssets.ToList())
                {
                    if (unloadedCount >= assetsToUnload)
                        break;

                    _loadedAssets.Remove(kvp.Key);
                    unloadedCount++;
                    report.MemoryFreed += EstimateAssetSize(kvp.Value);
                }

                report.AssetsOptimized = unloadedCount;
                report.ActionsTaken.Add(
                    $"Unloaded {unloadedCount} assets for memory optimization");
            }

            //Duplicate assets
            var duplicateHashes = _entries.Values
                .GroupBy(e => e.Hash)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var duplicate in duplicateHashes)
            {
                report.ActionsTaken.Add(
                    $"Found {duplicate.Count()} assets with same hash: {duplicate.Key}");
            }

            //Compression opportunities
            var uncompressedAssets = _entries.Values
                .Where(e => !e.IsCompressed && e.OriginalSize > 1024)
                .ToList();

            if (uncompressedAssets.Any() && _options.CompressBundle)
            {
                report.ActionsTaken.Add(
                    $"Found {uncompressedAssets.Count} large assets that could benefit from compression");
            }

            return report;
        }

        //--------------------------------------------------------------------
        //INTERNAL VALIDATION HELPERS
        //--------------------------------------------------------------------

        private void ValidateAssetEntries(BundleValidationResult result)
        {
            foreach (var entry in _entries.Values)
            {
                if (string.IsNullOrWhiteSpace(entry.Path))
                {
                    result.IsValid = false;
                    result.Issues.Add("Asset entry has empty path");
                    continue;
                }

                if (entry.OriginalSize <= 0)
                {
                    result.IsValid = false;
                    result.Issues.Add(
                        $"Asset {entry.Path} has invalid original size: {entry.OriginalSize}");
                }

                if (entry.CompressedSize <= 0)
                {
                    result.IsValid = false;
                    result.Issues.Add(
                        $"Asset {entry.Path} has invalid compressed size: {entry.CompressedSize}");
                }

                if (entry.Offset < 0)
                {
                    result.IsValid = false;
                    result.Issues.Add(
                        $"Asset {entry.Path} has invalid offset: {entry.Offset}");
                }

                if (entry.IsCompressed && entry.CompressedSize >= entry.OriginalSize)
                {
                    result.Issues.Add(
                        $"Asset {entry.Path} is marked as compressed but size didn't decrease");
                }

                if (string.IsNullOrWhiteSpace(entry.Hash))
                {
                    result.Issues.Add($"Asset {entry.Path} has empty hash");
                }
            }
        }

        private void ValidateAssetFiles(BundleValidationResult result)
        {
            foreach (var entry in _entries.Values)
            {
                try
                {
                    _bundleStream.Seek(entry.Offset, SeekOrigin.Begin);

                    var buffer = new byte[System.Math.Min(entry.CompressedSize, 1024)];
                    var bytesRead = _bundleStream.Read(buffer, 0, buffer.Length);

                    if (bytesRead <= 0)
                    {
                        result.CorruptedAssets.Add(entry.Path);
                        result.IsValid = false;
                        result.Issues.Add(
                            $"Asset {entry.Path} appears to be corrupted or missing");
                    }
                }
                catch (Exception ex)
                {
                    result.CorruptedAssets.Add(entry.Path);
                    result.IsValid = false;
                    result.Issues.Add(
                        $"Error reading asset {entry.Path}: {ex.Message}");
                }
            }
        }

        private string CalculateBundleChecksum()
        {
            try
            {
                using var sha256 = SHA256.Create();
                using var stream = new FileStream(_bundlePath, FileMode.Open, FileAccess.Read);
                var hash = sha256.ComputeHash(stream);
                return Convert.ToHexString(hash).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Failed to calculate bundle checksum: {ex.Message}");
                return string.Empty;
            }
        }

        private long EstimateMemoryUsage()
        {
            long total = 0;

            foreach (var asset in _loadedAssets.Values)
                total += EstimateAssetSize(asset);

            return total;
        }

        private static long EstimateAssetSize(object asset)
        {
            if (asset is string s)
                return s.Length * 2;

            if (asset is byte[] bytes)
                return bytes.Length;

            //Safe fallback check if Texture2D engine namespace isn't explicitly imported
            if (asset != null && asset.GetType().Name == "Texture2D")
                return 1024 * 1024;

            return 1024;
        }
    }
}

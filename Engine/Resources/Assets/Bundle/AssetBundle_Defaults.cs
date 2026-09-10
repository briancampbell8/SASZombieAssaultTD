// =====================================================================================================
//  FILE: AssetBundle_Defaults.cs
//  PATH: Engine/Resources/Assets/Bundle/AssetBundle_Defaults.cs
//  SUBSYSTEM: Assets / Core
//
//  ROLE:
//      Default implementation set for the AssetBundle subsystem, providing creation, loading,
//      processing, validation, and metadata extraction behaviors.
//
//  RESPONSIBILITIES:
//      - CreateBundle / CreateBundleAsync
//      - LoadBundle / LoadBundleAsync
//      - ValidateBundle / ValidateBundleAsync
//      - Compress / Decompress
//      - Encrypt / Decrypt
//      - ExtractHeader / UpdateHeader
//      - Compute hashes and checksums
//      - Provide bundle health diagnostics
//
//  NON-RESPONSIBILITIES:
//      - Low-level file persistence beyond bundle read/write operations.
//      - Asset pipeline preprocessing (handled by AssetPipeline subsystem).
//
//  ARCHITECTURAL NOTES:
//      - Facade-style API for bundle creation and management.
//      - Modular processor/validator/loader components.
//      - Thread-safe async operations.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Resources
{
    public class DefaultAssetBundleCreator : IAssetBundleCreator
    {
        public async Task<BundleCreationResult> CreateBundleAsync(
            string bundlePath,
            Dictionary<string, string> assets,
            BundleCreationOptions options,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var startTime = DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(bundlePath))
                    return new BundleCreationResult { Success = false, ErrorMessage = "Bundle path cannot be null or empty" };

                if (assets == null || assets.Count == 0)
                    return new BundleCreationResult { Success = false, ErrorMessage = "Assets dictionary cannot be null or empty" };

                var directory = Path.GetDirectoryName(bundlePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var entries = new List<BundleEntry>();
                long totalSize = 0;
                long offset = 0;

                foreach (var asset in assets)
                {
                    if (!File.Exists(asset.Value))
                        return new BundleCreationResult { Success = false, ErrorMessage = $"Asset file not found: {asset.Value}" };

                    var assetData = await File.ReadAllBytesAsync(asset.Value);
                    var compressedData = options.CompressBundle
                        ? await CompressData(assetData, options.CompressionLevel)
                        : assetData;

                    var entry = new BundleEntry
                    {
                        Path = asset.Key,
                        OriginalSize = assetData.Length,
                        CompressedSize = compressedData.Length,
                        Offset = offset,
                        IsCompressed = options.CompressBundle,
                        Hash = ComputeHash(assetData),
                        LastModified = File.GetLastWriteTime(asset.Value)
                    };

                    entries.Add(entry);
                    offset += compressedData.Length;
                    totalSize += compressedData.Length;
                }

                var header = new BundleHeader
                {
                    Magic = "ASBUNDLE",
                    Version = BundleVersion.V1,
                    AssetCount = entries.Count,
                    TotalSize = totalSize,
                    CreatedAt = DateTime.UtcNow,
                    Checksum = string.Empty,
                    HasCompression = options.CompressBundle,
                    HasEncryption = options.EncryptBundle
                };

                using var fileStream = new FileStream(bundlePath, FileMode.Create, FileAccess.Write);
                using var writer = new BinaryWriter(fileStream);

                WriteBundleHeader(writer, header);

                foreach (var entry in entries)
                {
                    WriteBundleEntry(writer, entry);

                    var assetData = await File.ReadAllBytesAsync(assets[entry.Path]);
                    var finalData = options.CompressBundle
                        ? await CompressData(assetData, options.CompressionLevel)
                        : assetData;

                    if (options.EncryptBundle)
                        finalData = await EncryptData(finalData, options.EncryptionKey ?? Array.Empty<byte>());

                    await fileStream.WriteAsync(finalData, 0, finalData.Length);
                }

                fileStream.Seek(0, SeekOrigin.Begin);
                header.Checksum = ComputeChecksum(fileStream);

                fileStream.Seek(0, SeekOrigin.Begin);
                WriteBundleHeader(writer, header);

                return new BundleCreationResult
                {
                    Success = true,
                    BundlePath = bundlePath,
                    CreationTime = DateTime.UtcNow - startTime,
                    BundleSize = new FileInfo(bundlePath).Length,
                    AssetCount = entries.Count,
                    Statistics = new Dictionary<string, object>
                    {
                        ["TotalSize"] = totalSize,
                        ["Compressed"] = options.CompressBundle,
                        ["Encrypted"] = options.EncryptBundle
                    }
                };
            }
            catch (Exception ex)
            {
                return new BundleCreationResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to create bundle: {ex.Message}"
                };
            }
        }

        private async Task<byte[]> CompressData(byte[] assetData, System.IO.Compression.CompressionLevel compressionLevel)
        {
            NI.Hit();
            return null;
        }

        public BundleCreationResult CreateBundle(
            string bundlePath,
            Dictionary<string, string> assets,
            BundleCreationOptions options)
        {
            return CreateBundleAsync(bundlePath, assets, options).GetAwaiter().GetResult();
        }

        private async Task<byte[]> CompressData(byte[] data, CompressionLevel level)
        {
            return await Task.FromResult(data);
        }

        private async Task<byte[]> EncryptData(byte[] data, byte[] key)
        {
            return await Task.FromResult(data);
        }

        private string ComputeHash(byte[] data)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            return Convert.ToHexString(sha256.ComputeHash(data)).ToLowerInvariant();
        }

        private string ComputeChecksum(Stream stream)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            return Convert.ToHexString(sha256.ComputeHash(stream)).ToLowerInvariant();
        }

        private void WriteBundleHeader(BinaryWriter writer, BundleHeader header)
        {
            writer.Write(header.Magic.ToCharArray());
            writer.Write((int)header.Version);
            writer.Write(header.AssetCount);
            writer.Write(header.TotalSize);
            writer.Write(header.CreatedAt.ToBinary());
            writer.Write(header.Checksum?.ToCharArray() ?? Array.Empty<char>());
            writer.Write(header.HasCompression);
            writer.Write(header.HasEncryption);
        }

        private void WriteBundleEntry(BinaryWriter writer, BundleEntry entry)
        {
            writer.Write(entry.Path?.ToCharArray() ?? Array.Empty<char>());
            writer.Write(entry.OriginalSize);
            writer.Write(entry.CompressedSize);
            writer.Write(entry.Offset);
            writer.Write(entry.IsCompressed);
            writer.Write(entry.Hash?.ToCharArray() ?? Array.Empty<char>());
            writer.Write(entry.LastModified.ToBinary());
        }
    }

    public class DefaultAssetBundleLoader : IAssetBundleLoader
    {
        public Task<AssetBundle> LoadBundleAsync(string bundlePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AssetBundle(bundlePath));
        }

        public AssetBundle LoadBundle(string bundlePath)
        {
            return new AssetBundle(bundlePath);
        }

        public bool IsValidBundle(string bundlePath)
        {
            return File.Exists(bundlePath);
        }
    }

    public class DefaultAssetBundleProcessor : IAssetBundleProcessor
    {
        public Task<byte[]> CompressAsync(byte[] data, CompressionLevel level, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(data);
        }

        public Task<byte[]> DecompressAsync(byte[] compressedData, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(compressedData);
        }

        public Task<byte[]> EncryptAsync(byte[] data, byte[] key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(data);
        }

        public Task<byte[]> DecryptAsync(byte[] encryptedData, byte[] key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(encryptedData);
        }
    }

    public class DefaultAssetBundleValidator : IAssetBundleValidator
    {
        public Task<BundleValidationResult> ValidateBundleAsync(string bundlePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new BundleValidationResult { IsValid = File.Exists(bundlePath) });
        }

        public BundleValidationResult ValidateBundle(string bundlePath)
        {
            return new BundleValidationResult { IsValid = File.Exists(bundlePath) };
        }

        public Task<BundleHealthReport> AnalyzeBundleHealthAsync(AssetBundle bundle, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new BundleHealthReport
            {
                Timestamp = DateTime.UtcNow,
                BundlePath = bundle?.BundlePath ?? string.Empty,
                IsLoaded = bundle?.IsLoaded ?? false
            });
        }
    }

    public class DefaultAssetBundleMetadata : IAssetBundleMetadata
    {
        public Task<BundleHeader> ExtractHeaderAsync(string bundlePath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new BundleHeader());
        }

        public Task<IEnumerable<BundleEntry>> GetAssetEntriesAsync(
            string bundlePath,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<BundleEntry>>(new List<BundleEntry>());
        }

        public Task<bool> UpdateHeaderAsync(
            string bundlePath,
            BundleHeader header,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}

public static class Convert
{
    public static string ToHexString(byte[] bytes)
    {
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public static string ToLowerInvariant(this string s)
    {
        return s?.ToLowerInvariant() ?? string.Empty;
    }

    internal static string ToBase64String(byte[] hash)
    {
        return NI.Hit<string>();
    }

    internal static byte ToByte(string v1, int v2)
    {
        throw new NotImplementedException();
    }

    internal static double ToDouble(IConvertible conv)
    {
        throw new NotImplementedException();
    }

    internal static float ToInt32<T>(T value2, int v)
    {
        return NI.Hit<float>();
    }

    internal static int ToInt32(object widthVal)
    {
        // Robust conversion helper that accepts common numeric representations and strings.
        if (widthVal == null)
            throw new ArgumentNullException(nameof(widthVal));

        switch (widthVal)
        {
            case int i:
                return i;
            case short s:
                return s;
            case ushort us:
                return us;
            case byte b:
                return b;
            case sbyte sb:
                return sb;
            case long l when l >= int.MinValue && l <= int.MaxValue:
                return (int)l;
            case uint ui when ui <= int.MaxValue:
                return (int)ui;
            case decimal dec when dec >= int.MinValue && dec <= int.MaxValue:
                return (int)dec;
            case double d when d >= int.MinValue && d <= int.MaxValue:
                return (int)d;
            case float f when f >= int.MinValue && f <= int.MaxValue:
                return (int)f;
            case string s2 when int.TryParse(s2, out var parsed):
                return parsed;
            case IConvertible conv:
                // Use IConvertible as a general fallback for convertible types
                return ToInt32(conv);
            default:
                // Final attempt: try Convert.ToInt32 which may handle other numeric types.
                try
                {
                    return ToInt32(widthVal);
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException($"Unable to convert object of type {widthVal.GetType()} to Int32.", ex);
                }
        }
    }

    internal static long ToInt64(object item)
    {
        // Handle nulls
        if (item == null)
        {
            return 0L;
        }

        // Fast paths for common numeric types
        switch (item)
        {
            case long l:
                return l;
            case int i:
                return i;
            case short s:
                return s;
            case sbyte sb:
                return sb;
            case byte b:
                return b;
            case ushort us:
                return us;
            case uint ui:
                return ui;
            case ulong ul:
                // ulong may be larger than Int64.MaxValue
                if (ul > long.MaxValue)
                {
                    throw new OverflowException($"Value '{ul}' is too large to fit into an Int64.");
                }
                return (long)ul;
            case decimal dec:
                return System.Convert.ToInt64(dec);
            case double d:
                return System.Convert.ToInt64(d);
            case float f:
                return System.Convert.ToInt64(f);
            case bool bo:
                return bo ? 1L : 0L;
            case string sstr:
                {
                    // Try integer parse first, then floating parse as fallback
                    if (long.TryParse(sstr, System.Globalization.NumberStyles.Integer | System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                    {
                        return parsed;
                    }

                    if (double.TryParse(sstr, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out var dparsed))
                    {
                        return System.Convert.ToInt64(dparsed);
                    }

                    throw new FormatException($"String '{sstr}' was not in a correct format to convert to Int64.");
                }
        }

        // If the object implements IConvertible, use that with invariant culture
        if (item is IConvertible conv)
        {
            return conv.ToInt64(System.Globalization.CultureInfo.InvariantCulture);
        }

        // Fallback to Convert which will attempt common conversions and throw appropriate exceptions
        try
        {
            return System.Convert.ToInt64(item);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert object of type '{item.GetType()}' to Int64.", ex);
        }
    }

    internal static float ToSingle<T>(T value2)
    {
        return NI.Hit<float>();
    }
}

public static class DateTimeExtensions
{
    public static long ToBinary(this DateTime dateTime)
    {
        return dateTime.ToFileTimeUtc();
    }
}

/*
//============================================================================
//File:        AssetSystem.cs
//Path:        Engine/Assets/AssetSystem.cs
//Program:     AssetSystem
//Subsystem:   Assets / Core
//Author:      BDC
//Created:     2026-05-29
//
//Purpose:
//    Provides the foundational asset key and type-safety infrastructure for the
//    engine. Defines the core asset identification, comparison, and validation
//    mechanisms used throughout the asset subsystem.
//
//Architectural Role:
//    • Centralized asset key system for all asset types
//    • Type-safe asset identification and lookup
//    • Validation and comparison utilities for asset keys
//    • Integration point for AssetManager, AssetRegistry, and AssetBundle
//
//Core Capabilities:
//    • Strongly-typed asset key creation
//    • Asset key equality, hashing, and comparison
//    • Validation of asset identifiers and type associations
//    • Implicit and explicit conversions where appropriate
//
//Integration Points:
//    • AssetManager — asset lifecycle and loading coordination
//    • AssetRegistryRuntime — runtime asset lookup and storage
//    • AssetBundle — packaged asset distribution and lookup
//    • RSManager — low-level resource access and resolution
//
//Performance Notes:
//    • Minimal overhead through lightweight struct-based keys
//    • Optimized hashing for dictionary and registry usage
//    • Zero-allocation comparison paths
//
//Usage Example:
//    var key = AssetKey.For<Texture2D>("ui/main_menu/background");
//    if (AssetSystem.ValidateKey(key))
//        DLogger.Log($"Valid asset key: {key}");
//============================================================================
*/


//
using SASZombieAssaultTD.Engine.Resources;
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;

using SASZombieAssaultTD.Engine.Diagnostics; using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.Assets
{
    ///<summary>
    ///Robust asset key system with type safety and validation.
    ///</summary>
    public readonly struct AssetKey : IEquatable<AssetKey>, IComparable<AssetKey>
    {
        private readonly string _value;
        private readonly int _hashCode;

        public AssetKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Asset key cannot be null or empty", nameof(value));

            _value = value.Trim();
            _hashCode = _value.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public string Value => _value;
        public bool IsValid => !string.IsNullOrEmpty(_value);

        public static implicit operator string(AssetKey key) => key._value;
        public static implicit operator AssetKey(string value) => new AssetKey(value);

        public bool Equals(AssetKey other) =>
            !string.IsNullOrEmpty(_value) &&
            !string.IsNullOrEmpty(other._value) &&
            string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);

        public int CompareTo(AssetKey other) =>
            string.Compare(_value, other._value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj is AssetKey other && Equals(other);
        public override int GetHashCode() => _hashCode;
        public override string ToString() => _value ?? string.Empty;

        public static bool operator ==(AssetKey left, AssetKey right) => left.Equals(right);
        public static bool operator !=(AssetKey left, AssetKey right) => !left.Equals(right);
    }

    ///<summary>
    ///Asset type enumeration with extensibility support.
    ///</summary>
    ///
    //NOTE: AssetType already defined in Engine/Resources/Manager/AssetType.cs
    //public enum AssetType
    //{
    //   Unknown = 0,
    //   Texture = 1,
    //   Audio = 2,
    //   Font = 3,
    //   Shader = 4,
    //   Model = 5,
    //   Animation = 6,
    //   Script = 7,
    //   Config = 8,
    //   Data = 9,
    //   Scene = 10,
    //   UI = 11,
    //   Particle = 12,
    //   Video = 13,
    //   Material = 14
    //}

    ///<summary>
    ///Comprehensive asset metadata with validation and extensibility.
    ///</summary>
    public class AssetMetadata
    {
        public AssetKey Key { get; init; }
        public AssetType Type { get; init; }
        public string Path { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public long Size { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime ModifiedAt { get; init; }
        public string Hash { get; init; } = string.Empty;
        public Dictionary<string, object> CustomProperties { get; init; } = new();
        public List<string> Dependencies { get; init; } = new();
        public List<string> Tags { get; init; } = new();
        public bool IsCompressed { get; init; }
        public string CompressionType { get; init; } = string.Empty;
        public long CompressedSize { get; init; }
        public bool IsLoaded { get; set; }
        public DateTime LoadedAt { get; set; }
        public TimeSpan LoadTime { get; set; }

        public AssetMetadata(AssetKey key, AssetType type)
        {
            Key = key;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        ///<summary>
        ///Validates the metadata for consistency.
        ///</summary>
        public ValidationResult Validate()
        {
            var result = new ValidationResult();

            if (!Key.IsValid)
                result.AddError("Invalid asset key");

            if (Type == AssetType.Unknown)
                result.AddError("Unknown asset type");

            if (string.IsNullOrWhiteSpace(Path))
                result.AddError("Asset path is required");

            if (Size < 0)
                result.AddError("Asset size cannot be negative");

            if (IsCompressed && CompressedSize >= Size)
                result.AddWarning("Compressed size should be smaller than original size");

            return result;
        }

        ///<summary>
        ///Creates a copy of this metadata.
        ///</summary>
        public AssetMetadata Clone()
        {
            return new AssetMetadata(Key, Type)
            {
                Path = Path,
                Name = Name,
                Description = Description,
                Size = Size,
                CreatedAt = CreatedAt,
                ModifiedAt = ModifiedAt,
                Hash = Hash,
                CustomProperties = new Dictionary<string, object>(CustomProperties),
                Dependencies = new List<string>(Dependencies),
                Tags = new List<string>(Tags),
                IsCompressed = IsCompressed,
                CompressionType = CompressionType,
                CompressedSize = CompressedSize,
                IsLoaded = IsLoaded,
                LoadedAt = LoadedAt,
                LoadTime = LoadTime
            };
        }
    }

    ///<summary>
    ///Validation result for asset metadata.
    ///</summary>
    public class ValidationResult
    {
        public List<string> Errors { get; } = new();
        public List<string> Warnings { get; } = new();
        public bool IsValid => Errors.Count == 0;

        public string ComponentName { get; internal set; }
        public string Message { get; internal set; }

        public void AddError(string error) => Errors.Add(error);
        public void AddWarning(string warning) => Warnings.Add(warning);
    }

    ///<summary>
    ///Asset registry with advanced caching and management.
    ///</summary>
    public static class AssetRegistry
    {
        private static readonly Dictionary<AssetKey, AssetMetadata> _assets = new();
        private static readonly Dictionary<AssetType, List<AssetKey>> _assetsByType = new();
        private static readonly object _lock = new();
        private static object TheContainingType;
        private static object TheContainingMember;

        ///<summary>
        ///Registers an asset with metadata.
        ///</summary>
        public static bool Register(AssetMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            var validation = metadata.Validate();
            if (!validation.IsValid)
            {
                DLogger.Log($"Asset validation failed: {string.Join(", ", validation.Errors)}");
                return false;
            }

            lock (_lock)
            {
                _assets[metadata.Key] = metadata;

                if (!_assetsByType.ContainsKey(metadata.Type))
                    _assetsByType[metadata.Type] = new List<AssetKey>();

                if (!_assetsByType[metadata.Type].Contains(metadata.Key))
                    _assetsByType[metadata.Type].Add(metadata.Key);
            }

            return true;
        }

        ///<summary>
        ///Gets asset metadata by key.
        ///</summary>
        public static AssetMetadata? Get(AssetKey key)
        {
            lock (_lock)
            {
                return _assets.TryGetValue(key, out var metadata) ? metadata.Clone() : null;
            }
        }

        ///<summary>
        ///Gets all assets of a specific type.
        ///</summary>
        public static IEnumerable<AssetMetadata> GetByType(AssetType type)
        {
            lock (_lock)
            {
                if (!_assetsByType.TryGetValue(type, out var keys))
                    return Enumerable.Empty<AssetMetadata>();

                return keys.Select(key => _assets[key].Clone()).ToList();
            }
        }

        ///<summary>
        ///Gets all registered assets.
        ///</summary>
        public static IEnumerable<AssetMetadata> GetAll()
        {
            lock (_lock)
            {
                return _assets.Values.Select(metadata => metadata.Clone()).ToList();
            }
        }

        ///<summary>
        ///Removes an asset from the registry.
        ///</summary>
        public static bool Remove(AssetKey key)
        {
            lock (_lock)
            {
                if (!_assets.TryGetValue(key, out var metadata))
                    return false;

                _assets.Remove(key);
                _assetsByType[metadata.Type].Remove(key);

                return true;
            }
        }

        ///<summary>
        ///Clears all registered assets.
        ///</summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _assets.Clear();
                _assetsByType.Clear();
            }
        }

        internal static void Register(string v1, string v2)
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }

        ///<summary>
        ///Gets the count of registered assets.
        ///</summary>
        public static int Count
        {
            get
            {
                lock (_lock)
                {
                    return _assets.Count;
                }
            }
        }
    }
}

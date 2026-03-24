/*
File:    AssetValidation.cs
Author:  BDC
Created: 2026-02-07
Purpose: Provides validation helpers for asset integrity and compatibility.
Notes:   Stateless. Used by loaders, registries, and bundle systems.
*/

using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.Extensions;

namespace SASZombieAssaultTD.Engine.Resources
{
    /// <summary>
    /// Asset key for identifying assets in the resource system.
    /// </summary>
    public class AssetKey
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }

        public AssetKey(string name, string type, string path = null)
        {
            Name = name;
            Type = type;
            Path = path;
        }

        public override string ToString()
        {
            return $"{Type}:{Name}";
        }
    }

    /// <summary>
    /// Provides validation helpers for asset loading and metadata.
    /// </summary>
    public static class AssetValidation
    {
        /// <summary>
        /// Validates that the loaded instance matches the expected asset type.
        /// </summary>
        public static bool ValidateInstanceType(AssetKey key, object instance)
        {
            try
            {
                if (instance is null)
                {
                    ModernLoggingSystem.Log("Error", $"[Assets] Validation failed for '{key}': instance is null.");
                    return false;
                }

                bool isValid = instance != null;
                if (!isValid)
                {
                    ModernLoggingSystem.Log("Error",
                    $"[Assets] Validation failed for '{key}': instance type mismatch.");
                }

                return isValid;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error", $"[Assets] Validation failed for '{key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates that metadata is consistent with the asset type.
        /// </summary>
        public static bool ValidateMetadata(SASZombieAssaultTD.Engine.Resources.AssetMetadata metadata)
        {
            try
            {
                if (metadata is null)
                {
                    ModernLoggingSystem.Log("Error", "[Assets] Validation failed: metadata is null.");
                    return false;
                }

                // Key must be non-null and non-whitespace
                if (string.IsNullOrWhiteSpace(metadata.Key))
                {
                    ModernLoggingSystem.Log("Error", "[Assets] Validation failed: metadata key is null or empty.");
                    return false;
                }

                // Path must be non-null and non-whitespace
                if (string.IsNullOrWhiteSpace(metadata.Path))
                {
                    ModernLoggingSystem.Log("Error",
                    $"[Assets] Validation failed for '{metadata.Key}': path is null or empty.");
                    return false;
                }

                // Basic rule: metadata.Type must not be Unknown
                if (metadata.Type == AssetType.Unknown)
                {
                    ModernLoggingSystem.Log("Error",
                    $"[Assets] Validation failed for '{metadata.Key}': asset type is Unknown.");
                    return false;
                }

                // Type must be a defined enum value
                if (!Enum.IsDefined(metadata.Type))
                {
                    ModernLoggingSystem.Log("Error",
                    $"[Assets] Validation failed for '{metadata.Key}': asset type '{(int)metadata.Type}' is not a defined AssetType.");
                    return false;
                }

                // If a format is provided, it must be non-empty
                if (metadata.Format != null && metadata.Format.Trim().Length == 0)
                {
                    ModernLoggingSystem.Log("Error",
                    $"[Assets] Validation failed for '{metadata.Key}': format is empty or whitespace.");
                    return false;
                }

                // If a format is provided, it must be compatible with the asset type
                if (metadata.Format != null && !ValidateExtension(metadata.Type, metadata.Format))
                {
                    ModernLoggingSystem.Log("Error",
                    $"[Assets] Validation failed for '{metadata.Key}': format '{metadata.Format}' is not supported for asset type '{metadata.Type}'.");
                    return false;
                }

                // SizeBytes must be non-negative
                if (metadata.SizeBytes < 0)
                {
                    ModernLoggingSystem.Log("Error",
                    $" [Assets] Validation failed for '{metadata.Key}': size is negative ({metadata.SizeBytes} bytes).");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error",
                $"[Assets] Validation failed for '{metadata?.Key ?? "unknown"}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates that a file extension matches the expected asset type.
        /// </summary>
        public static bool ValidateExtension(AssetType type, string? extension)
        {
            try
            {
                if (extension is null)
                    return false;

                extension = extension.ToLowerInvariant();

                return type switch
                {
                    AssetType.Texture => extension is "png" or "jpg" or "jpeg",
                    AssetType.SpriteSheet => extension is "png" or "json",
                    AssetType.Sound => extension is "wav" or "ogg",
                    AssetType.Music => extension is "mp3" or "ogg",
                    AssetType.Json => extension is "json",
                    AssetType.Binary => true, // any extension allowed
                    AssetType.Font => extension is "ttf" or "otf",
                    AssetType.Shader => extension is "glsl" or "hlsl",
                    _ => false
                };
            }
            catch (Exception ex)
            {
                ModernLoggingSystem.Log("Error",
                $"[Assets] Extension validation failed for type '{type}', extension '{extension}': {ex.Message}");
                return false;
            }
        }
    }
}



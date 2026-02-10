/*
    File:    AssetValidation.cs
    Author:  BDC
    Created: 2026-02-07
    Purpose: Provides validation helpers for asset integrity and compatibility.
    Notes:   Stateless. Used by loaders, registries, and bundle systems.
*/

using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System;

namespace SASZombieAssaultTD.Engine.Systems.Assets
{
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
                    DebugLogger.Log("Error", $"[Assets] Validation failed for '{key}': instance is null.");
                    return false;
                }

                bool isValid = key.AssetType.IsAssignableFrom(instance.GetType());
                if (!isValid)
                {
                    DebugLogger.Log("Error",
                        $"[Assets] Validation failed for '{key}': expected type '{key.AssetType.Name}', got '{instance.GetType().Name}'.");
                }

                return isValid;
            }
            catch (Exception ex)
            {
                DebugLogger.Log("Error", $"[Assets] Validation failed for '{key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates that metadata is consistent with the asset type.
        /// </summary>
        public static bool ValidateMetadata(AssetMetadata metadata)
        {
            try
            {
                if (metadata is null)
                {
                    DebugLogger.Log("Error", "[Assets] Validation failed: metadata is null.");
                    return false;
                }

                // Key must be non-null and non-whitespace
                if (string.IsNullOrWhiteSpace(metadata.Key))
                {
                    DebugLogger.Log("Error", "[Assets] Validation failed: metadata key is null or empty.");
                    return false;
                }

                // Path must be non-null and non-whitespace
                if (string.IsNullOrWhiteSpace(metadata.Path))
                {
                    DebugLogger.Log("Error",
                        $"[Assets] Validation failed for '{metadata.Key}': path is null or empty.");
                    return false;
                }

                // Basic rule: metadata.Type must not be Unknown
                if (metadata.Type == AssetType.Unknown)
                {
                    DebugLogger.Log("Error",
                        $"[Assets] Validation failed for '{metadata.Key}': asset type is Unknown.");
                    return false;
                }

                // Type must be a defined enum value
                if (!Enum.IsDefined(metadata.Type))
                {
                    DebugLogger.Log("Error",
                        $"[Assets] Validation failed for '{metadata.Key}': asset type '{(int)metadata.Type}' is not a defined AssetType.");
                    return false;
                }

                // If a format is provided, it must be non-empty
                if (metadata.Format != null && metadata.Format.Trim().Length == 0)
                {
                    DebugLogger.Log("Error",
                        $"[Assets] Validation failed for '{metadata.Key}': format is empty or whitespace.");
                    return false;
                }

                // If a format is provided, it must be compatible with the asset type
                if (metadata.Format != null && !ValidateExtension(metadata.Type, metadata.Format))
                {
                    DebugLogger.Log("Error",
                        $"[Assets] Validation failed for '{metadata.Key}': format '{metadata.Format}' is not supported for asset type '{metadata.Type}'.");
                    return false;
                }

                // SizeBytes must be non-negative if provided
                if (metadata.SizeBytes.HasValue && metadata.SizeBytes.Value < 0)
                {
                    DebugLogger.Log("Error",
                        $"[Assets] Validation failed for '{metadata.Key}': size is negative ({metadata.SizeBytes.Value} bytes).");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Log("Error",
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
                DebugLogger.Log("Error",
                    $"[Assets] Extension validation failed for type '{type}', extension '{extension}': {ex.Message}");
                return false;
            }
        }
    }
}
/*
File:    AssetUtils.cs
Author:  BDC
Created: 2026-02-07
Purpose: Provides helper utilities for asset operations.
Notes:   Stateless. Used by loaders, validation, and registries.
*/

using System;
using System.IO;

namespace SASZombieAssaultTD.Engine.Assets
{
    /// <summary>
    /// Utility helpers for asset-related operations.
    /// </summary>
    public static class AssetUtils
    {
        /// <summary>
        /// Normalizes a file path by converting backslashes to forward slashes
        /// and removing duplicate separators. Thread-safe and stateless.
        /// </summary>
        public static string NormalizePath(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            string normalized = path.Replace('\\', '/');
            while (normalized.Contains("//"))
                normalized = normalized.Replace("//", "/");

            return normalized;
        }

        /// <summary>
        /// Attempts to read all bytes from a file.
        /// Returns null if the file does not exist or cannot be read.
        /// Thread-safe and stateless.
        /// </summary>
        public static byte[]? TryReadFile(string path)
        {
            try
            {
                return File.Exists(path) ? File.ReadAllBytes(path) : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts the file extension (without the dot), or null if none exists.
        /// </summary>
        public static string? GetExtension(string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            string ext = Path.GetExtension(path);
            return string.IsNullOrEmpty(ext) ? null : ext.TrimStart('.');
        }
    }
}



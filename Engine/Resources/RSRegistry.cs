/*
File:    AssetRegistry.cs
Author:  BDC
Created: 2026-02-10

Purpose:
In-memory registry mapping asset keys to their identifiers/paths.

Notes:
All public members lock on _syncRoot for thread safety.
Dictionary<string, string> is not safe for concurrent read/write;
consistent locking is required.
All() returns a snapshot for safe iteration outside the lock.
*/
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Core;
using System;
using System.Collections.Generic;

namespace SASZombieAssaultTD.Engine.Resources
{
    public static class RSRegistry
    {
        private static readonly Dictionary<string, string> _assets = new Dictionary<string, string>();
        private static readonly object _syncRoot = new object();

        public static void Register(string key, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Asset key must not be null or empty.", nameof(key));

            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Asset path must not be null or empty.", nameof(relativePath));

            lock (_syncRoot)
            {
                if (!_assets.ContainsKey(key))
                    _assets[key] = relativePath;
            }
        }

        public static IReadOnlyDictionary<string, string> All()
        {
            lock (_syncRoot)
            {
                // Return a snapshot so callers iterate safely outside the lock.
                return new Dictionary<string, string>(_assets);
            }
        }

        /// <summary>Resolves the relative path for the given asset key.</summary>
        public static string Resolve(string key)
        {
            lock (_syncRoot)
            {
                if (_assets.TryGetValue(key, out string? path))
                    return path;
            }

            Engine.Diagnostics.DebugLogger.LogDebug("Warning", $"Asset key '{key}' not found in registry.");
            throw new KeyNotFoundException($"Asset key '{key}' is not registered.");
        }

        /// <summary>Attempts to resolve the relative path for the given asset key without throwing.</summary>
        public static bool TryResolve(string key, out string path)
        {
            lock (_syncRoot)
            {
                return _assets.TryGetValue(key, out path!);
            }
        }

        /// <summary>Returns true if the given asset key is registered.</summary>
        public static bool Contains(string key)
        {
            lock (_syncRoot)
            {
                return _assets.ContainsKey(key);
            }
        }

        /// <summary>Removes the entry associated with the given asset key.</summary>
        public static void Unregister(string key)
        {
            lock (_syncRoot)
            {
                _assets.Remove(key);
            }
        }

        /// <summary>Removes all entries from the registry.</summary>
        public static void Clear()
        {
            lock (_syncRoot)
            {
                _assets.Clear();
            }
        }

        /// <summary>Gets the number of registered assets.</summary>
        public static int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _assets.Count;
                }
            }
        }
    }
}




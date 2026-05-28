/*
File:    DataLoader.cs
Author:  BDC
Created: 2026-02-10

Purpose:
Loads data files from disk and parses JSON content when applicable.

Notes:
LoadAll was removed � AssetPipeline.LoadAll is the canonical bulk loader.

*/
using System;
using System.IO;
using System.Text.Json;

namespace SASZombieAssaultTD.Engine.Assets
{
    public static partial class DataLoader
    {
        /// <summary>
        /// Loads data from the specified file path.
        /// Returns the parsed object for JSON files or the raw text content.
        /// </summary>
        /// <param name="path">The path to the data file.</param>
        /// <returns>The parsed object or raw text content.</returns>
        public static object Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Data file not found: {path}");

            string content = File.ReadAllText(path);

            // Try to deserialize as JSON if it's a .json file
            if (path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var obj = JsonSerializer.Deserialize<object>(content);
                    return obj ?? content;
                }
                catch
                {
                    // If JSON parsing fails, return raw content
                    return content;
                }
            }

            return content;
        }
    }
}



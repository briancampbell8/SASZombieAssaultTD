using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SASZombieAssaultTD.Engine.VectorMath;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Rendering
{
    ///<summary>
    ///Font cache for efficient font loading and management.
    ///Provides centralized font access with caching to improve performance.
    ///</summary>
    public static class FontCache
    {
        private static readonly ConcurrentDictionary<string, CachedFont> _fonts = new();
        private static readonly CachedFont _defaultFont = new CachedFont("Arial", 12);

        ///<summary>
        ///Gets a font by name, loading and caching it if necessary.
        ///</summary>
        ///<param name="fontName">Name of the font</param>
        ///<returns>Font instance</returns>
        public static CachedFont GetFont(string fontName)
        {
            if (string.IsNullOrEmpty(fontName))
                return _defaultFont;

            return _fonts.GetOrAdd(fontName, name => new CachedFont(name, 12));
        }

        ///<summary>
        ///Gets a font by name and size, loading and caching it if necessary.
        ///</summary>
        ///<param name="fontName">Name of the font</param>
        ///<param name="size">Font size</param>
        ///<returns>Font instance</returns>
        public static CachedFont GetFont(string fontName, float size)
        {
            if (string.IsNullOrEmpty(fontName))
                return new CachedFont("Arial", size);

            var key = $"{fontName}_{size}";
            return _fonts.GetOrAdd(key, name => new CachedFont(fontName, size));
        }

        ///<summary>
        ///Preloads a font into the cache.
        ///</summary>
        ///<param name="fontName">Name of the font to preload</param>
        public static void PreloadFont(string fontName)
        {
            GetFont(fontName);
        }

        ///<summary>
        ///Clears all cached fonts.
        ///</summary>
        public static void ClearCache()
        {
            _fonts.Clear();
        }
    }

    ///<summary>
    ///Basic font implementation for rendering text.
    ///</summary>
    public class CachedFont
    {
        public string Name { get; }
        public float Size { get; }

        public CachedFont(string name, float size)
        {
            Name = name ?? "Arial";
            Size = size;
        }

        ///<summary>
        ///Measures the size of a text string.
        ///</summary>
        ///<param name="text">Text to measure</param>
        ///<returns>Vector3 representing text dimensions</returns>
        public Vector3 MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Vector3.Zero;

            //Simple approximation: width = character count * size * 0.6
            var width = text.Length * Size * 0.6f;
            return new Vector3(width, Size * 1.2f, 0);
        }
    }
}

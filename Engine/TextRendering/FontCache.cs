// =====================================================================================================
//  FILE: FontCache.cs
//  PATH: Engine/Render/Text/FontCache.cs
//  SUBSYSTEM: Text Rendering (Deterministic Font Metadata Cache)
//
//  ROLE:
//      Provides deterministic font metadata lookup for UI and HUD components. This cache supplies
//      stable font descriptors (name + size) used to construct runtime System.Drawing.Font objects.
//
//  RESPONSIBILITIES:
//      - Maintain a deterministic mapping of font keys to font metadata.
//      - Provide safe, side‑effect‑free lookup via GetFont().
//      - Ensure all UI subsystems receive consistent font definitions.
//
//  NON-RESPONSIBILITIES:
//      - Loading OS fonts.
//      - Managing GPU font atlases.
//      - Performing text layout or glyph rasterization.
//      - Logging or diagnostics (resource modules must remain pure).
//
//  ARCHITECTURAL NOTES:
//      - This module is part of the Resource Management Framework.
//      - All font definitions must remain deterministic and stable across runs.
//      - Missing keys must return a safe fallback font descriptor.
// =====================================================================================================

using System.Collections.Generic;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.TextRendering
{
    internal static class FontCache
    {
        private static readonly Dictionary<string, CachedFont> _fonts = new();

        static FontCache()
        {
            // Deterministic default font set
            _fonts["title"] = new CachedFont("Arial", 14f);
            _fonts["default"] = new CachedFont("Arial", 12f);
            _fonts["small"] = new CachedFont("Arial", 10f);
            _fonts["icon"] = new CachedFont("Arial", 16f);
        }

        public static CachedFont GetFont(string key)
        {
            if (key == null)
                return new CachedFont("Arial", 12f);

            if (_fonts.TryGetValue(key, out var font))
                return font;

            // Deterministic fallback
            return new CachedFont("Arial", 12f);
        }
    }

    internal readonly struct CachedFont
    {
        public string Name { get; }
        public float Size { get; }

        public CachedFont(string name, float size)
        {
            Name = name;
            Size = size;
        }
    }
}

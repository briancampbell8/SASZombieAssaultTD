// ====================================================================================================
//  FILE: Font.cs
//  PATH: ./Engine/Render/Text/
//  MODULE: Render
//
//  ROLE:
//      Immutable engine-level font descriptor used by the Render/Text subsystem.
//      Provides deterministic idECSEntityCore for font resources (system or file-based).
//
//  RESPONSIBILITIES:
//      - Provide WithSize(), WithWeight(), WithStyle() modifiers.
//      - Provide deterministic equality and hashing for caching and lookup.
//      - Provide ToString() for debugging and diagnostics.
//      - Store font metadata (name, size, weight, style, file path).
//
//  NON-RESPONSIBILITIES:
//      - Performing rendering or GPU upload operations.
//      - Managing glyphs or font atlases.
//      - Interacting with System.Drawing or OS font APIs.
//
//  NOTES:
//      Modernized for deterministic Option‑B architecture.
//      Removed legacy System.Drawing.Font conversion.
//      Pure value type; safe for caching, UI layout, and Finalizer integration.
// ====================================================================================================

using System;
using static SASZombieAssaultTD.Engine.TextRendering.TextEnums;

namespace SASZombieAssaultTD.Engine.TextRendering
{
    /// <summary>
    /// Immutable engine-level font descriptor used by the Render/Text subsystem. This struct does NOT perform rendering
    /// and does NOT depend on System.Drawing. It is a pure value type describing font idECSEntityCore, size, weight,
    /// and style.
    /// </summary>
    public readonly struct Font : IEquatable<Font>
    {
        public readonly string Name;
        public readonly float Size;
        public readonly FontWeight Weight;
        public readonly FontStyle Style;

        /// <summary>
        /// Optional file path for file-based fonts. Null for system fonts.
        /// </summary>
        public readonly string? FilePath;

        /// <summary>
        /// True if this font refers to a system-installed font. False if this font refers to a file-based font.
        /// </summary>
        public readonly bool IsSystemFont;

        /// <summary>
        /// Deterministic resource identifier used for caching.
        /// </summary>
        public readonly string ResourceId;

        public string FamilyName => Name;
        public string DisplayName => $"{Name} {Size}pt {Weight} {Style}";

        // ---------------------------------------------------------------------
        // Constructors
        // ---------------------------------------------------------------------

        public Font(string name, float size)
            : this(name, size, FontWeight.Regular, FontStyle.Normal)
        {
        }

        public Font(string name, float size, FontWeight weight, FontStyle style)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = System.Math.Max(1f, size);
            Weight = weight;
            Style = style;

            IsSystemFont = true;
            FilePath = null;

            ResourceId = $"{Name}_{Size}_{Weight}_{Style}";
        }

        /// <summary>
        /// File-based font constructor.
        /// </summary>
        public Font(string filePath, float size, FontWeight weight, FontStyle style, bool isFileBased = true)
        {
            if (isFileBased)
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentNullException(nameof(filePath));

                FilePath = filePath;
                Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                IsSystemFont = false;
            }
            else
            {
                // Treat filePath as a system font name
                Name = filePath ?? throw new ArgumentNullException(nameof(filePath));
                FilePath = null;
                IsSystemFont = true;
            }

            Size = System.Math.Max(1f, size);
            Weight = weight;
            Style = style;

            ResourceId = $"{Name}_{Size}_{Weight}_{Style}";
        }

        public Font(Font other)
        {
            Name = other.Name;
            Size = other.Size;
            Weight = other.Weight;
            Style = other.Style;
            ResourceId = other.ResourceId;
            IsSystemFont = other.IsSystemFont;
            FilePath = other.FilePath;
        }

        // ---------------------------------------------------------------------
        // Modifiers
        // ---------------------------------------------------------------------

        public Font WithSize(float newSize) => new Font(Name, newSize, Weight, Style);

        public Font WithWeight(FontWeight newWeight) => new Font(Name, Size, newWeight, Style);

        public Font WithStyle(FontStyle newStyle) => new Font(Name, Size, Weight, newStyle);

        // ---------------------------------------------------------------------
        // Equality
        // ---------------------------------------------------------------------

        public bool Equals(Font other)
        {
            return Name == other.Name &&
                   System.Math.Abs(Size - other.Size) < 0.001f &&
                   Weight == other.Weight &&
                   Style == other.Style &&
                   IsSystemFont == other.IsSystemFont &&
                   FilePath == other.FilePath;
        }

        public override bool Equals(object obj) => obj is Font other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Name, Size, Weight, Style, IsSystemFont, FilePath);

        public static bool operator ==(Font left, Font right) => left.Equals(right);

        public static bool operator !=(Font left, Font right) => !left.Equals(right);

        // ---------------------------------------------------------------------
        // Debug
        // ---------------------------------------------------------------------

        public override string ToString() => $"{Name} {Size}pt {Weight} {Style}";
    }
}
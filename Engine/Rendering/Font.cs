using System;

namespace SASZombieAssaultTD.Engine.Rendering
{
    public sealed class Font : IEquatable<Font>
    {
        public readonly string Name;
        public readonly float Size;
        public readonly FontWeight Weight;
        public readonly FontStyle Style;
        public readonly string ResourceId;
        public readonly bool IsSystemFont;
        public readonly string? FilePath;
        
        public string FamilyName => Name;
        public string DisplayName => $"{Name} {Size}pt {Weight} {Style}";
        
        public Font(string name, float size) : this(name, size, FontWeight.Regular, FontStyle.Normal) { }
        
        public Font(string name, float size, FontWeight weight, FontStyle style)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = System.Math.Max(1f, size);
            Weight = weight;
            Style = style;
            ResourceId = $"{Name}_{Size}_{Weight}_{Style}";
            IsSystemFont = true;
            FilePath = null;
        }
        
        public Font(string filePath, float size, FontWeight weight, FontStyle style, bool isFileBased = true)
        {
            if (!isFileBased)
            {
                // This path is for system fonts with string name
                Name = filePath ?? throw new ArgumentNullException(nameof(filePath));
                IsSystemFont = true;
                FilePath = null;
            }
            else
            {
                // This path is for file-based fonts
                if (string.IsNullOrEmpty(filePath))
                    throw new ArgumentNullException(nameof(filePath));
                    
                FilePath = filePath;
                Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                IsSystemFont = false;
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

        public Font(CachedFont cachedFont) : this()
        {
        }

        public Font()
        {
        }

        public Font WithSize(float newSize) => new Font(Name, newSize, Weight, Style);
        public Font WithWeight(FontWeight newWeight) => new Font(Name, Size, newWeight, Style);
        public Font WithStyle(FontStyle newStyle) => new Font(Name, Size, Weight, newStyle);
        
        public bool Equals(Font other)
        {
            return Name == other.Name && 
                   System.Math.Abs(Size - other.Size) < 0.001f &&
                   Weight == other.Weight &&
                   Style == other.Style;
        }
        
        public override bool Equals(object obj) => obj is Font other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Name, Size, Weight, Style);
        
        public static bool operator ==(Font left, Font right) => left.Equals(right);
        public static bool operator !=(Font left, Font right) => !left.Equals(right);
        
        public override string ToString() => $"{Name} {Size}pt {Weight} {Style}";
    }
    
    public enum FontWeight
    {
        Thin, ExtraLight, Light, Regular, Medium, SemiBold, Bold, ExtraBold, Black
    }
    
    public enum FontStyle
    {
        Normal, Italic, Oblique
    }
}

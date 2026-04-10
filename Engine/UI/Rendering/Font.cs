using System.Drawing;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Font wrapper for UI text rendering.
    /// Provides font information for text drawing operations.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Gets the font family name.
        /// </summary>
        public string FamilyName { get; }

        /// <summary>
        /// Gets the font size in points.
        /// </summary>
        public float Size { get; }

        /// <summary>
        /// Gets the font style.
        /// </summary>
        public FontStyle Style { get; }

        /// <summary>
        /// Creates a font with specified family and size.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <param name="size">Font size</param>
        public Font(string familyName, float size)
        {
            FamilyName = familyName;
            Size = size;
            Style = FontStyle.Regular;
        }

        /// <summary>
        /// Creates a font with specified family, size, and style.
        /// </summary>
        /// <param name="familyName">Font family name</param>
        /// <param name="size">Font size</param>
        /// <param name="style">Font style</param>
        public Font(string familyName, float size, FontStyle style)
        {
            FamilyName = familyName;
            Size = size;
            Style = style;
        }

        /// <summary>
        /// Gets a default UI font.
        /// </summary>
        public static Font Default => new Font("Arial", 12f);

        /// <summary>
        /// Gets a small UI font.
        /// </summary>
        public static Font Small => new Font("Arial", 10f);

        /// <summary>
        /// Gets a large UI font.
        /// </summary>
        public static Font Large => new Font("Arial", 16f);
    }
}

/*
File:    Size.cs
Purpose: Size structure for UI element dimensions in SAS Zombie Assault TD.
Features: Width and height properties for UI element sizing.
Standards: XML documentation with detailed property descriptions and usage examples.
Integration: Core UI layout system for element sizing and positioning.
Performance: Lightweight struct with no allocations during normal operations.
*/

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Size structure for UI element dimensions.
    /// Represents width and height dimensions for UI elements.
    /// This struct provides a way to specify the size of UI elements
    /// in a consistent and efficient manner.
    /// </summary>
    /// <remarks>
    /// The Size structure is used throughout the UI system to define the
    /// dimensions of UI elements. It provides width and height properties
    /// for consistent sizing across the user interface.
    /// 
    /// Sizing Features:
    /// - Width and height properties for element dimensions
    /// - Consistent sizing across UI components
    /// - Integration with layout and positioning systems
    /// - Support for both absolute and relative sizing
    /// 
    /// Common Use Cases:
    /// - Defining UI element dimensions
    /// - Layout calculations and positioning
    /// - Size constraints and minimum/maximum sizes
    /// - Responsive design and scaling
    /// 
    /// Performance Considerations:
    /// - Struct type for stack allocation and value semantics
    /// - No heap allocations during normal usage
    /// - Copy semantics ensure independent size instances
    /// - Efficient mathematical operations for layout calculations
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create size for button
    /// var buttonSize = new Size(100, 30);
    /// 
    /// // Create square size
    /// var squareSize = new Size(50);
    /// 
    /// // Use in layout calculation
    /// var elementSize = new Size(width, height);
    /// element.Size = elementSize;
    /// </code>
    /// </example>
    public struct Size
    {
        /// <summary>
        /// Width of the size.
        /// Defines the horizontal dimension of the UI element.
        /// </summary>
        /// <remarks>
        /// The width represents the horizontal size of the UI element.
        /// Width values should typically be positive, representing the
        /// horizontal extent of the element from left to right.
        /// </remarks>
        public float Width;

        /// <summary>
        /// Height of the size.
        /// Defines the vertical dimension of the UI element.
        /// </summary>
        /// <remarks>
        /// The height represents the vertical size of the UI element.
        /// Height values should typically be positive, representing the
        /// vertical extent of the element from top to bottom.
        /// </remarks>
        public float Height;

        /// <summary>
        /// Creates a size with specified width and height.
        /// This constructor allows complete control over size dimensions.
        /// </summary>
        /// <param name="width">Width of the size</param>
        /// <param name="height">Height of the size</param>
        /// <remarks>
        /// This constructor provides maximum flexibility for size specification,
        /// allowing custom width and height values. Use this when you need
        /// precise control over element dimensions.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create custom size
        /// var customSize = new Size(200, 100);
        /// // Result: Width=200, Height=100
        /// </code>
        /// </example>
        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a square size with equal width and height.
        /// This constructor creates a square size where width equals height.
        /// </summary>
        /// <param name="size">Size for both width and height</param>
        /// <remarks>
        /// This constructor is useful for creating square elements or when
        /// you need equal dimensions for width and height. It's commonly used
        /// for icons, avatars, and other square UI elements.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create square size
        /// var squareSize = new Size(50);
        /// // Result: Width=50, Height=50
        /// </code>
        /// </example>
        public Size(float size)
        {
            Width = size;
            Height = size;
        }

        /// <summary>
        /// Gets an empty size.
        /// This static property provides a size with zero dimensions.
        /// </summary>
        /// <remarks>
        /// The Empty size is commonly used as a default value or
        /// when representing non-existent or zero-sized elements.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use empty size as default
        /// var defaultSize = Size.Empty;
        /// // Result: Width=0, Height=0
        /// </code>
        /// </example>
        public static Size Empty => new Size(0, 0);
    }
}

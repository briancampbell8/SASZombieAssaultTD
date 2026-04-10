/*
File:    Margin.cs
Purpose: Margin structure for UI element spacing in SAS Zombie Assault TD.
Features: Defines space around UI elements with constructor overloads.
Standards: XML documentation with detailed parameter descriptions and usage examples.
Integration: Core UI layout system for element positioning and spacing.
Performance: Lightweight struct with no allocations during normal operations.
*/

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Margin structure for UI element spacing.
    /// Defines the space around UI elements for layout purposes.
    /// This struct provides a way to specify padding around UI elements
    /// to control spacing and positioning within containers.
    /// </summary>
    /// <remarks>
    /// The Margin structure is used by UI layout systems to create space
    /// around elements, preventing visual clutter and improving readability.
    /// Margins are applied to all four sides of an element (left, right,
    /// top, bottom) and can be specified individually or uniformly.
    /// 
    /// Layout Integration:
    /// - Used by layout managers for element positioning
    /// - Applied during layout calculation and rendering
    /// - Affects the effective size and position of UI elements
    /// - Works with padding to control internal and external spacing
    /// 
    /// Common Use Cases:
    /// - Creating space between buttons in a toolbar
    /// - Adding breathing room around text blocks
    /// - Separating UI sections for visual hierarchy
    /// - Ensuring consistent spacing across the interface
    /// 
    /// Performance Considerations:
    /// - Struct type for stack allocation and value semantics
    /// - No heap allocations during normal usage
    /// - Copy semantics ensure independent margin instances
    /// - Static properties provide common margin values without allocation
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create uniform margin for all sides
    /// var buttonMargin = new Margin(8f);
    /// 
    /// // Create different horizontal and vertical margins
    /// var panelMargin = new Margin(16f, 8f); // 16px horizontal, 8px vertical
    /// 
    /// // Create custom margins for each side
    /// var customMargin = new Margin(4f, 8f, 12f, 6f); // left, top, right, bottom
    /// 
    /// // Use predefined margin values
    /// var smallMargin = Margin.Small; // 4px on all sides
    /// var largeMargin = Margin.Large; // 16px on all sides
    /// 
    /// // Apply margin in layout calculation
    /// var elementBounds = new Rect(
    ///     position.X + margin.Left,
    ///     position.Y + margin.Top,
    ///     width + margin.Left + margin.Right,
    ///     height + margin.Top + margin.Bottom
    /// );
    /// </code>
    /// </example>
    public struct Margin
    {
        /// <summary>
        /// Left margin spacing in pixels.
        /// Defines the horizontal space on the left side of the element.
        /// </summary>
        public float Left;

        /// <summary>
        /// Right margin spacing in pixels.
        /// Defines the horizontal space on the right side of the element.
        /// </summary>
        public float Right;

        /// <summary>
        /// Top margin spacing in pixels.
        /// Defines the vertical space above the element.
        /// </summary>
        public float Top;

        /// <summary>
        /// Bottom margin spacing in pixels.
        /// Defines the vertical space below the element.
        /// </summary>
        public float Bottom;

        /// <summary>
        /// Creates a margin with uniform spacing on all sides.
        /// This constructor sets all margin values to the same specified amount.
        /// </summary>
        /// <param name="uniform">Uniform margin value for all sides in pixels</param>
        /// <remarks>
        /// This constructor is useful when you want equal spacing on all sides
        /// of an element, which is common for creating consistent visual rhythm
        /// in the user interface.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create 10px margin on all sides
        /// var margin = new Margin(10f);
        /// // Result: Left=10, Right=10, Top=10, Bottom=10
        /// </code>
        /// </example>
        public Margin(float uniform)
        {
            Left = uniform;
            Right = uniform;
            Top = uniform;
            Bottom = uniform;
        }

        /// <summary>
        /// Creates a margin with different horizontal and vertical spacing.
        /// This constructor sets left and right to one value, and top and bottom to another.
        /// </summary>
        /// <param name="horizontal">Horizontal margin for left and right sides in pixels</param>
        /// <param name="vertical">Vertical margin for top and bottom sides in pixels</param>
        /// <remarks>
        /// This constructor is useful when you want different spacing for
        /// horizontal and vertical dimensions, which is common for elements
        /// that need more horizontal separation than vertical, or vice versa.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create 16px horizontal, 8px vertical margin
        /// var margin = new Margin(16f, 8f);
        /// // Result: Left=16, Right=16, Top=8, Bottom=8
        /// </code>
        /// </example>
        public Margin(float horizontal, float vertical)
        {
            Left = horizontal;
            Right = horizontal;
            Top = vertical;
            Bottom = vertical;
        }

        /// <summary>
        /// Creates a margin with custom spacing for each side.
        /// This constructor allows complete control over spacing on each side.
        /// </summary>
        /// <param name="left">Left margin in pixels</param>
        /// <param name="top">Top margin in pixels</param>
        /// <param name="right">Right margin in pixels</param>
        /// <param name="bottom">Bottom margin in pixels</param>
        /// <remarks>
        /// This constructor provides maximum flexibility for margin specification,
        /// allowing different spacing on each side of an element. This is useful
        /// for asymmetric layouts where different sides need different spacing.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create custom margins for each side
        /// var margin = new Margin(4f, 8f, 12f, 6f);
        /// // Result: Left=4, Top=8, Right=12, Bottom=6
        /// </code>
        /// </example>
        public Margin(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Gets a margin with zero spacing on all sides.
        /// This static property provides a margin with no spacing, useful as a
        /// default value or when no margin is desired.
        /// </summary>
        /// <remarks>
        /// The Zero margin is commonly used as the default margin value
        /// for UI elements that don't require spacing, or as a starting
        /// point for margin calculations.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use zero margin for compact layouts
        /// var compactMargin = Margin.Zero;
        /// // Result: Left=0, Right=0, Top=0, Bottom=0
        /// </code>
        /// </example>
        public static Margin Zero => new Margin(0f);

        /// <summary>
        /// Gets a margin with uniform spacing of 4 pixels.
        /// This static property provides a small margin for subtle spacing.
        /// </summary>
        /// <remarks>
        /// The Small margin is useful for creating subtle separation between
        /// closely related UI elements, such as buttons in a toolbar or
        /// items in a list.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use small margin for tight layouts
        /// var tightMargin = Margin.Small;
        /// // Result: Left=4, Right=4, Top=4, Bottom=4
        /// </code>
        /// </example>
        public static Margin Small => new Margin(4f);

        /// <summary>
        /// Gets a margin with uniform spacing of 8 pixels.
        /// This static property provides a medium margin for standard spacing.
        /// </summary>
        /// <remarks>
        /// The Medium margin is the standard spacing for most UI elements,
        /// providing good visual separation without taking up too much space.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use medium margin for standard layouts
        /// var standardMargin = Margin.Medium;
        /// // Result: Left=8, Right=8, Top=8, Bottom=8
        /// </code>
        /// </example>
        public static Margin Medium => new Margin(8f);

        /// <summary>
        /// Gets a margin with uniform spacing of 16 pixels.
        /// This static property provides a large margin for significant spacing.
        /// </summary>
        /// <remarks>
        /// The Large margin is useful for creating clear separation between
        /// major UI sections or when more breathing room is needed.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use large margin for section separation
        /// var sectionMargin = Margin.Large;
        /// // Result: Left=16, Right=16, Top=16, Bottom=16
        /// </code>
        /// </example>
        public static Margin Large => new Margin(16f);
    }
}

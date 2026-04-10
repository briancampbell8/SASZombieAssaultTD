/*
File:    VerticalAlignment.cs
Purpose: Vertical alignment enumeration for UI element positioning in SAS Zombie Assault TD.
Features: Defines how elements are aligned vertically within their containers.
Standards: XML documentation with detailed enum value descriptions and usage examples.
Integration: Core UI layout system for element positioning and alignment.
Performance: Lightweight enum with no allocations during normal operations.
*/

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Vertical alignment enumeration for UI element positioning.
    /// Defines how elements are aligned vertically within their containers.
    /// This enum is used by layout systems to determine the vertical position
    /// of UI elements relative to their container bounds.
    /// </summary>
    /// <remarks>
    /// Vertical alignment controls how UI elements are positioned vertically
    /// within their containers. This is essential for creating consistent
    /// and predictable layouts across different screen sizes and resolutions.
    /// 
    /// Layout Integration:
    /// - Used by layout managers for element positioning
    /// - Applied during layout calculation and rendering
    /// - Works with horizontal alignment for complete positioning control
    /// - Affects the vertical position and size of UI elements
    /// 
    /// Common Use Cases:
    /// - Aligning buttons to the bottom of a panel
    /// - Centering text labels vertically
    /// - Stretching elements to fill available vertical space
    /// - Creating consistent header and footer positioning
    /// 
    /// Container Behavior:
    /// - The container provides the reference bounds for alignment
    /// - Elements may be resized based on alignment requirements
    /// - Margins and padding are applied after alignment calculation
    /// - Alignment affects both position and potentially size of elements
    /// 
    /// Performance Considerations:
    /// - Enum type for efficient comparison and storage
    /// - No heap allocations during normal usage
    /// - Fast switch/case operations for layout calculations
    /// - Compile-time constants for performance optimization
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set vertical alignment for a button
    /// button.VerticalAlignment = VerticalAlignment.Bottom;
    /// 
    /// // Use in layout calculation
    /// var elementY = alignment switch
    /// {
    ///     VerticalAlignment.Top => container.Top + margin.Top,
    ///     VerticalAlignment.Center => container.Top + (container.Height - elementHeight) / 2,
    ///     VerticalAlignment.Bottom => container.Bottom - elementHeight - margin.Bottom,
    ///     VerticalAlignment.Stretch => container.Top + margin.Top
    /// };
    /// 
    /// // Apply stretch alignment
    /// if (alignment == VerticalAlignment.Stretch)
    /// {
    ///     element.Height = container.Height - margin.Top - margin.Bottom;
    /// }
    /// </code>
    /// </example>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Align to the top of the container.
        /// Elements are positioned at the top edge of their container,
        /// maintaining their original height unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// Top alignment is commonly used for:
        /// - Header elements and navigation bars
        /// - Menu items and toolbars
        /// - Elements that should appear at the top of a panel
        /// - Consistent top-edge positioning across containers
        /// 
        /// Layout Behavior:
        /// - Element's top edge aligns with container's top edge
        /// - Original height is preserved unless constrained
        /// - Margins are applied after alignment
        /// - No vertical stretching occurs
        /// </remarks>
        /// <example>
        /// <code>
        /// // Align button to top of panel
        /// button.VerticalAlignment = VerticalAlignment.Top;
        /// // Result: Button appears at top of container with original height
        /// </code>
        /// </example>
        Top,

        /// <summary>
        /// Align to the vertical center of the container.
        /// Elements are positioned centrally within their container,
        /// maintaining their original height unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// Center alignment is commonly used for:
        /// - Dialog titles and important labels
        /// - Modal dialogs and popup content
        /// - Elements that need visual emphasis
        /// - Balanced layouts with symmetrical spacing
        /// 
        /// Layout Behavior:
        /// - Element's center aligns with container's vertical center
        /// - Original height is preserved unless constrained
        /// - Equal spacing above and below the element
        /// - No vertical stretching occurs
        /// </remarks>
        /// <example>
        /// <code>
        /// // Center label vertically in panel
        /// label.VerticalAlignment = VerticalAlignment.Center;
        /// // Result: Label appears centered with equal top/bottom spacing
        /// </code>
        /// </example>
        Center,

        /// <summary>
        /// Align to the bottom of the container.
        /// Elements are positioned at the bottom edge of their container,
        /// maintaining their original height unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// Bottom alignment is commonly used for:
        /// - Status bars and information panels
        /// - Action buttons and confirmation dialogs
        /// - Elements that should appear at the bottom of a panel
        /// - Consistent bottom-edge positioning across containers
        /// 
        /// Layout Behavior:
        /// - Element's bottom edge aligns with container's bottom edge
        /// - Original height is preserved unless constrained
        /// - Margins are applied after alignment
        /// - No vertical stretching occurs
        /// </remarks>
        /// <example>
        /// <code>
        /// // Align button to bottom of panel
        /// button.VerticalAlignment = VerticalAlignment.Bottom;
        /// // Result: Button appears at bottom of container with original height
        /// </code>
        /// </example>
        Bottom,

        /// <summary>
        /// Stretch to fill the vertical space of the container.
        /// Elements are resized to fill the entire vertical space of their container,
        /// accounting for margins and padding.
        /// </summary>
        /// <remarks>
        /// Stretch alignment is commonly used for:
        /// - Background panels and containers
        /// - List boxes and scrollable content areas
        /// - Elements that should fill available vertical space
        /// - Responsive layouts that adapt to container size
        /// 
        /// Layout Behavior:
        /// - Element's height is set to fill container's vertical space
        /// - Top and bottom margins are respected
        /// - Element may be resized significantly
        /// - Useful for flexible and responsive layouts
        /// 
        /// Size Considerations:
        /// - Element's minimum and maximum size constraints are respected
        /// - Content may need to adapt to new dimensions
        /// - Aspect ratio is not preserved unless specified
        /// - Child elements may be affected by parent stretching
        /// </remarks>
        /// <example>
        /// <code>
        /// // Stretch panel to fill container height
        /// panel.VerticalAlignment = VerticalAlignment.Stretch;
        /// // Result: Panel height equals container height minus margins
        /// </code>
        /// </example>
        Stretch
    }
}

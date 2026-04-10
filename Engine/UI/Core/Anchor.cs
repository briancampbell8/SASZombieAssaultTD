/*
File:    Anchor.cs
Purpose: Anchor enumeration for UI element positioning in SAS Zombie Assault TD.
Features: Defines anchor points for UI elements relative to their containers.
Standards: XML documentation with detailed enum value descriptions and usage examples.
Integration: Core UI layout system for element positioning and anchoring.
Performance: Lightweight enum with no allocations during normal operations.
*/

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Anchor enumeration for UI element positioning.
    /// Defines the anchor point for UI elements relative to their container.
    /// This enum is used by layout systems to determine the reference point
    /// for positioning UI elements within their containers.
    /// </summary>
    /// <remarks>
    /// Anchor points define where UI elements are positioned relative to their
    /// container bounds. This provides a simple way to position elements at
    /// common locations without needing to specify exact coordinates.
    /// 
    /// Layout Integration:
    /// - Used by layout managers for element positioning
    /// - Applied during layout calculation and rendering
    /// - Works with margins and padding for precise positioning
    /// - Affects the position but not typically the size of UI elements
    /// 
    /// Common Use Cases:
    /// - Positioning buttons in corners of panels
    /// - Centering elements within containers
    /// - Creating consistent corner-based layouts
    /// - Simplifying common positioning patterns
    /// 
    /// Anchor Behavior:
    /// - The anchor point defines the reference position
    /// - Elements are positioned relative to this point
    /// - Margins and padding are applied after anchoring
    /// - Anchor affects position but may affect size in some layouts
    /// 
    /// Performance Considerations:
    /// - Enum type for efficient comparison and storage
    /// - No heap allocations during normal usage
    /// - Fast switch/case operations for layout calculations
    /// - Compile-time constants for performance optimization
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set anchor for a button
    /// button.Anchor = Anchor.TopRight;
    /// 
    /// // Use in layout calculation
    /// var elementPosition = anchor switch
    /// {
    ///     Anchor.TopLeft => new Vector2(container.Left + margin.Left, container.Top + margin.Top),
    ///     Anchor.TopRight => new Vector2(container.Right - elementWidth - margin.Right, container.Top + margin.Top),
    ///     Anchor.BottomLeft => new Vector2(container.Left + margin.Left, container.Bottom - elementHeight - margin.Bottom),
    ///     Anchor.BottomRight => new Vector2(container.Right - elementWidth - margin.Right, container.Bottom - elementHeight - margin.Bottom),
    ///     Anchor.Center => new Vector2(container.Left + (container.Width - elementWidth) / 2, container.Top + (container.Height - elementHeight) / 2)
    /// };
    /// </code>
    /// </example>
    public enum Anchor
    {
        /// <summary>
        /// Anchor to the top-left corner of the container.
        /// Elements are positioned at the top-left corner of their container,
        /// maintaining their original size unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// TopLeft anchoring is commonly used for:
        /// - Navigation elements and menu items
        /// - Elements that should appear in the top-left corner
        /// - Consistent corner-based positioning
        /// - Default anchoring for many UI frameworks
        /// 
        /// Layout Behavior:
        /// - Element's top-left corner aligns with container's top-left corner
        /// - Original size is preserved unless constrained
        /// - Margins are applied after anchoring
        /// - No resizing occurs unless specified
        /// </remarks>
        /// <example>
        /// <code>
        /// // Anchor button to top-left corner
        /// button.Anchor = Anchor.TopLeft;
        /// // Result: Button appears at top-left of container
        /// </code>
        /// </example>
        TopLeft,

        /// <summary>
        /// Anchor to the top-right corner of the container.
        /// Elements are positioned at the top-right corner of their container,
        /// maintaining their original size unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// TopRight anchoring is commonly used for:
        /// - Close buttons and action controls
        /// - Elements that should appear in the top-right corner
        /// - Secondary controls and status indicators
        /// - Consistent corner-based positioning
        /// 
        /// Layout Behavior:
        /// - Element's top-right corner aligns with container's top-right corner
        /// - Original size is preserved unless constrained
        /// - Margins are applied after anchoring
        /// - No resizing occurs unless specified
        /// </remarks>
        /// <example>
        /// <code>
        /// // Anchor close button to top-right corner
        /// closeButton.Anchor = Anchor.TopRight;
        /// // Result: Close button appears at top-right of container
        /// </code>
        /// </example>
        TopRight,

        /// <summary>
        /// Anchor to the bottom-left corner of the container.
        /// Elements are positioned at the bottom-left corner of their container,
        /// maintaining their original size unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// BottomLeft anchoring is commonly used for:
        /// - Status information and help buttons
        /// - Elements that should appear in the bottom-left corner
        /// - Secondary controls and navigation aids
        /// - Consistent corner-based positioning
        /// 
        /// Layout Behavior:
        /// - Element's bottom-left corner aligns with container's bottom-left corner
        /// - Original size is preserved unless constrained
        /// - Margins are applied after anchoring
        /// - No resizing occurs unless specified
        /// </remarks>
        /// <example>
        /// <code>
        /// // Anchor help button to bottom-left corner
        /// helpButton.Anchor = Anchor.BottomLeft;
        /// // Result: Help button appears at bottom-left of container
        /// </code>
        /// </example>
        BottomLeft,

        /// <summary>
        /// Anchor to the bottom-right corner of the container.
        /// Elements are positioned at the bottom-right corner of their container,
        /// maintaining their original size unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// BottomRight anchoring is commonly used for:
        /// - Action buttons and confirmation controls
        /// - Elements that should appear in the bottom-right corner
        /// - Primary controls and call-to-action elements
        /// - Consistent corner-based positioning
        /// 
        /// Layout Behavior:
        /// - Element's bottom-right corner aligns with container's bottom-right corner
        /// - Original size is preserved unless constrained
        /// - Margins are applied after anchoring
        /// - No resizing occurs unless specified
        /// </remarks>
        /// <example>
        /// <code>
        /// // Anchor OK button to bottom-right corner
        /// okButton.Anchor = Anchor.BottomRight;
        /// // Result: OK button appears at bottom-right of container
        /// </code>
        /// </example>
        BottomRight,

        /// <summary>
        /// Anchor to the center of the container.
        /// Elements are positioned centrally within their container,
        /// maintaining their original size unless other constraints apply.
        /// </summary>
        /// <remarks>
        /// Center anchoring is commonly used for:
        /// - Dialog content and modal elements
        /// - Elements that need visual emphasis
        /// - Balanced layouts with symmetrical positioning
        /// - Content that should be the focal point
        /// 
        /// Layout Behavior:
        /// - Element's center aligns with container's center
        /// - Original size is preserved unless constrained
        /// - Equal spacing on all sides of the element
        /// - No resizing occurs unless specified
        /// </remarks>
        /// <example>
        /// <code>
        /// // Center dialog in container
        /// dialog.Anchor = Anchor.Center;
        /// // Result: Dialog appears centered with equal spacing
        /// </code>
        /// </example>
        Center
    }
}

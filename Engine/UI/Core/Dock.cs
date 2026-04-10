/*
File:    Dock.cs
Purpose: Dock enumeration for UI element positioning within containers in SAS Zombie Assault TD.
Features: Defines how elements are docked to the edges of their container.
Standards: XML documentation with detailed enum value descriptions and usage examples.
Integration: Core UI layout system for element positioning and docking.
Performance: Lightweight enum with no allocations during normal operations.
*/

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Dock enumeration for UI element positioning within containers.
    /// Defines how elements are docked to the edges of their container.
    /// This enum is used by layout systems to determine how UI elements
    /// fill space within their containers.
    /// </summary>
    /// <remarks>
    /// Docking controls how UI elements fill space within their containers.
    /// Unlike anchoring which affects position, docking affects both position
    /// and size, allowing elements to fill entire edges or the container itself.
    /// 
    /// Layout Integration:
    /// - Used by layout managers for element sizing and positioning
    /// - Applied during layout calculation and rendering
    /// - Works with margins and padding for precise spacing
    /// - Affects both position and size of UI elements
    /// 
    /// Common Use Cases:
    /// - Creating toolbars and status bars
    /// - Building panel layouts with edge-based content
    /// - Implementing responsive designs
    /// - Maximizing content area utilization
    /// 
    /// Dock Behavior:
    /// - Docked elements fill the specified edge of the container
    /// - Elements may be resized significantly based on dock mode
    /// - Multiple docked elements share space along the same edge
    /// - Fill mode uses the entire container space
    /// 
    /// Performance Considerations:
    /// - Enum type for efficient comparison and storage
    /// - No heap allocations during normal usage
    /// - Fast switch/case operations for layout calculations
    /// - Compile-time constants for performance optimization
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set dock for a panel
    /// panel.Dock = Dock.Top;
    /// 
    /// // Use in layout calculation
    /// var elementBounds = dock switch
    /// {
    ///     Dock.Left => new Rect(container.Left + margin.Left, container.Top + margin.Top, 100, container.Height - margin.Top - margin.Bottom),
    ///     Dock.Right => new Rect(container.Right - 100 - margin.Right, container.Top + margin.Top, 100, container.Height - margin.Top - margin.Bottom),
    ///     Dock.Top => new Rect(container.Left + margin.Left, container.Top + margin.Top, container.Width - margin.Left - margin.Right, 50),
    ///     Dock.Bottom => new Rect(container.Left + margin.Left, container.Bottom - 50 - margin.Bottom, container.Width - margin.Left - margin.Right, 50),
    ///     Dock.Fill => new Rect(container.Left + margin.Left, container.Top + margin.Top, container.Width - margin.Left - margin.Right, container.Height - margin.Top - margin.Bottom)
    /// };
    /// </code>
    /// </example>
    public enum Dock
    {
        /// <summary>
        /// No docking - element uses explicit positioning.
        /// </summary>
        None = 0,

        /// <summary>
        /// Dock to the left edge of the container.
        /// Elements fill the left edge of their container, extending from top to bottom.
        /// </summary>
        /// <remarks>
        /// Left docking is commonly used for:
        /// - Navigation panels and sidebars
        /// - Tool menus and option panels
        /// - Hierarchical content organization
        /// - Vertical toolbars and controls
        /// 
        /// Layout Behavior:
        /// - Element fills the left edge from top to bottom
        /// - Width is typically fixed or constrained
        /// - Height equals container height minus margins
        /// - Multiple left-docked elements share horizontal space
        /// </remarks>
        /// <example>
        /// <code>
        /// // Dock navigation panel to left edge
        /// navPanel.Dock = Dock.Left;
        /// // Result: Panel fills left edge of container
        /// </code>
        /// </example>
        Left,

        /// <summary>
        /// Dock to the right edge of the container.
        /// Elements fill the right edge of their container, extending from top to bottom.
        /// </summary>
        /// <remarks>
        /// Right docking is commonly used for:
        /// - Information panels and status displays
        /// - Action buttons and control groups
        /// - Secondary content and details
        /// - Context-sensitive controls
        /// 
        /// Layout Behavior:
        /// - Element fills the right edge from top to bottom
        /// - Width is typically fixed or constrained
        /// - Height equals container height minus margins
        /// - Multiple right-docked elements share horizontal space
        /// </remarks>
        /// <example>
        /// <code>
        /// // Dock info panel to right edge
        /// infoPanel.Dock = Dock.Right;
        /// // Result: Panel fills right edge of container
        /// </code>
        /// </example>
        Right,

        /// <summary>
        /// Dock to the top edge of the container.
        /// Elements fill the top edge of their container, extending from left to right.
        /// </summary>
        /// <remarks>
        /// Top docking is commonly used for:
        /// - Toolbars and navigation bars
        /// - Header content and titles
        /// - Menu systems and controls
        /// - Status information and alerts
        /// 
        /// Layout Behavior:
        /// - Element fills the top edge from left to right
        /// - Height is typically fixed or constrained
        /// - Width equals container width minus margins
        /// - Multiple top-docked elements share vertical space
        /// </remarks>
        /// <example>
        /// <code>
        /// // Dock toolbar to top edge
        /// toolbar.Dock = Dock.Top;
        /// // Result: Toolbar fills top edge of container
        /// </code>
        /// </example>
        Top,

        /// <summary>
        /// Dock to the bottom edge of the container.
        /// Elements fill the bottom edge of their container, extending from left to right.
        /// </summary>
        /// <remarks>
        /// Bottom docking is commonly used for:
        /// - Status bars and information displays
        /// - Action button groups
        /// - Navigation controls and progress indicators
        /// - Footer content and metadata
        /// 
        /// Layout Behavior:
        /// - Element fills the bottom edge from left to right
        /// - Height is typically fixed or constrained
        /// - Width equals container width minus margins
        /// - Multiple bottom-docked elements share vertical space
        /// </remarks>
        /// <example>
        /// <code>
        /// // Dock status bar to bottom edge
        /// statusBar.Dock = Dock.Bottom;
        /// // Result: Status bar fills bottom edge of container
        /// </code>
        /// </example>
        Bottom,

        /// <summary>
        /// Fill the entire container space.
        /// Elements fill the entire available space of their container.
        /// </summary>
        /// <remarks>
        /// Fill docking is commonly used for:
        /// - Main content areas and workspaces
        /// - Background panels and containers
        /// - Responsive layouts that adapt to container size
        /// - Maximizing content utilization
        /// 
        /// Layout Behavior:
        /// - Element fills the entire container space
        /// - Size equals container size minus margins
        /// - Position is aligned with container edges
        /// - Typically used for primary content areas
        /// 
        /// Size Considerations:
        /// - Element may be resized significantly
        /// - Content should adapt to new dimensions
        /// - Minimum and maximum size constraints are respected
        /// - Child elements may be affected by parent filling
        /// </remarks>
        /// <example>
        /// <code>
        /// // Fill content area
        /// contentPanel.Dock = Dock.Fill;
        /// // Result: Panel fills entire container space
        /// </code>
        /// </example>
        Fill
    }
}

/*
File:    ButtonStyle.cs
Purpose: Button style enumeration for UI button appearance in SAS Zombie Assault TD.
Features: Defines visual style and behavior of buttons with comprehensive style options.
Standards: XML documentation with detailed enum value descriptions and usage examples.
Integration: Core UI styling system for button appearance and user interaction feedback.
Performance: Lightweight enum with no allocations during normal operations.
*/

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Button style enumeration for UI button appearance.
    /// Defines the visual style and behavior of buttons.
    /// This enum is used to control the visual appearance and semantic
    /// meaning of buttons in the user interface.
    /// </summary>
    /// <remarks>
    /// Button styles provide semantic meaning and visual consistency
    /// across the user interface. Each style represents a specific type
    /// of action or interaction pattern, helping users understand the
    /// purpose and importance of different buttons.
    /// 
    /// Visual Design:
    /// - Each style has distinct visual characteristics
    /// - Colors and styling reinforce semantic meaning
    /// - Consistent appearance across the application
    /// - Clear visual hierarchy and user guidance
    /// 
    /// Common Use Cases:
    /// - Primary actions and main functionality
    /// - Secondary and alternative actions
    /// - Destructive and dangerous operations
    /// - Navigation and linking behaviors
    /// 
    /// User Experience:
    /// - Styles provide immediate visual feedback
    /// - Consistent behavior reinforces user expectations
    /// - Semantic meaning improves usability
    /// - Visual hierarchy guides user attention
    /// 
    /// Performance Considerations:
    /// - Enum type for efficient comparison and storage
    /// - No heap allocations during normal usage
    /// - Fast switch/case operations for styling logic
    /// - Compile-time constants for performance optimization
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set button style based on action importance
    /// saveButton.Style = ButtonStyle.Primary;      // Main action
    /// cancelButton.Style = ButtonStyle.Secondary;  // Alternative action
    /// deleteButton.Style = ButtonStyle.Danger;      // Destructive action
    /// helpButton.Style = ButtonStyle.Link;         // Navigation action
    /// 
    /// // Use style in rendering logic
    /// var colors = button.Style switch
    /// {
    ///     ButtonStyle.Primary => new[] { Color.Blue, Color.DarkBlue },
    ///     ButtonStyle.Secondary => new[] { Color.Gray, Color.DarkGray },
    ///     ButtonStyle.Danger => new[] { Color.Red, Color.DarkRed },
    ///     ButtonStyle.Success => new[] { Color.Green, Color.DarkGreen },
    ///     _ => new[] { Color.LightGray, Color.Gray }
    /// };
    /// </code>
    /// </example>
    public enum ButtonStyle
    {
        /// <summary>
        /// Primary button style for main actions.
        /// Typically uses prominent colors and styling.
        /// This style is reserved for the most important actions in a context.
        /// </summary>
        /// <remarks>
        /// Primary buttons are the most visually prominent and should be used
        /// sparingly for the main action or call-to-action. They typically use
        /// the brand color or a strong, attention-grabbing color scheme.
        /// 
        /// Visual Characteristics:
        /// - Bold, prominent colors (often brand colors)
        /// - Strong visual emphasis and high contrast
        /// - Larger size or more prominent placement
        /// - Clear visual hierarchy above other elements
        /// 
        /// Usage Guidelines:
        /// - Use for the single most important action
        /// - Limit to one primary button per view or context
        /// - Avoid overuse to maintain visual impact
        /// - Ensure accessibility with sufficient contrast
        /// 
        /// Common Examples:
        /// - "Save" or "Submit" forms
        /// - "Buy" or "Purchase" actions
        /// - "Continue" or "Next" steps
        /// - Main call-to-action buttons
        /// </remarks>
        /// <example>
        /// <code>
        /// // Primary save button
        /// saveButton.Style = ButtonStyle.Primary;
        /// // Result: Prominent blue button, main action emphasis
        /// </code>
        /// </example>
        Primary,

        /// <summary>
        /// Secondary button style for alternative actions.
        /// Less prominent than primary style but still clearly interactive.
        /// This style is used for secondary actions that are important but not primary.
        /// </summary>
        /// <remarks>
        /// Secondary buttons are less prominent than primary buttons but still
        /// clearly indicate interactivity. They use more subdued colors and styling
        /// while maintaining good visibility and accessibility.
        /// 
        /// Visual Characteristics:
        /// - Muted colors (grays, light brand colors)
        /// - Moderate visual emphasis
        /// - Standard size and placement
        /// - Clear but less prominent than primary
        /// 
        /// Usage Guidelines:
        /// - Use for alternative actions to primary buttons
        /// - Multiple secondary buttons can coexist
        /// - Good for optional or less critical actions
        /// - Maintain accessibility with proper contrast
        /// 
        /// Common Examples:
        /// - "Cancel" or "Back" actions
        /// - "Skip" or "Later" options
        /// - "Optional" or "Advanced" settings
        /// - Alternative workflow choices
        /// </remarks>
        /// <example>
        /// <code>
        /// // Secondary cancel button
        /// cancelButton.Style = ButtonStyle.Secondary;
        /// // Result: Muted gray button, alternative action
        /// </code>
        /// </example>
        Secondary,

        /// <summary>
        /// Tertiary button style for minor actions.
        /// Minimal styling for less important actions.
        /// This style is used for actions of low importance or frequency.
        /// </summary>
        /// <remarks>
        /// Tertiary buttons have the most subtle visual appearance and are used
        /// for actions that are rarely used or of minimal importance. They provide
        /// interactivity without competing for attention.
        /// 
        /// Visual Characteristics:
        /// - Very subtle styling (light colors, minimal borders)
        /// - Low visual emphasis
        /// - May appear as text-only or minimal borders
        /// - Clear interactivity but minimal prominence
        /// 
        /// Usage Guidelines:
        /// - Use for rarely accessed actions
        /// - Good for utility or helper functions
        /// - Can appear multiple times without visual clutter
        /// - Ensure discoverability when needed
        /// 
        /// Common Examples:
        /// - "Help" or "Info" buttons
        /// - "Settings" or "Preferences" links
        /// - "Advanced" or "Details" options
        /// - Utility and helper functions
        /// </remarks>
        /// <example>
        /// <code>
        /// // Tertiary help button
        /// helpButton.Style = ButtonStyle.Tertiary;
        /// // Result: Minimal styling, low prominence
        /// </code>
        /// </example>
        Tertiary,

        /// <summary>
        /// Success button style for positive actions.
        /// Uses green or positive color scheme.
        /// This style reinforces successful or positive actions.
        /// </summary>
        /// <remarks>
        /// Success buttons use green coloration to reinforce positive actions
        /// and successful outcomes. They provide clear visual feedback for
        /// actions that are expected to have positive results.
        /// 
        /// Visual Characteristics:
        /// - Green color scheme (various shades)
        /// - Positive, encouraging visual appearance
        /// - Clear association with success and completion
        /// - Good contrast and accessibility
        /// 
        /// Usage Guidelines:
        /// - Use for actions with positive outcomes
        /// - Reinforce success and completion semantics
        /// - Avoid overuse to maintain impact
        /// - Ensure colorblind accessibility
        /// 
        /// Common Examples:
        /// - "Complete" or "Finish" actions
        /// - "Confirm" or "Accept" operations
        /// - "Save" or "Apply" changes
        /// - "Success" or "Done" acknowledgments
        /// </remarks>
        /// <example>
        /// <code>
        /// // Success complete button
        /// completeButton.Style = ButtonStyle.Success;
        /// // Result: Green button, positive action emphasis
        /// </code>
        /// </example>
        Success,

        /// <summary>
        /// Warning button style for caution actions.
        /// Uses yellow or warning color scheme.
        /// This style indicates actions that require caution or attention.
        /// </summary>
        /// <remarks>
        /// Warning buttons use yellow or orange coloration to indicate actions
        /// that require user attention or caution. They suggest potential
        /// consequences or the need for careful consideration.
        /// 
        /// Visual Characteristics:
        /// - Yellow/orange color scheme
        /// - Cautionary visual appearance
        /// - Clear association with warning and attention
        /// - Good visibility without being alarming
        /// 
        /// Usage Guidelines:
        /// - Use for actions with potential consequences
        /// - Indicate need for careful consideration
        /// - Avoid overuse to maintain impact
        /// - Ensure accessibility and contrast
        /// 
        /// Common Examples:
        /// - "Reset" or "Restore" actions
        /// - "Warning" or "Caution" acknowledgments
        /// - "Proceed anyway" confirmations
        /// - Actions with irreversible consequences
        /// </remarks>
        /// <example>
        /// <code>
        /// // Warning reset button
        /// resetButton.Style = ButtonStyle.Warning;
        /// // Result: Yellow button, caution emphasis
        /// </code>
        /// </example>
        Warning,

        /// <summary>
        /// Danger button style for destructive actions.
        /// Uses red or danger color scheme.
        /// This style indicates actions that are destructive or irreversible.
        /// </summary>
        /// <remarks>
        /// Danger buttons use red coloration to indicate destructive actions
        /// or operations with serious consequences. They provide strong visual
        /// warning about the potential impact of the action.
        /// 
        /// Visual Characteristics:
        /// - Red color scheme (various shades)
        /// - Alarming or attention-grabbing appearance
        /// - Clear association with danger and destruction
        /// - Strong visual warning to prevent accidental activation
        /// 
        /// Usage Guidelines:
        /// - Use only for truly destructive actions
        /// - Provide clear confirmation dialogs
        /// - Avoid overuse to maintain impact
        /// - Ensure accessibility and clear meaning
        /// 
        /// Common Examples:
        /// - "Delete" or "Remove" actions
        /// - "Discard" or "Cancel" changes
        /// - "Reset" or "Clear" operations
        /// - Irreversible or destructive actions
        /// </remarks>
        /// <example>
        /// <code>
        /// // Danger delete button
        /// deleteButton.Style = ButtonStyle.Danger;
        /// // Result: Red button, destructive action warning
        /// </code>
        /// </example>
        Danger,

        /// <summary>
        /// Link button style for navigation actions.
        /// Styled like a hyperlink.
        /// This style indicates navigation or linking behavior rather than actions.
        /// </summary>
        /// <remarks>
        /// Link buttons are styled to resemble hyperlinks, indicating navigation
        /// or linking behavior. They typically appear as underlined text with
        /// interactive styling rather than traditional button appearance.
        /// 
        /// Visual Characteristics:
        /// - Text-only or minimal button appearance
        /// - Underlined or colored text styling
        /// - Hyperlink-like visual behavior
        /// - Clear interactivity without button appearance
        /// 
        /// Usage Guidelines:
        /// - Use for navigation and linking actions
        /// - Maintain consistency with web hyperlink conventions
        /// - Ensure clear interactivity indicators
        /// - Good for secondary navigation options
        /// 
        /// Common Examples:
        /// - "Learn more" or "Details" links
        /// - Navigation to other pages or sections
        /// - External website links
        /// - Help and documentation links
        /// </remarks>
        /// <example>
        /// <code>
        /// // Link help button
        /// helpButton.Style = ButtonStyle.Link;
        /// // Result: Hyperlink-style, navigation emphasis
        /// </code>
        /// </example>
        Link,

        /// <summary>
        /// Outline button style with transparent background.
        /// Shows only border and text.
        /// This style provides a less prominent but still clearly interactive button.
        /// </summary>
        /// <remarks>
        /// Outline buttons have transparent backgrounds with visible borders,
        /// providing a less prominent but still clearly interactive appearance.
        /// They're useful when you want button functionality without solid fill.
        /// 
        /// Visual Characteristics:
        /// - Transparent background with visible border
        /// - Border color indicates button state and style
        /// - Text color matches border color
        /// - Clear interactivity with minimal visual weight
        /// 
        /// Usage Guidelines:
        /// - Use when solid buttons would be too prominent
        /// - Good for secondary or tertiary actions
        /// - Works well in tight spaces or crowded interfaces
        /// - Maintain accessibility with proper contrast
        /// 
        /// Common Examples:
        /// - "Add" or "New" actions in lists
        /// - Secondary actions in forms
        /// - Actions in toolbars or tight spaces
        /// - Less prominent interactive elements
        /// </remarks>
        /// <example>
        /// <code>
        /// // Outline add button
        /// addButton.Style = ButtonStyle.Outline;
        /// // Result: Transparent background with border, moderate prominence
        /// </code>
        /// </example>
        Outline,

        /// <summary>
        /// Ghost button style with semi-transparent appearance.
        /// Minimal visual impact.
        /// This style provides the most subtle button appearance with transparency.
        /// </summary>
        /// <remarks>
        /// Ghost buttons have semi-transparent backgrounds with minimal visual
        /// impact, providing the most subtle interactive appearance. They're
        /// useful when you need interactivity with minimal visual presence.
        /// 
        /// Visual Characteristics:
        /// - Semi-transparent background
        /// - Minimal borders or no borders
        /// - Subtle text coloring
        /// - Very low visual weight and prominence
        /// 
        /// Usage Guidelines:
        /// - Use for very subtle interactive elements
        /// - Good for overlay or context-sensitive actions
        /// - Works well on varied backgrounds due to transparency
        /// - Ensure visibility and accessibility
        /// 
        /// Common Examples:
        /// - Actions in overlays or modals
        /// - Context-sensitive toolbar actions
        /// - Minimal interactive elements
        /// - Actions that should not distract from content
        /// </remarks>
        /// <example>
        /// <code>
        /// // Ghost close button in overlay
        /// closeButton.Style = ButtonStyle.Ghost;
        /// // Result: Semi-transparent, minimal visual impact
        /// </code>
        /// </example>
        Ghost
    }
}

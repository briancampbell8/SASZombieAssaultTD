/*
File:    Shadow.cs
Purpose: Shadow structure for UI element visual effects in SAS Zombie Assault TD.
Features: Defines drop shadow properties for UI components with color, offset, and blur.
Standards: XML documentation with detailed parameter descriptions and usage examples.
Integration: Core UI styling system for visual effects and element enhancement.
Performance: Lightweight struct with no allocations during normal operations.
*/

using System.Drawing;
using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Shadow structure for UI element visual effects.
    /// Defines drop shadow properties for UI components.
    /// This struct provides a way to specify shadow appearance including
    /// color, offset, and blur for visual depth and emphasis.
    /// </summary>
    /// <remarks>
    /// The Shadow structure is used by UI rendering systems to create
    /// drop shadow effects that add visual depth and emphasis to UI elements.
    /// Shadows can enhance readability and create a sense of hierarchy.
    /// 
    /// Visual Effects:
    /// - Creates depth perception for UI elements
    /// - Enhances text readability on varied backgrounds
    /// - Provides visual separation between elements
    /// - Adds emphasis and importance to selected elements
    /// 
    /// Common Use Cases:
    /// - Text shadows for improved readability
    /// - Button shadows for depth and interactivity
    /// - Panel shadows for layering and hierarchy
    /// - Modal overlays for focus and attention
    /// 
    /// Performance Considerations:
    /// - Struct type for stack allocation and value semantics
    /// - No heap allocations during normal usage
    /// - Copy semantics ensure independent shadow instances
    /// - Static properties provide common shadow values without allocation
    /// 
    /// Rendering Considerations:
    /// - Shadow rendering may impact performance with complex blur effects
    /// - Alpha blending is typically used for realistic shadow appearance
    /// - Shadow offset should be consistent with light source direction
    /// - Blur radius affects both visual quality and rendering performance
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create custom shadow
    /// var buttonShadow = new Shadow(Color.FromArgb(128, 0, 0, 0), new Vector2(2, 2), 4f);
    /// 
    /// // Use predefined shadow
    /// var defaultShadow = Shadow.Default;
    /// 
    /// // Create subtle text shadow
    /// var textShadow = new Shadow(Color.FromArgb(64, 0, 0, 0), new Vector2(1, 1), 2f);
    /// 
    /// // Apply shadow to UI element
    /// button.Shadow = buttonShadow;
    /// textLabel.Shadow = textShadow;
    /// </code>
    /// </example>
    public struct Shadow
    {
        /// <summary>
        /// Shadow color with alpha channel for transparency.
        /// Defines the color and opacity of the shadow effect.
        /// </summary>
        /// <remarks>
        /// The shadow color should typically be dark with partial transparency
        /// to create realistic shadow effects. The alpha component controls
        /// the shadow's opacity, with lower values creating more subtle shadows.
        /// 
        /// Color Guidelines:
        /// - Use dark colors (black, dark gray) for realistic shadows
        /// - Alpha values between 64-192 typically work best
        /// - Consider background color when choosing shadow color
        /// - Match shadow color to UI theme and design language
        /// </remarks>
        /// <example>
        /// <code>
        /// // Semi-transparent black shadow
        /// shadow.Color = Color.FromArgb(128, 0, 0, 0);
        /// 
        /// // Dark gray shadow
        /// shadow.Color = Color.FromArgb(96, 64, 64, 64);
        /// 
        /// // Colored shadow for artistic effect
        /// shadow.Color = Color.FromArgb(64, 0, 0, 255);
        /// </code>
        /// </example>
        public Color Color;

        /// <summary>
        /// Shadow offset from the element.
        /// Defines the horizontal and vertical displacement of the shadow.
        /// Z component is ignored for 2D shadow effects.
        /// </summary>
        /// <remarks>
        /// The offset determines where the shadow appears relative to the element.
        /// Positive X values move the shadow right, positive Y values move it down.
        /// The offset should be consistent with the perceived light source direction.
        /// 
        /// Offset Guidelines:
        /// - Small offsets (1-2px) create subtle shadows
        /// - Medium offsets (3-5px) create noticeable depth
        /// - Large offsets (6px+) create dramatic elevation effects
        /// - Consistent offsets across elements create unified appearance
        /// </remarks>
        /// <example>
        /// <code>
        /// // Subtle down-right shadow
        /// shadow.Offset = new Vector3(1, 1, 0);
        /// 
        /// // Standard shadow
        /// shadow.Offset = new Vector3(2, 2, 0);
        /// 
        /// // Elevated element shadow
        /// shadow.Offset = new Vector3(4, 4, 0);
        /// 
        /// // Left-up shadow (light from bottom-right)
        /// shadow.Offset = new Vector3(-2, -2, 0);
        /// </code>
        /// </example>
        public Vector3 Offset;

        /// <summary>
        /// Shadow blur radius.
        /// Defines the softness/bluriness of the shadow edges.
        /// </summary>
        /// <remarks>
        /// The blur radius controls how soft the shadow edges appear.
        /// Higher values create softer, more diffuse shadows, while lower
        /// values create sharper, more defined shadows.
        /// 
        /// Blur Guidelines:
        /// - Zero blur creates sharp shadows (hard edges)
        /// - Low blur (1-3px) creates subtle softness
        /// - Medium blur (4-8px) creates natural shadows
        /// - High blur (9px+) creates dramatic soft effects
        /// - Larger blur values may impact rendering performance
        /// 
        /// Performance Note:
        /// Blur rendering can be computationally expensive, especially
        /// with large blur radii. Consider performance implications when
        /// using high blur values in real-time UI rendering.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Sharp shadow (no blur)
        /// shadow.Blur = 0f;
        /// 
        /// // Subtle blur
        /// shadow.Blur = 2f;
        /// 
        /// // Natural shadow blur
        /// shadow.Blur = 4f;
        /// 
        /// // Soft, diffuse shadow
        /// shadow.Blur = 8f;
        /// </code>
        /// </example>
        public float Blur;

        /// <summary>
        /// Creates a shadow with specified properties.
        /// This constructor allows complete control over shadow appearance.
        /// </summary>
        /// <param name="color">Shadow color including alpha channel for transparency</param>
        /// <param name="offset">Shadow offset from element (X, Y displacement, Z ignored)</param>
        /// <param name="blur">Blur radius for shadow softness</param>
        /// <remarks>
        /// This constructor provides maximum flexibility for shadow specification,
        /// allowing custom colors, offsets, and blur effects. Use this when you
        /// need precise control over shadow appearance.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create custom shadow
        /// var shadow = new Shadow(
        ///     Color.FromArgb(128, 0, 0, 0), // Semi-transparent black
        ///     new Vector2(2, 2),            // Down-right offset
        ///     4f                           // Medium blur
        /// );
        /// </code>
        /// </example>
        public Shadow(Color color, Vector3 offset, float blur)
        {
            Color = color;
            Offset = offset;
            Blur = blur;
        }

        /// <summary>
        /// Gets a shadow with no offset or blur.
        /// This static property provides a shadow with no visual effect,
        /// useful as a default value or when shadows are disabled.
        /// </summary>
        /// <remarks>
        /// The None shadow is commonly used as the default shadow value
        /// for UI elements that don't require shadow effects, or as a
        /// starting point for shadow configuration.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use no shadow for flat design
        /// var flatShadow = Shadow.None;
        /// // Result: Color=Transparent, Offset=(0,0), Blur=0
        /// </code>
        /// </example>
        public static Shadow None => new Shadow(Color.Transparent, Vector3.Zero, 0f);

        /// <summary>
        /// Gets a default drop shadow.
        /// This static property provides a standard shadow suitable
        /// for most UI elements with moderate depth and natural appearance.
        /// </summary>
        /// <remarks>
        /// The Default shadow is designed to work well for most UI elements,
        /// providing good visual depth without being overly dramatic. It uses
        /// a semi-transparent black color with moderate offset and blur.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Use default shadow for buttons
        /// button.Shadow = Shadow.Default;
        /// // Result: Color=128,0,0,0, Offset=(2,2), Blur=4
        /// </code>
        /// </example>
        public static Shadow Default => new Shadow(Color.FromArgb(128, 0, 0, 0), new Vector3(2, 2), 4f);
    }
}

/*
File:    Gradient.cs
Purpose: Gradient structure for UI element visual effects in SAS Zombie Assault TD.
Features: Defines color gradients for UI components with direction control.
Standards: XML documentation with detailed parameter descriptions and usage examples.
Integration: Core UI styling system for visual effects and element enhancement.
Performance: Lightweight struct with no allocations during normal operations.
*/

using SASZombieAssaultTD.Engine.VectorMath;

namespace SASZombieAssaultTD.Engine.UI.Core
{
    /// <summary>
    /// Gradient structure for UI element visual effects.
    /// Defines color gradients for UI components.
    /// This struct provides a way to specify smooth color transitions
    /// for creating visually appealing UI elements.
    /// </summary>
    /// <remarks>
    /// The Gradient structure is used by UI rendering systems to create
    /// smooth color transitions between two colors. Gradients can enhance
    /// visual appeal and create depth, emphasis, or artistic effects.
    /// 
    /// Visual Effects:
    /// - Creates smooth color transitions for visual appeal
    /// - Enhances button and panel backgrounds
    /// - Provides depth and dimension to flat elements
    /// - Enables artistic and modern UI designs
    /// 
    /// Common Use Cases:
    /// - Button backgrounds with gradient effects
    /// - Panel headers and section dividers
    /// - Progress bars and loading indicators
    /// - Modern UI design elements and accents
    /// 
    /// Performance Considerations:
    /// - Struct type for stack allocation and value semantics
    /// - No heap allocations during normal usage
    /// - Copy semantics ensure independent gradient instances
    /// - Static properties provide common gradient values without allocation
    /// 
    /// Rendering Considerations:
    /// - Gradient rendering may impact performance with complex calculations
    /// - Direction vector should be normalized for consistent results
    /// - Color interpolation affects visual quality and rendering performance
    /// - Consider using texture gradients for complex patterns
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create custom gradient
    /// var buttonGradient = new Gradient(Color.Blue, Color.DarkBlue, new Vector3(0, 1, 0));
    /// 
    /// // Use predefined gradients
    /// var horizontalGradient = Gradient.Horizontal(Color.White, Color.LightGray);
    /// var verticalGradient = Gradient.Vertical(Color.Red, Color.DarkRed);
    /// 
    /// // Create diagonal gradient
    /// var diagonalGradient = Gradient.Diagonal(Color.Yellow, Color.Orange);
    /// 
    /// // Apply gradient to UI element
    /// button.BackgroundGradient = buttonGradient;
    /// panel.HeaderGradient = horizontalGradient;
    /// </code>
    /// </example>
    public struct Gradient
    {
        /// <summary>
        /// Starting color of the gradient.
        /// Defines the color at the beginning of the gradient transition.
        /// </summary>
        /// <remarks>
        /// The start color appears at the origin point of the gradient direction.
        /// For horizontal gradients, this is the left side. For vertical gradients,
        /// this is the top. The color should be chosen to complement the end color.
        /// 
        /// Color Guidelines:
        /// - Use colors that create pleasing transitions
        /// - Consider contrast and readability for text overlays
        /// - Match gradient colors to UI theme and design language
        /// - Ensure colors work well together in the gradient context
        /// </remarks>
        /// <example>
        /// <code>
        /// // Light to dark gradient
        /// gradient.StartColor = Color.LightBlue;
        /// gradient.EndColor = Color.DarkBlue;
        /// 
        /// // Warm color gradient
        /// gradient.StartColor = Color.Yellow;
        /// gradient.EndColor = Color.Red;
        /// 
        /// // Subtle gradient
        /// gradient.StartColor = Color.White;
        /// gradient.EndColor = Color.LightGray;
        /// </code>
        /// </example>
        public Color StartColor;

        /// <summary>
        /// Ending color of the gradient.
        /// Defines the color at the end of the gradient transition.
        /// </summary>
        /// <remarks>
        /// The end color appears at the terminal point of the gradient direction.
        /// For horizontal gradients, this is the right side. For vertical gradients,
        /// this is the bottom. The color should create a smooth transition from the start color.
        /// 
        /// Color Guidelines:
        /// - Ensure smooth color transition from start color
        /// - Consider accessibility and contrast requirements
        /// - Use colors that maintain visual hierarchy
        /// - Test gradient appearance at different sizes and scales
        /// </remarks>
        /// <example>
        /// <code>
        /// // Blue gradient
        /// gradient.StartColor = Color.LightBlue;
        /// gradient.EndColor = Color.DarkBlue;
        /// 
        /// // Green gradient
        /// gradient.StartColor = Color.LightGreen;
        /// gradient.EndColor = Color.DarkGreen;
        /// 
        /// // Monochrome gradient
        /// gradient.StartColor = Color.White;
        /// gradient.EndColor = Color.Black;
        /// </code>
        /// </example>
        public Color EndColor;

        /// <summary>
        /// Gradient direction vector.
        /// Normalized vector indicating gradient flow direction.
        /// Z component is ignored for 2D gradient effects.
        /// </summary>
        /// <remarks>
        /// The direction vector determines how the gradient flows across the element.
        /// The vector should be normalized for consistent gradient behavior.
        /// Common directions include horizontal (1,0), vertical (0,1), and diagonal.
        /// 
        /// Direction Guidelines:
        /// - Use normalized vectors for consistent gradient behavior
        /// - (1,0) for left-to-right horizontal gradients
        /// - (0,1) for top-to-bottom vertical gradients
        /// - (1,1) for diagonal top-left to bottom-right gradients
        /// - Consider visual flow and user interface design patterns
        /// </remarks>
        /// <example>
        /// <code>
        /// // Horizontal gradient (left to right)
        /// gradient.Direction = new Vector3(1, 0, 0);
        /// 
        /// // Vertical gradient (top to bottom)
        /// gradient.Direction = new Vector3(0, 1, 0);
        /// 
        /// // Diagonal gradient
        /// gradient.Direction = new Vector3(1, 1, 0).Normalized();
        /// 
        /// // Reverse horizontal (right to left)
        /// gradient.Direction = new Vector3(-1, 0, 0);
        /// </code>
        /// </example>
        public Vector3 Direction;

        /// <summary>
        /// Creates a gradient with specified colors and direction.
        /// This constructor allows complete control over gradient appearance.
        /// </summary>
        /// <param name="startColor">Starting color of the gradient</param>
        /// <param name="endColor">Ending color of the gradient</param>
        /// <param name="direction">Gradient direction (should be normalized, Z component ignored)</param>
        /// <remarks>
        /// This constructor provides maximum flexibility for gradient specification,
        /// allowing custom colors and directions. Use this when you need precise
        /// control over gradient appearance and behavior.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create custom diagonal gradient
        /// var gradient = new Gradient(
        ///     Color.Blue,                    // Start color
        ///     Color.DarkBlue,                // End color
        ///     new Vector3(1, 1).Normalized() // Diagonal direction
        /// );
        /// </code>
        /// </example>
        public Gradient(Color startColor, Color endColor, Vector3 direction)
        {
            StartColor = startColor;
            EndColor = endColor;
            Direction = direction;
        }

        /// <summary>
        /// Creates a horizontal gradient.
        /// This static method creates a gradient that flows from left to right.
        /// </summary>
        /// <param name="startColor">Starting color (left side)</param>
        /// <param name="endColor">Ending color (right side)</param>
        /// <returns>Horizontal gradient with left-to-right flow</returns>
        /// <remarks>
        /// Horizontal gradients are commonly used for:
        /// - Button backgrounds and interactive elements
        /// - Progress bars and loading indicators
        /// - Section headers and dividers
        /// - Modern UI design patterns
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create horizontal blue gradient
        /// var blueGradient = Gradient.Horizontal(Color.LightBlue, Color.DarkBlue);
        /// 
        /// // Create horizontal gray gradient
        /// var grayGradient = Gradient.Horizontal(Color.White, Color.Gray);
        /// </code>
        /// </example>
        public static Gradient Horizontal(Color startColor, Color endColor)
        {
            return new Gradient(startColor, endColor, new Vector3(1, 0, 0));
        }

        /// <summary>
        /// Creates a vertical gradient.
        /// This static method creates a gradient that flows from top to bottom.
        /// </summary>
        /// <param name="startColor">Starting color (top side)</param>
        /// <param name="endColor">Ending color (bottom side)</param>
        /// <returns>Vertical gradient with top-to-bottom flow</returns>
        /// <remarks>
        /// Vertical gradients are commonly used for:
        /// - Panel backgrounds and containers
        /// - Header sections and title bars
        /// - Status indicators and information displays
        /// - Depth and elevation effects
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create vertical green gradient
        /// var greenGradient = Gradient.Vertical(Color.LightGreen, Color.DarkGreen);
        /// 
        /// // Create vertical red gradient
        /// var redGradient = Gradient.Vertical(Color.LightRed, Color.DarkRed);
        /// </code>
        /// </example>
        public static Gradient Vertical(Color startColor, Color endColor)
        {
            return new Gradient(startColor, endColor, new Vector3(0, 1, 0));
        }

        /// <summary>
        /// Creates a diagonal gradient from top-left to bottom-right.
        /// This static method creates a gradient that flows diagonally.
        /// </summary>
        /// <param name="startColor">Starting color (top-left corner)</param>
        /// <param name="endColor">Ending color (bottom-right corner)</param>
        /// <returns>Diagonal gradient with top-left to bottom-right flow</returns>
        /// <remarks>
        /// Diagonal gradients are commonly used for:
        /// - Artistic UI elements and accents
        /// - Modern design patterns and visual interest
        /// - Dynamic and energetic interface elements
        /// - Creative backgrounds and overlays
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create diagonal orange gradient
        /// var orangeGradient = Gradient.Diagonal(Color.Yellow, Color.Orange);
        /// 
        /// // Create diagonal purple gradient
        /// var purpleGradient = Gradient.Diagonal(Color.LightPurple, Color.DarkPurple);
        /// </code>
        /// </example>
        public static Gradient Diagonal(Color startColor, Color endColor)
        {
            return new Gradient(startColor, endColor, new Vector3(1, 1, 0));
        }

        /// <summary>
        /// Gets a solid gradient (same start and end color).
        /// This static method creates a gradient with no color transition.
        /// </summary>
        /// <param name="color">Solid color for the gradient</param>
        /// <returns>Solid gradient with no color transition</returns>
        /// <remarks>
        /// Solid gradients are useful for:
        /// - Consistent color backgrounds
        /// - Fallback when gradients are not desired
        /// - Uniform color fills with gradient compatibility
        /// - Simplifying gradient logic when needed
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create solid blue gradient
        /// var solidBlue = Gradient.Solid(Color.Blue);
        /// 
        /// // Create solid white gradient
        /// var solidWhite = Gradient.Solid(Color.White);
        /// </code>
        /// </example>
        public static Gradient Solid(Color color)
        {
            return new Gradient(color, color, Vector3.Zero);
        }
    }
}

/*
File:    IRenderContext.cs
Purpose: Render context interface for UI rendering operations in SAS Zombie Assault TD.
Features: Basic drawing primitives for UI components with comprehensive documentation.
Standards: XML documentation with detailed parameter descriptions and usage examples.
Integration: Core UI rendering system for all UI components.
Performance: Optimized for frequent UI rendering operations.
*/

using System.Drawing;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Render context interface for UI rendering operations.
    /// Provides basic drawing primitives for UI components.
    /// This interface defines the contract for UI rendering systems,
    /// enabling consistent drawing operations across different rendering backends.
    /// </summary>
    /// <remarks>
    /// The IRenderContext interface provides the fundamental drawing operations
    /// required for UI component rendering. It abstracts the underlying rendering
    /// implementation while providing a consistent API for UI components.
    /// 
    /// Rendering Operations:
    /// - Rectangle drawing for backgrounds, borders, and UI elements
    /// - Text rendering for labels, buttons, and content display
    /// - Image/texture rendering for icons, backgrounds, and visual elements
    /// 
    /// Implementation Considerations:
    /// - Thread safety: Implementations should handle concurrent access
    /// - Performance: Operations should be optimized for frequent calls
    /// - Coordinate system: Uses screen coordinates with origin at top-left
    /// - Color handling: Supports alpha blending and transparency
    /// 
    /// Usage Pattern:
    /// UI components receive an IRenderContext instance during rendering
    /// and use it to draw their visual representation. The context is
    /// typically provided by the UI system or rendering manager.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Example implementation usage
    /// public class ButtonRenderer : UIComponent
    /// {
    ///     public override void Render(IRenderContext context)
    ///     {
    ///         // Draw button background
    ///         context.DrawRectangle(bounds, backgroundColor);
    ///         
    ///         // Draw button text
    ///         context.DrawText(text, textPosition, textColor, font);
    ///         
    ///         // Draw button icon
    ///         if (icon != null)
    ///         {
    ///             context.DrawImage(icon, iconRect);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IRenderContext
    {
        object Viewport { get; set; }

        /// <summary>
        /// Draws a filled rectangle with the specified color.
        /// This method renders a solid rectangle at the specified position
        /// with the given dimensions and fill color.
        /// </summary>
        /// <param name="rect">Rectangle bounds and position in screen coordinates</param>
        /// <param name="color">Fill color including alpha channel for transparency</param>
        /// <remarks>
        /// The rectangle is drawn filled with the specified color. The color's
        /// alpha component determines transparency. The rectangle coordinates
        /// are interpreted as screen coordinates with the origin at the top-left.
        /// 
        /// Performance Notes:
        /// - This is a fundamental operation called frequently during UI rendering
        /// - Implementations should batch rectangle draws when possible
        /// - Consider using texture atlases for complex rectangle patterns
        /// </remarks>
        /// <example>
        /// <code>
        /// // Draw a blue button background
        /// var buttonRect = new Rect(10, 10, 100, 30);
        /// context.DrawRectangle(buttonRect, Color.Blue);
        /// 
        /// // Draw a semi-transparent overlay
        /// var overlayRect = new Rect(0, 0, 800, 600);
        /// context.DrawRectangle(overlayRect, Color.FromArgb(128, 0, 0, 0));
        /// </code>
        /// </example>
        void DrawRectangle(Rect rect, Color color);

        /// <summary>
        /// Draws text at the specified position.
        /// This method renders text using the provided font and color at the
        /// specified screen position. Text rendering supports font metrics and
        /// proper character spacing.
        /// </summary>
        /// <param name="text">Text content to draw; cannot be null or empty</param>
        /// <param name="position">Screen position for text (top-left of text baseline, Z component ignored)</param>
        /// <param name="color">Text color including alpha channel for transparency</param>
        /// <param name="font">Font to use for text rendering; cannot be null</param>
        /// <remarks>
        /// Text rendering uses the specified font's metrics for proper character
        /// spacing and alignment. The position parameter specifies where the text
        /// should be drawn, typically the top-left corner of the text bounds.
        /// 
        /// Rendering Considerations:
        /// - Text rendering can be expensive; consider batching text operations
        /// - Font caching is recommended for frequently used fonts
        /// - Unicode and multi-byte character support is expected
        /// - Text color alpha affects text transparency
        /// 
        /// Performance Notes:
        /// - Complex fonts and large text blocks can impact performance
        /// - Consider using text atlases for static text content
        /// - Font loading should be cached and reused
        /// </remarks>
        /// <example>
        /// <code>
        /// // Draw button label
        /// var buttonFont = Font.Default;
        /// var textPosition = new Vector3(50, 20, 0);
        /// context.DrawText("Click Me", textPosition, Color.White, buttonFont);
        /// 
        /// // Draw disabled text with transparency
        /// context.DrawText("Disabled", textPosition, Color.FromArgb(128, 128, 128, 128), buttonFont);
        /// </code>
        /// </example>
        void DrawText(string text, Vector3 position, Color color, Font font);

        /// <summary>
        /// Draws an image/texture at the specified rectangle.
        /// This method renders a texture stretched to fit the destination
        /// rectangle, supporting various image formats and transparency.
        /// </summary>
        /// <param name="texture">Texture to render; cannot be null</param>
        /// <param name="rect">Destination rectangle for texture rendering</param>
        /// <remarks>
        /// The texture is drawn stretched to fit the destination rectangle.
        /// Texture rendering supports alpha channels for transparency and
        /// various image formats depending on the implementation.
        /// 
        /// Rendering Considerations:
        /// - Texture filtering affects quality vs performance
        /// - Alpha blending supports transparent textures
        /// - Texture coordinates typically use (0,0) to (1,1) for full texture
        /// - Consider texture atlases for multiple small images
        /// 
        /// Performance Notes:
        /// - Texture changes can be expensive; batch by texture when possible
        /// - Large textures impact memory usage and rendering performance
        /// - Consider mipmapping for scaled texture rendering
        /// - Texture loading should be done asynchronously when possible
        /// </remarks>
        /// <example>
        /// <code>
        /// // Draw button icon
        /// var iconTexture = Texture.Load("button_icon.png");
        /// var iconRect = new Rect(5, 5, 20, 20);
        /// context.DrawImage(iconTexture, iconRect);
        /// 
        /// // Draw background image stretched to full screen
        /// var backgroundTexture = Texture.Load("background.jpg");
        /// var screenRect = new Rect(0, 0, 800, 600);
        /// context.DrawImage(backgroundTexture, screenRect);
        /// </code>
        /// </example>
        void DrawImage(Texture texture, Rect rect);
        void Present();
    }
}

/*
File:    ITexture2D.cs
Purpose: Texture interface for UI rendering in SAS Zombie Assault TD.
Features: Texture properties, dimensions, and resource management.
Standards: XML documentation with detailed method descriptions and usage examples.
Integration: Core UI rendering system for texture management.
Performance: Optimized for texture access and resource management.
*/

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Interface for 2D texture resources used in UI rendering.
    /// Provides basic texture properties and resource management capabilities.
    /// This interface abstracts the underlying texture implementation for UI elements.
    /// </summary>
    /// <remarks>
    /// The ITexture2D interface provides a common abstraction for texture resources
    /// used in UI rendering. It allows different texture implementations to be
    /// used interchangeably while providing consistent access to texture properties.
    /// 
    /// Texture Properties:
    /// - Width and height dimensions
    /// - Texture format and pixel data
    /// - Resource management and cleanup
    /// - Texture binding and usage
    /// 
    /// Usage Patterns:
    /// - UI element backgrounds and images
    /// - Sprite sheets and texture atlases
    /// - Font glyph textures
    /// - Icon and button textures
    /// </remarks>
    public interface ITexture2D
    {
        /// <summary>
        /// Gets the texture width in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the texture height in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the texture name or identifier.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Binds the texture for rendering.
        /// </summary>
        void Bind();

        /// <summary>
        /// Releases texture resources.
        /// </summary>
        void Dispose();
    }
}

/*
File:    IRenderTarget.cs
Purpose: Render target interface for UI rendering in SAS Zombie Assault TD.
Features: Off-screen rendering, multiple render targets, and layer management.
Standards: XML documentation with detailed method descriptions and usage examples.
Integration: Core UI rendering system for advanced rendering techniques.
Performance: Optimized for render target switching and layer management.
*/

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    /// <summary>
    /// Interface for render targets used in UI rendering.
    /// Provides off-screen rendering capabilities for UI layers and effects.
    /// This interface enables advanced rendering techniques like layering and post-processing.
    /// </summary>
    /// <remarks>
    /// The IRenderTarget interface provides a way to render UI elements to
    /// off-screen surfaces, enabling advanced rendering techniques like
    /// layering, post-processing effects, and complex UI compositions.
    /// 
    /// Render Target Features:
    /// - Off-screen rendering capabilities
    /// - Multiple render target support
    /// - Layer management and composition
    /// - Post-processing effects integration
    /// 
    /// Usage Patterns:
    /// - UI layer separation and composition
    /// - Post-processing effects application
    /// - Complex UI rendering pipelines
    /// - Multi-pass rendering techniques
    /// </remarks>
    public interface IRenderTarget
    {
        /// <summary>
        /// Gets the render target width.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the render target height.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the render target name or identifier.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Binds the render target for rendering.
        /// </summary>
        void Bind();

        /// <summary>
        /// Unbinds the render target.
        /// </summary>
        void Unbind();

        /// <summary>
        /// Clears the render target.
        /// </summary>
        void Clear();

        /// <summary>
        /// Releases render target resources.
        /// </summary>
        void Dispose();
    }
}

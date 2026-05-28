/*
File:    SpriteComponent.cs
Path:    Engine/Components/SpriteComponent.cs
Purpose:   P11-03-02-A - Core ECS component for sprite rendering properties.
           Stores texture asset ID, source rectangle, color tint, layer depth, and visibility.

Role:      Essential rendering component for entities requiring visual representation.
           - Stores texture asset reference for RSManager lookup
           - Manages source rectangle for sprite sheet animation
           - Handles color tint for visual effects and team colors
           - Controls layer depth for proper rendering order
           - Provides visibility flag for show/hide functionality
           - Supports rotation and scaling through transform integration

Features:   ECS-friendly pure data structure optimized for rendering system.
           Thread-safe property access for concurrent rendering operations.
           Supports sprite sheet animations through source rectangle updates.
           Provides efficient batch rendering compatibility.
           Integrates seamlessly with transform component for positioning.

Notes:      This component is required by all entities that need visual representation.
           Texture asset IDs are resolved through the RSManager system.
           Layer depth values determine rendering order (lower values render first).
           Color tint supports alpha blending for transparency effects.

*/
using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.ECS;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Component for sprite rendering properties.
    /// P11-03-02-A: Stores texture/sprite asset ID, source rectangle, color tint, layer depth, and visibility flag.
    /// </summary>
    public class SpriteComponent : BaseComponent
    {
        ///  Properties

        /// <summary>
        /// Texture or sprite asset ID for RSManager lookup.
        /// </summary>
        public string AssetId { get; set; } = string.Empty;

        /// <summary>
        /// Sprite index for sprite sheet animations.
        /// </summary>
        public int SpriteIndex { get; set; }

        /// <summary>
        /// Transform offset for positioning.
        /// </summary>
        public Vector3 TransformOffset { get; set; } = new Vector3(0, 0, 0);

        /// <summary>
        /// Optional source rectangle within the texture (null = full texture).
        /// </summary>
        public Rectangle? SourceRectangle { get; set; } = null;

        /// <summary>
        /// Optional color tint applied to the sprite (white = no tint).
        /// </summary>
        public Color TintColor { get; set; } = Color.White;

        /// <summary>
        /// Color tint alias for compatibility.
        /// </summary>
        public Color ColorTint { get; set; } = Color.White;

        /// <summary>
        /// Animation time for sprite animations.
        /// </summary>
        public float AnimationTime { get; set; }

        /// <summary>
        /// Layer depth for rendering order (lower values = render first, behind).
        /// </summary>
        public float LayerDepth { get; set; } = 0.0f;

        /// <summary>
        /// Visibility flag - sprite is only rendered when true.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// 

        ///  Constructors

        /// <summary>
        /// Creates a new SpriteComponent with default values.
        /// </summary>
        public SpriteComponent() { }

        /// <summary>
        /// Creates a new SpriteComponent with specified asset ID.
        /// </summary>
        /// <param name="assetId">Texture or sprite asset ID</param>
        public SpriteComponent(string assetId)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
        }

        /// <summary>
        /// Creates a new SpriteComponent with full configuration.
        /// </summary>
        /// <param name="assetId">Texture or sprite asset ID</param>
        /// <param name="sourceRectangle">Optional source rectangle</param>
        /// <param name="tintColor">Optional color tint</param>
        /// <param name="layerDepth">Layer depth for ordering</param>
        /// <param name="isVisible">Initial visibility state</param>
        public SpriteComponent(
            string assetId,
            Rectangle? sourceRectangle = null,
            Color? tintColor = null,
            float layerDepth = 0.0f,
            bool isVisible = true)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
            SourceRectangle = sourceRectangle;
            TintColor = tintColor ?? Color.White;
            LayerDepth = layerDepth;
            IsVisible = isVisible;
        }

        public SpriteComponent(string assetId, object value, Color color, float v1, bool v2) : this(assetId)
        {
        }

        /// 

        ///  Methods

        /// <summary>
        /// Toggles the visibility of the sprite.
        /// </summary>
        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        /// <summary>
        /// Resets the sprite component to its default state.
        /// </summary>
        public void Reset()
        {
            AssetId = string.Empty;
            SourceRectangle = null;
            TintColor = Color.White;
            LayerDepth = 0.0f;
            IsVisible = true;
        }

        /// <summary>
        /// Gets a string representation for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"SpriteComponent(Asset: {AssetId}, Layer: {LayerDepth}, Visible: {IsVisible})";
        }

        /// 
    }
}





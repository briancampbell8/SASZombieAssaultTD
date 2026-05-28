/*
File:    UIComponent.cs
Purpose: Component for UI element properties.
Features: Text/sprite asset ID, screen-space position, color tint, scale, layer depth, visibility.

P11-03-05-A: Component stores all required UI element properties.
*/
using System;
using System.Drawing;

namespace SASZombieAssaultTD.Engine.Components
{
    /// <summary>
    /// Component for UI element properties.
    /// P11-03-05-A: Stores text/sprite asset ID, screen-space position, color tint, scale, layer depth, and visibility flag.
    /// </summary>
    public class UIComponent
    {
        ///  Properties

        /// <summary>
        /// Text or sprite asset ID for RSManager lookup.
        /// </summary>
        public string AssetId { get; set; } = string.Empty;

        /// <summary>
        /// Position in screen-space coordinates (pixels from top-left).
        /// </summary>
        public PointF Position { get; set; } = PointF.Empty;

        /// <summary>
        /// Color tint applied to UI element (white = no tint).
        /// </summary>
        public Color ColorTint { get; set; } = Color.White;

        /// <summary>
        /// Scale factor for UI element (1.0 = original size).
        /// </summary>
        public float Scale { get; set; } = 1.0f;

        /// <summary>
        /// Layer depth for UI rendering order (lower values = render first, behind).
        /// UI elements typically use higher values than world entities.
        /// </summary>
        public float LayerDepth { get; set; } = 1000.0f; // Default UI layer

        /// <summary>
        /// Visibility flag - UI element is only rendered when true.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Optional text content for text-based UI elements.
        /// </summary>
        public string? Text { get; set; } = null;

        /// <summary>
        /// Optional font for text rendering (null = use default font).
        /// </summary>
        public string? FontName { get; set; } = null;

        /// <summary>
        /// Optional font size for text rendering.
        /// </summary>
        public float FontSize { get; set; } = 12.0f;

        /// 

        ///  Constructors

        /// <summary>
        /// Creates a new UIComponent with default values.
        /// </summary>
        public UIComponent() { }

        /// <summary>
        /// Creates a new UIComponent with specified asset ID and position.
        /// </summary>
        /// <param name="assetId">Text or sprite asset ID</param>
        /// <param name="position">Screen-space position</param>
        public UIComponent(string assetId, PointF position)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
            Position = position;
        }

        /// <summary>
        /// Creates a new UIComponent with text content.
        /// </summary>
        /// <param name="text">Text content</param>
        /// <param name="position">Screen-space position</param>
        /// <param name="color">Text color</param>
        /// <param name="fontSize">Font size</param>
        public UIComponent(string text, PointF position, Color? color = null, float fontSize = 12.0f)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Position = position;
            ColorTint = color ?? Color.White;
            FontSize = fontSize;
        }

        /// <summary>
        /// Creates a new UIComponent with full configuration.
        /// </summary>
        /// <param name="assetId">Text or sprite asset ID</param>
        /// <param name="position">Screen-space position</param>
        /// <param name="color">Color tint</param>
        /// <param name="scale">Scale factor</param>
        /// <param name="layerDepth">Layer depth</param>
        /// <param name="isVisible">Initial visibility</param>
        public UIComponent(
            string assetId,
            PointF position,
            Color? color = null,
            float scale = 1.0f,
            float layerDepth = 1000.0f,
            bool isVisible = true)
        {
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
            Position = position;
            ColorTint = color ?? Color.White;
            Scale = scale;
            LayerDepth = layerDepth;
            IsVisible = isVisible;
        }

        /// <summary>
        /// Creates a new text-based UIComponent with full configuration.
        /// </summary>
        /// <param name="text">Text content</param>
        /// <param name="position">Screen-space position</param>
        /// <param name="fontName">Font name</param>
        /// <param name="fontSize">Font size</param>
        /// <param name="color">Text color</param>
        /// <param name="layerDepth">Layer depth</param>
        /// <param name="isVisible">Initial visibility</param>
        public UIComponent(
            string text,
            PointF position,
            string? fontName = null,
            float fontSize = 12.0f,
            Color? color = null,
            float layerDepth = 1000.0f,
            bool isVisible = true)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Position = position;
            FontName = fontName;
            FontSize = fontSize;
            ColorTint = color ?? Color.White;
            LayerDepth = layerDepth;
            IsVisible = isVisible;
        }

        /// 

        ///  Methods

        /// <summary>
        /// Toggles the visibility of the UI element.
        /// </summary>
        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        /// <summary>
        /// Resets the UI component to its default state.
        /// </summary>
        public void Reset()
        {
            AssetId = string.Empty;
            Position = PointF.Empty;
            ColorTint = Color.White;
            Scale = 1.0f;
            LayerDepth = 1000.0f;
            IsVisible = true;
            Text = null;
            FontName = null;
            FontSize = 12.0f;
        }

        /// <summary>
        /// Gets a string representation for debugging.
        /// </summary>
        public override string ToString()
        {
            return !string.IsNullOrEmpty(Text)
                ? $"UIComponent(Text: '{Text}', Pos: {Position}, Layer: {LayerDepth}, Visible: {IsVisible})"
                : $"UIComponent(Asset: {AssetId}, Pos: {Position}, Layer: {LayerDepth}, Visible: {IsVisible})";
        }

        /// 
    }
}





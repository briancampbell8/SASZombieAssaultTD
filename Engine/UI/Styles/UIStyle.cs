// ====================================================================================================
//  FILE: UIStyle.cs
//  PATH: ./Engine/UI/Styles/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide SetBackgroundColor() behavior for the UI subsystem.
//      - Provide SetTextColor() behavior for the UI subsystem.
//      - Provide SetBorder() behavior for the UI subsystem.
//      - Provide SetFont() behavior for the UI subsystem.
//      - Provide SetPadding() behavior for the UI subsystem.
//      - Provide SetPadding() behavior for the UI subsystem.
//      - Provide SetMargin() behavior for the UI subsystem.
//      - Provide SetMargin() behavior for the UI subsystem.
//      - Provide SetCornerRadius() behavior for the UI subsystem.
//      - Provide Copy() behavior for the UI subsystem.
//      - Provide Merge() behavior for the UI subsystem.
//      - Provide Reset() behavior for the UI subsystem.
//      - Provide ToString() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using static System.Math;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Styles
{
    /// <summary>
    /// Style properties such as colors, fonts, padding, and margins P80-06-01: UIStyle defining style properties such
    /// as colors, fonts, padding, and margins
    /// </summary>
    public class UIStyle
    {
        private System.Drawing.Color _backgroundColor = System.Drawing.Color.Transparent;
        private System.Drawing.Color _textColor = System.Drawing.Color.Black;
        private System.Drawing.Color _borderColor = System.Drawing.Color.Black;
        private string _font = string.Empty;
        private float _fontSize = 12.0f;
        private UI.Layout.UIPadding _padding;
        private UI.Layout.UIMargin _margin;
        private float _borderThickness = 0.0f;
        private float _cornerRadius = 0.0f;
        private bool _wordWrap = false;
        private System.Drawing.ContentAlignment _textAlignment = System.Drawing.ContentAlignment.TopLeft;
        private float Shadowoffset;
        public float Background;
        internal int UISystemBorderThickness;
        private System.Drawing.Color _shadowColor;
        internal Color HoverColor;
        internal Color PressedColor;
        internal Color ActiveColor;
        private object _shadowoffset;
        internal static System.Drawing.Color Color;
        internal System.Drawing.Color ForegroundColor;
        internal float Opacity;

        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        public System.Drawing.Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    DLogger.Log($"UIStyle: Background color set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text color
        /// </summary>
        public System.Drawing.Color TextColor
        {
            get => _textColor;
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    DLogger.Log($"UIStyle: Text color set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color
        /// </summary>
        public System.Drawing.Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    DLogger.Log($"UIStyle: Border color set to {value}");
                }
            }
        }

        public System.Drawing.Color ShadowColor
        {
            get => _shadowColor;
            set
            {
                if (_shadowColor != value)
                {
                    _shadowColor = value;
                    DLogger.Log($"UIStyle: Shadow color set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the border thickness
        /// </summary>
        public float BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (_borderThickness != value)
                {
                    _borderThickness = Max(0.0f, value);
                    DLogger.Log($"UIStyle: Border thickness set to {_borderThickness}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the corner radius
        /// </summary>
        public float ShadowOffset
        {
            get => ShadowOffset;
            set
            {
                if (Shadowoffset != value)
                {
                    ShadowOffset = Max(0.0f, value);
                    DLogger.Log($"UIStyle: Shadow offsets set to {_shadowoffset}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the font name
        /// </summary>
        public string FontName => _font;

        /// <summary>
        /// Gets or sets the font
        /// </summary>
        public string Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value ?? string.Empty;
                    DLogger.Log($"UIStyle: Font set to '{_font}'");
                }
            }
        }

        /// <summary>
        /// Gets or sets the font size
        /// </summary>
        public float FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = Max(1.0f, value);
                    DLogger.Log($"UIStyle: Font size set to {_fontSize}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the padding
        /// </summary>
        public UI.Layout.UIPadding Padding
        {
            get => _padding;
            set
            {
                _padding = value;
                DLogger.Log($"UIStyle: Padding set to {value}");
            }
        }

        /// <summary>
        /// Gets or sets the margin
        /// </summary>
        public UI.Layout.UIMargin Margin
        {
            get => _margin;
            set
            {
                _margin = value;
                DLogger.Log($"UIStyle: Margin set to {value}");
            }
        }

        /// <summary>
        /// Gets or sets the border thickness
        /// </summary>

        /// <summary>
        /// Gets or sets the corner radius
        /// </summary>
        public float CornerRadius
        {
            get => _cornerRadius;
            set
            {
                if (_cornerRadius != value)
                {
                    _cornerRadius = Max(0.0f, value);
                    DLogger.Log($"UIStyle: Corner radius set to {_cornerRadius}");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether text should wrap
        /// </summary>
        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                if (_wordWrap != value)
                {
                    _wordWrap = value;
                    DLogger.Log($"UIStyle: Word wrap set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text alignment
        /// </summary>
        public System.Drawing.ContentAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    DLogger.Log($"UIStyle: Text alignment set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets whether the style has a border
        /// </summary>
        public bool HasBorder => _borderThickness > 0;

        /// <summary>
        /// Gets whether the style has rounded corners
        /// </summary>
        public bool HasRoundedCorners => _cornerRadius > 0;

        /// <summary>
        /// Initializes a new UIStyle
        /// </summary>
        public UIStyle()
        {
            _padding = UI.Layout.UIPadding.Zero;
            _margin = UI.Layout.UIMargin.Zero;

            _backgroundColor = System.Drawing.Color.Transparent;
            _textColor = System.Drawing.Color.Black;
            _borderColor = System.Drawing.Color.Black;
            _font = string.Empty;
            _fontSize = 12.0f;
            _borderThickness = 0.0f;
            _cornerRadius = 0.0f;
            _wordWrap = false;
            _textAlignment = System.Drawing.ContentAlignment.TopLeft;

            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyle: Created new style");
        }

        /// <summary>
        /// Initializes a new UIStyle with background color
        /// </summary>
        /// <param name="backgroundColor">Background color</param>
        public UIStyle(System.Drawing.Color backgroundColor) : this()
        {
            BackgroundColor = backgroundColor;
            DLogger.Log($"UIStyle: Created style with background color {backgroundColor}");
        }

        /// <summary>
        /// Sets the background color
        /// </summary>
        /// <param name="color">Background color</param>
        public void SetBackgroundColor(System.Drawing.Color color)
        {
            BackgroundColor = color;
        }

        /// <summary>
        /// Sets the text color
        /// </summary>
        /// <param name="color">Text color</param>
        public void SetTextColor(System.Drawing.Color color)
        {
            TextColor = color;
        }

        /// <summary>
        /// Sets the border properties
        /// </summary>
        /// <param name="color">Border color</param>
        /// <param name="thickness">Border thickness</param>
        public void SetBorder(System.Drawing.Color color, float thickness)
        {
            BorderColor = color;
            BorderThickness = thickness;
        }

        /// <summary>
        /// Sets the font properties
        /// </summary>
        /// <param name="font">Font name</param>
        /// <param name="size">Font size</param>
        public void SetFont(string font, float size)
        {
            Font = font;
            FontSize = size;
        }

        /// <summary>
        /// Sets the padding
        /// </summary>
        /// <param name="padding">Padding values</param>
        public void SetPadding(UI.Layout.UIPadding padding)
        {
            Padding = padding;
        }

        /// <summary>
        /// Sets uniform padding
        /// </summary>
        /// <param name="padding">Padding value for all sides</param>
        public void SetPadding(float padding)
        {
            Padding = UI.Layout.UIPadding.Uniform(padding);
        }

        /// <summary>
        /// Sets the margin
        /// </summary>
        /// <param name="margin">Margin values</param>
        public void SetMargin(UI.Layout.UIMargin margin)
        {
            Margin = margin;
        }

        /// <summary>
        /// Sets uniform margin
        /// </summary>
        /// <param name="margin">Margin value for all sides</param>
        public void SetMargin(float margin)
        {
            Margin = UI.Layout.UIMargin.Uniform(margin);
        }

        /// <summary>
        /// Sets the corner radius
        /// </summary>
        /// <param name="radius">Corner radius</param>
        public void SetCornerRadius(float radius)
        {
            CornerRadius = radius;
        }

        /// <summary>
        /// Creates a copy of this style
        /// </summary>
        /// <returns>Copy of the style</returns>
        public UIStyle Copy()
        {
            try
            {
                var copy = new UIStyle();
                copy._backgroundColor = _backgroundColor;
                copy._textColor = _textColor;
                copy._borderColor = _borderColor;
                copy._font = _font;
                copy._fontSize = _fontSize;
                copy._padding = _padding;
                copy._margin = _margin;
                copy._borderThickness = _borderThickness;
                copy._cornerRadius = _cornerRadius;
                copy._wordWrap = _wordWrap;
                copy._textAlignment = _textAlignment;

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyle: Created style copy");
                return copy;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyle: Error creating copy - {ex.Message}");
                return new UIStyle();
            }
        }

        /// <summary>
        /// Merges another style into this style
        /// </summary>
        /// <param name="other">Style to merge from</param>
        public void Merge(UIStyle other)
        {
            try
            {
                if (other == null)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyle: Cannot merge null style");
                    return;
                }

                //Only merge non-default values
                if (other._backgroundColor != System.Drawing.Color.Transparent)
                    BackgroundColor = other._backgroundColor;

                if (other._textColor != System.Drawing.Color.Black)
                    TextColor = other._textColor;

                if (other._borderColor != System.Drawing.Color.Black)
                    BorderColor = other._borderColor;

                if (!string.IsNullOrEmpty(other._font))
                    Font = other._font;

                if (other._fontSize != 12.0f)
                    FontSize = other._fontSize;

                if (other._borderThickness > 0)
                    BorderThickness = other._borderThickness;

                if (other._cornerRadius > 0)
                    CornerRadius = other._cornerRadius;

                if (other._wordWrap)
                    WordWrap = other._wordWrap;

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyle: Merged style");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyle: Error merging style - {ex.Message}");
            }
        }

        /// <summary>
        /// Resets the style to default values
        /// </summary>
        public void Reset()
        {
            try
            {
                _backgroundColor = System.Drawing.Color.Transparent;
                _textColor = System.Drawing.Color.Black;
                _borderColor = System.Drawing.Color.Black;
                _font = string.Empty;
                _fontSize = 12.0f;
                _padding = UI.Layout.UIPadding.Zero;
                _margin = UI.Layout.UIMargin.Zero;
                _borderThickness = 0.0f;
                _cornerRadius = 0.0f;
                _wordWrap = false;
                _textAlignment = System.Drawing.ContentAlignment.TopLeft;

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyle: Reset to defaults");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyle: Error resetting style - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a string representation of the style
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            try
            {
                return $"UIStyle: BG={_backgroundColor}, Text={_textColor}, Font={_font}, Size={_fontSize}";
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyle: Error creating string representation - {ex.Message}");
                return "UIStyle: Error";
            }
        }
    }
}

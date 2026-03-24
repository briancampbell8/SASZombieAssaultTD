using System;

namespace SASZombieAssaultTD.Engine.UI.Widgets
{
    /// <summary>
    /// A UI text element with string content, font reference, and color fields
    /// P80-04-01: UIText providing a UI text element with string content, font reference, and color fields
    /// </summary>
    public class UIText : UIWidgetBase
    {
        private string _text = string.Empty;
        private string _font = string.Empty;
        private System.Drawing.Color _color = System.Drawing.Color.White;
        private float _fontSize = 12.0f;
        private bool _wordWrap = false;
        private System.Drawing.ContentAlignment _alignment = System.Drawing.ContentAlignment.TopLeft;

        /// <summary>
        /// Gets or sets the text content
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value ?? string.Empty;
                    InvalidateLayout();
                    Console.WriteLine($"UIText: Text set to '{_text}'");
                }
            }
        }

        /// <summary>
        /// Gets or sets the font reference
        /// </summary>
        public string Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value ?? string.Empty;
                    InvalidateLayout();
                    Console.WriteLine($"UIText: Font set to '{_font}'");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text color
        /// </summary>
        public System.Drawing.Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    Console.WriteLine($"UIText: Color set to {_color}");
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
                    _fontSize = System.Math.Max(1.0f, value);
                    InvalidateLayout();
                    Console.WriteLine($"UIText: Font size set to {_fontSize}");
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
                    InvalidateLayout();
                    Console.WriteLine($"UIText: Word wrap set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text alignment
        /// </summary>
        public System.Drawing.ContentAlignment Alignment
        {
            get => _alignment;
            set
            {
                if (_alignment != value)
                {
                    _alignment = value;
                    InvalidateLayout();
                    Console.WriteLine($"UIText: Alignment set to {value}");
                }
            }
        }

        /// <summary>
        /// Initializes a new UIText
        /// </summary>
        public UIText() : base()
        {
            Console.WriteLine("UIText: Created new text element");
        }

        /// <summary>
        /// Initializes a new UIText with text
        /// </summary>
        /// <param name="text">Initial text content</param>
        public UIText(string text) : this()
        {
            Text = text;
            Console.WriteLine($"UIText: Created text element with '{text}'");
        }

        /// <summary>
        /// Calculates the text size for layout
        /// </summary>
        /// <returns>Calculated text size</returns>
        private System.Drawing.SizeF CalculateTextSize()
        {
            try
            {
                // This would use the actual font rendering system
                // For now, estimate based on character count and font size
                var charCount = _text?.Length ?? 0;
                var width = charCount * _fontSize * 0.6f; // Rough estimate
                var height = _fontSize * 1.2f; // Rough estimate

                return new System.Drawing.SizeF(width, height);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIText: Error calculating text size - {ex.Message}");
                return new System.Drawing.SizeF(100, 20); // Default size
            }
        }

        /// <summary>
        /// Updates the text element
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public override void Update(float deltaTime)
        {
            try
            {
                base.Update(deltaTime);

                // Update text animations or effects here
                UpdateTextAnimation(deltaTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIText: Error during update - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the text element
        /// </summary>
        public override void Render()
        {
            try
            {
                if (string.IsNullOrEmpty(_text) || !IsVisible)
                    return;

                // This would use the actual text rendering system
                // For now, render as a simple rectangle with text indicator
                var textSize = CalculateTextSize();

                // Update size to match text size
                if (Size != textSize)
                {
                    Size = textSize;
                }

                // Render background
                RenderBackground();

                // Render text (placeholder - would use actual text rendering)
                RenderTextContent();

                // Render border if hovered
                if (IsHovered)
                {
                    RenderBorder();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UIText: Error during render - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates text animations (placeholder implementation)
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        protected virtual void UpdateTextAnimation(float deltaTime)
        {
            // Override in derived classes for text animations
            // Examples: typewriter effect, fade in/out, color cycling
        }

        /// <summary>
        /// Renders the text background (placeholder implementation)
        /// </summary>
        protected virtual void RenderBackground()
        {
            // Override in derived classes for custom background rendering
            // For now, render a simple colored rectangle
        }

        /// <summary>
        /// Renders the text content (placeholder implementation)
        /// </summary>
        protected virtual void RenderTextContent()
        {
            // Override in derived classes for actual text rendering
            // For now, just log the text
            Console.WriteLine($"UIText: Rendering text '{_text}' at {AbsolutePosition}");
        }

        /// <summary>
        /// Renders the text border (placeholder implementation)
        /// </summary>
        protected virtual void RenderBorder()
        {
            // Override in derived classes for custom border rendering
            // For now, just log the border
            Console.WriteLine($"UIText: Rendering border for text at {AbsolutePosition}");
        }
    }
}





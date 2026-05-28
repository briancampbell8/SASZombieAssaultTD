using System;

namespace SASZombieAssaultTD.Engine.UI.Widgets
{
    /// <summary>
    /// A button element with label, click event, and state handling
    /// P80-04-03: UIButton providing a button element with label, click event, and state handling
    /// </summary>
    public class UIButton : UIWidgetBase
    {
        private string _label = string.Empty;
        private System.Drawing.Color _normalColor = System.Drawing.Color.LightGray;
        private System.Drawing.Color _hoverColor = System.Drawing.Color.DarkGray;
        private System.Drawing.Color _pressedColor = System.Drawing.Color.Gray;
        private System.Drawing.Color _disabledColor = System.Drawing.Color.DarkGray;
        private System.Drawing.Color _textColor = System.Drawing.Color.Black;
        private string _font = string.Empty;
        private float _fontSize = 12.0f;

        /// <summary>
        /// Gets or sets the button label
        /// </summary>
        public string Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value ?? string.Empty;
                    InvalidateLayout();
                    System.Diagnostics.Debug.WriteLine($"UIButton: Label set to '{_label}'");
                }
            }
        }

        /// <summary>
        /// Gets or sets the normal state color
        /// </summary>
        public System.Drawing.Color NormalColor
        {
            get => _normalColor;
            set
            {
                if (_normalColor != value)
                {
                    _normalColor = value;
                    System.Diagnostics.Debug.WriteLine($"UIButton: Normal color set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the hover state color
        /// </summary>
        public System.Drawing.Color HoverColor
        {
            get => _hoverColor;
            set
            {
                if (_hoverColor != value)
                {
                    _hoverColor = value;
                    System.Diagnostics.Debug.WriteLine($"UIButton: Hover color set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the pressed state color
        /// </summary>
        public System.Drawing.Color PressedColor
        {
            get => _pressedColor;
            set
            {
                if (_pressedColor != value)
                {
                    _pressedColor = value;
                    System.Diagnostics.Debug.WriteLine($"UIButton: Pressed color set to {value}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the disabled state color
        /// </summary>
        public System.Drawing.Color DisabledColor
        {
            get => _disabledColor;
            set
            {
                if (_disabledColor != value)
                {
                    _disabledColor = value;
                    System.Diagnostics.Debug.WriteLine($"UIButton: Disabled color set to {value}");
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
                    System.Diagnostics.Debug.WriteLine($"UIButton: Text color set to {value}");
                }
            }
        }

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
                    InvalidateLayout();
                    System.Diagnostics.Debug.WriteLine($"UIButton: Font set to '{_font}'");
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
                    System.Diagnostics.Debug.WriteLine($"UIButton: Font size set to {_fontSize}");
                }
            }
        }

        /// <summary>
        /// Event fired when the button is clicked
        /// </summary>
        public event EventHandler<UIButtonClickEventArgs> Clicked;

        /// <summary>
        /// Initializes a new UIButton
        /// </summary>
        public UIButton() : base()
        {
            System.Diagnostics.Debug.WriteLine("UIButton: Created new button element");
        }

        /// <summary>
        /// Initializes a new UIButton with label
        /// </summary>
        /// <param name="label">Button label</param>
        public UIButton(string label) : this()
        {
            Label = label;
            System.Diagnostics.Debug.WriteLine($"UIButton: Created button with label '{label}'");
        }

        /// <summary>
        /// Simulates a button click
        /// </summary>
        public void Click()
        {
            try
            {
                if (IsDisabled)
                {
                    System.Diagnostics.Debug.WriteLine("UIButton: Cannot click disabled button");
                    return;
                }

                OnClicked();
                System.Diagnostics.Debug.WriteLine($"UIButton: Button clicked with label '{_label}'");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error during click - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current color based on button state
        /// </summary>
        /// <returns>Current color</returns>
        private System.Drawing.Color GetCurrentColor()
        {
            if (IsDisabled)
                return _disabledColor;
            else if (IsPressed)
                return _pressedColor;
            else if (IsHovered)
                return _hoverColor;
            else
                return _normalColor;
        }

        /// <summary>
        /// Updates the button
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public override void Update(float deltaTime)
        {
            try
            {
                base.Update(deltaTime);

                // Update button-specific animations or effects here
                UpdateButtonAnimation(deltaTime);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error during update - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the button
        /// </summary>
        public override void Render()
        {
            try
            {
                if (!IsVisible)
                    return;

                // Render button background
                RenderBackground();

                // Render button text
                RenderText();

                // Render button border if hovered
                if (IsHovered)
                {
                    RenderBorder();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error during render - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the button background
        /// </summary>
        protected virtual void RenderBackground()
        {
            try
            {
                var currentColor = GetCurrentColor();
                // This would use the actual rendering system
                // For now, just log the background color
                System.Diagnostics.Debug.WriteLine($"UIButton: Rendering background {currentColor} at {AbsolutePosition}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error rendering background - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the button text
        /// </summary>
        protected virtual void RenderText()
        {
            try
            {
                if (string.IsNullOrEmpty(_label))
                    return;

                // This would use the actual text rendering system
                // For now, just log the text
                System.Diagnostics.Debug.WriteLine($"UIButton: Rendering text '{_label}' with color {_textColor} at {AbsolutePosition}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error rendering text - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the button border
        /// </summary>
        protected virtual void RenderBorder()
        {
            try
            {
                // This would use the actual rendering system
                // For now, just log the border
                System.Diagnostics.Debug.WriteLine($"UIButton: Rendering border at {AbsolutePosition}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error rendering border - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates button animations (placeholder implementation)
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        protected virtual void UpdateButtonAnimation(float deltaTime)
        {
            // Override in derived classes for button animations
            // Examples: color transitions, scale effects, pulse animations
        }

        /// <summary>
        /// Called when the button is clicked
        /// </summary>
        protected virtual void OnClicked()
        {
            try
            {
                Clicked?.Invoke(this, new UIButtonClickEventArgs { Label = _label });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error in OnClicked - {ex.Message}");
            }
        }

        /// <summary>
        /// Handles mouse press events
        /// </summary>
        public override void OnMousePress()
        {
            try
            {
                if (!IsDisabled)
                {
                    OnPressed();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error in OnMousePress - {ex.Message}");
            }
        }

        /// <summary>
        /// Handles mouse release events
        /// </summary>
        public override void OnMouseRelease()
        {
            try
            {
                if (!IsDisabled && IsPressed)
                {
                    OnReleased();
                    Click();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIButton: Error in OnMouseRelease - {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Event arguments for button click events
    /// </summary>
    public class UIButtonClickEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the label of the clicked button
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets the timestamp of the click
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}





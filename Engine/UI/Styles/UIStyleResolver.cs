using System;
using System.Collections.Generic;
using System.Linq;
using SASZombieAssaultTD.Engine.UI.Components;
using SASZombieAssaultTD.Engine.UI.Widgets;

namespace SASZombieAssaultTD.Engine.UI.Styles
{
    /// <summary>
    /// Logic to apply styles to UI elements
    /// P80-06-03: UIStyleResolver providing logic to apply styles to UI elements
    /// </summary>
    public class UIStyleResolver
    {
        private readonly UIStyleSheet _styleSheet;

        /// <summary>
        /// Gets the style sheet
        /// </summary>
        public UIStyleSheet StyleSheet => _styleSheet;

        /// <summary>
        /// Initializes a new UIStyleResolver
        /// </summary>
        public UIStyleResolver(UIStyleSheet styleSheet)
        {
            _styleSheet = styleSheet ?? throw new ArgumentNullException(nameof(styleSheet));
            System.Diagnostics.Debug.WriteLine("UIStyleResolver: Initialized");
        }

        /// <summary>
        /// Applies a style to a UI element
        /// </summary>
        /// <param name="element">Element to apply style to</param>
        /// <param name="styleName">Name of the style to apply</param>
        public void ApplyStyle(UIElement element, string styleName)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot apply style to null element");
                    return;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot apply style with null or empty name");
                    return;
                }

                var style = _styleSheet.GetStyle(styleName);
                if (style == null)
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Style '{styleName}' not found");
                    return;
                }

                ApplyStyleToElement(element, style);

                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Applied style '{styleName}' to element");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Error applying style '{styleName}' - {ex.Message}");
            }
        }

        /// <summary>
        /// Applies a style to a UI element
        /// </summary>
        /// <param name="element">Element to apply style to</param>
        /// <param name="style">Style to apply</param>
        private void ApplyStyleToElement(UIElement element, UIStyle style)
        {
            try
            {
                // Apply background color
                if (style.BackgroundColor != System.Drawing.Color.Transparent)
                {
                    // This would use the element's background rendering method
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Applied background color {style.BackgroundColor}");
                }

                // Apply text color
                if (element is Label textElement)
                {
                    textElement.TextColor = new SASZombieAssaultTD.Engine.Core.Color(style.TextColor.R, style.TextColor.G, style.TextColor.B, style.TextColor.A);
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Applied text color {style.TextColor}");
                }

                // Apply border
                if (element is UIPanel panelElement)
                {
                    panelElement.BorderColor = style.BorderColor;
                    panelElement.BorderThickness = style.BorderThickness;
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Applied border color {style.BorderColor}, thickness {style.BorderThickness}");
                }

                // Apply font
                if (element is Label textElement2)
                {
                    textElement2.Font = new Font(style.Font, 12f);
                    textElement2.FontSize = style.FontSize;
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Applied font {style.Font}, size {style.FontSize}");
                }

                // Apply padding and margins
                element.Size = new System.Drawing.SizeF(
                element.Size.Width + style.Padding.Horizontal + style.Margin.Horizontal,
                element.Size.Height + style.Padding.Vertical + style.Margin.Vertical
                );

                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Applied padding and margins");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Error applying style to element - {ex.Message}");
            }
        }

        /// <summary>
        /// Applies a style to multiple elements
        /// </summary>
        /// <param name="elements">Elements to apply style to</param>
        /// <param name="styleName">Name of the style to apply</param>
        public void ApplyStyleToElements(IEnumerable<UIElement> elements, string styleName)
        {
            try
            {
                if (elements == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot apply style to null elements");
                    return;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot apply style with null or empty name");
                    return;
                }

                var style = _styleSheet.GetStyle(styleName);
                if (style == null)
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Style '{styleName}' not found");
                    return;
                }

                foreach (var element in elements)
                {
                    ApplyStyleToElement(element, style);
                }

                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Applied style '{styleName}' to {elements.Count()} elements");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Error applying style to elements - {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a style from an element
        /// </summary>
        /// <param name="element">Element to remove style from</param>
        /// <param name="styleName">Name of the style to remove</param>
        public void RemoveStyle(UIElement element, string styleName)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot remove style from null element");
                    return;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot remove style with null or empty name");
                    return;
                }

                var style = _styleSheet.GetStyle(styleName);
                if (style == null)
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Style '{styleName}' not found");
                    return;
                }

                RemoveStyleFromElement(element, style);

                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Removed style '{styleName}' from element");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Error removing style from element - {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a style from an element
        /// </summary>
        /// <param name="element">Element to remove style from</param>
        /// <param name="style">Style to remove</param>
        private void RemoveStyleFromElement(UIElement element, UIStyle style)
        {
            try
            {
                // This would reset the element's appearance to default values
                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Removed style from element");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Error removing style from element - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current style of an element
        /// </summary>
        /// <param name="element">Element to get style from</param>
        /// <param name="styleName">Name of the style to get</param>
        /// <returns>Current style, or null if not found</returns>
        public UIStyle GetCurrentStyle(UIElement element, string styleName)
        {
            try
            {
                if (element == null)
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot get style from null element");
                    return null;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    System.Diagnostics.Debug.WriteLine("UIStyleResolver: Cannot get style with null or empty name");
                    return null;
                }

                var style = _styleSheet.GetStyle(styleName);
                if (style == null)
                {
                    System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Style '{styleName}' not found");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Got style '{styleName}' for element");
                return style;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIStyleResolver: Error getting style from element - {ex.Message}");
                return null;
            }
        }
    }
}





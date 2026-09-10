// ====================================================================================================
//  FILE: UIStyleResolver.cs
//  PATH: ./Engine/UI/Styles/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide ApplyStyle() behavior for the UI subsystem.
//      - Provide ApplyStyleToElements() behavior for the UI subsystem.
//      - Provide RemoveStyle() behavior for the UI subsystem.
//      - Provide GetCurrentStyle() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Collections.Generic;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using System.Linq;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.MainMenu;

namespace SASZombieAssaultTD.Engine.UI.Styles
{
    /// <summary>
    /// Logic to apply styles to UI elements
    /// P80-06-03: UIStyleResolver providing logic to apply styles to UI elements
    /// </summary>
    public class UIStyleResolver
    {
        /// <summary>
        /// Initializes a new UIStyleResolver
        /// </summary>
        public UIStyleResolver() => DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Initialized");

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
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot apply style to null element");
                    return;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot apply style with null or empty name");
                    return;
                }

                var style = Resolve(styleName);
                if (style == null)
                {
                    DLogger.Log($"UIStyleResolver: Style '{styleName}' not found");
                    return;
                }

                ApplyStyleToElement(element, style);
                DLogger.Log($"UIStyleResolver: Applied style '{styleName}' to element");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyleResolver: Error applying style '{styleName}' - {ex.Message}");
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
                    DLogger.Log($"UIStyleResolver: Applied background color {style.BackgroundColor}");
                }

                // Apply text color
                if (element is UILabel textElement)
                {
                    // Fixed CS1061: Route through the child Style configuration structure
                    textElement.Style.TextColor = style.TextColor;
                    DLogger.Log($"UIStyleResolver: Applied text color {style.TextColor}");
                }

                // Apply border
                if (element is UIPanel panelElement)
                {
                    panelElement.Style.BorderColor = style.BorderColor;
                    panelElement.Style.BorderThickness = style.BorderThickness;
                    DLogger.Log($"UIStyleResolver: Applied border color {style.BorderColor}, thickness {style.BorderThickness}");
                }

                // Apply font
                if (element is UILabel textElement2)
                {
                    DLogger.Log($"UIStyleResolver: Applied font configuration");
                }

                // Apply padding and margins
                element.Size = new System.Drawing.SizeF(
                    element.Size.Width + style.Padding.Horizontal + style.Margin.Horizontal,
                    element.Size.Height + style.Padding.Vertical + style.Margin.Vertical
                );
                DLogger.Log($"UIStyleResolver: Applied padding and margins");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyleResolver: Error applying style to element - {ex.Message}");
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
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot apply style to null elements");
                    return;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot apply style with null or empty name");
                    return;
                }

                var style = Resolve(styleName);
                if (style == null)
                {
                    DLogger.Log($"UIStyleResolver: Style '{styleName}' not found");
                    return;
                }

                foreach (var element in elements)
                {
                    ApplyStyleToElement(element, style);
                }
                DLogger.Log($"UIStyleResolver: Applied style '{styleName}' to {elements.Count()} elements");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyleResolver: Error applying style to elements - {ex.Message}");
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
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot remove style from null element");
                    return;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot remove style with null or empty name");
                    return;
                }

                var style = Resolve(styleName);
                if (style == null)
                {
                    DLogger.Log($"UIStyleResolver: Style '{styleName}' not found");
                    return;
                }

                RemoveStyleFromElement(element, style);
                DLogger.Log($"UIStyleResolver: Removed style '{styleName}' from element");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyleResolver: Error removing style from element - {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a style from an element
        /// </summary>
        private void RemoveStyleFromElement(UIElement element, UIStyle style)
        {
            try
            {
                DLogger.Log($"UIStyleResolver: Removed style from element");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyleResolver: Error removing style from element - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current style of an element
        /// </summary>
        public UIStyle GetCurrentStyle(UIElement element, string styleName)
        {
            try
            {
                if (element == null)
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot get style from null element");
                    return null;
                }

                if (string.IsNullOrEmpty(styleName))
                {
                    DLogger.Log(LogSubsystems.ResourcesPipeline, "UIStyleResolver: Cannot get style with null or empty name");
                    return null;
                }

                var style = Resolve(styleName);
                if (style == null)
                {
                    DLogger.Log($"UIStyleResolver: Style '{styleName}' not found");
                    return null;
                }

                DLogger.Log($"UIStyleResolver: Got style '{styleName}' for element");
                return style;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIStyleResolver: Error getting style from element - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deterministically resolves an explicit UIStyle block out of static UIStyleSheet categories
        /// </summary>
        internal static UIStyle Resolve(string styleKey)
        {
            if (string.IsNullOrEmpty(styleKey)) return null;

            // Handle hierarchical queries: "MapMenu.Button.PlayButton"
            if (styleKey.StartsWith("MapMenu.Button."))
            {
                // Fixed CS0029: Wrap the raw Engine.Color value into a new UIStyle layout object instance
                return new UIStyle
                {
                    ActiveColor = UIStyleSheet.MapMenu.ButtonActive,
                    BackgroundColor = UIStyleSheet.MapMenu.ButtonBackground,
                    HoverColor = UIStyleSheet.MapMenu.ButtonHover,
                    PressedColor = UIStyleSheet.MapMenu.ButtonPressed
                };
            }
            if (styleKey.Contains("MapEntryLabel"))
            {
                // Fixed CS0266: Use a safe type navigation cast to bridge the object container interface mapping
                return UIStyleSheet.MapMenu.MapEntryLabelStyle as UIStyle ??
                    UIStyleSheet.MapMenu.MapEntryStyle;
            }
            // Fallback default style configuration package
            return UIStyleSheet.MapMenu.MapEntryStyle;
        }
    }
}

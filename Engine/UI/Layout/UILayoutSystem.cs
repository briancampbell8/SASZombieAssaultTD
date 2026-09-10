// ====================================================================================================
//  FILE: UILayoutSystem.cs
//  PATH: ./Engine/UI/Layout/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide CalculateLayout() behavior for the UI subsystem.
//      - Provide InvalidateLayout() behavior for the UI subsystem.
//      - Provide GetLayoutData() behavior for the UI subsystem.
//      - Provide Clear() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Components;
using SASZombieAssaultTD.Engine.UI.Elements;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI.Layout
{
    /// <summary>
    /// Layout calculation and element positioning logic P80-02-01: UILayoutSystem providing layout calculation and
    /// element positioning logic
    /// </summary>
    public class UILayoutSystem
    {
        private readonly Dictionary<UIElement, UILayoutData> _layoutData;
        private bool _needsRecalculation = true;

        /// <summary>
        /// Gets whether the layout system needs recalculation
        /// </summary>
        public bool NeedsRecalculation => _needsRecalculation;

        public class UILayoutData
        {
            public PointF CalculatedPosition { get; set; }
            public System.Drawing.SizeF CalculatedSize { get; set; }
            public DateTime LastCalculated { get; set; }
        }

        /// <summary>
        /// Initializes a new UILayoutSystem
        /// </summary>
        public UILayoutSystem()
        {
            _layoutData = new Dictionary<UIElement, UILayoutData>();
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UILayoutSystem: Initialized");
        }

        /// <summary>
        /// Calculates layout for a UI element and its children
        /// </summary>
        /// <param name="element">Root element to calculate layout for</param>
        public void CalculateLayout(UIElement element)
        {
            try
            {
                if (element == null)
                    return;

                DLogger.Log($"UILayoutSystem: Calculating layout for element");

                //Calculate layout for the element
                CalculateElementLayout(element);

                //Calculate layouts for children
                foreach (var child in element.Children)
                {
                    CalculateLayout(child);
                }

                _needsRecalculation = false;
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UILayoutSystem: Layout calculation completed");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UILayoutSystem: Error calculating layout - {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates layout for a single element
        /// </summary>
        /// <param name="element">Element to calculate layout for</param>
        private void CalculateElementLayout(UIElement element)
        {
            try
            {
                //Get or create layout data for this element
                if (!_layoutData.TryGetValue(element, out var layoutData))
                {
                    layoutData = new UILayoutData();
                    _layoutData[element] = layoutData;
                }

                //Calculate position based on anchor and alignment
                var calculatedPosition = CalculateElementPosition(element);
                var calculatedSize = CalculateElementSize(element);

                //Update element position and size if different
                if (element.Position.Equals(calculatedPosition))
                {
                    element.Position = calculatedPosition;
                }

                if (element.Size != calculatedSize)
                {
                    element.Size = calculatedSize;
                }

                //Store layout data
                layoutData.CalculatedPosition = calculatedPosition;
                layoutData.CalculatedSize = calculatedSize;
                layoutData.LastCalculated = DateTime.Now;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UILayoutSystem: Error calculating element layout - {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates the position of an element based on its anchor and alignment
        /// </summary>
        /// <param name="element">Element to calculate position for</param>
        /// <returns>Calculated position</returns>
        private PointF CalculateElementPosition(UIElement element)
        {
            try
            {
                if (element.Parent == null)
                {
                    //Root element stays at its current position
                    return element.Position;
                }

                var parentSize = element.Parent.Size;
                var elementSize = element.Size;
                var anchor = GetElementAnchor(element);
                var alignment = GetElementAlignment(element);

                var position = element.Position;

                //Apply anchor positioning
                switch (anchor)
                {
                    case UIAnchor.TopCenter:
                        position.X = (parentSize.Width - elementSize.Width) / 2;
                        break;

                    case UIAnchor.TopRight:
                        position.X = parentSize.Width - elementSize.Width;
                        break;

                    case UIAnchor.MiddleLeft:
                        position.Y = (parentSize.Height - elementSize.Height) / 2;
                        break;

                    case UIAnchor.MiddleCenter:
                        position.X = (parentSize.Width - elementSize.Width) / 2;
                        position.Y = (parentSize.Height - elementSize.Height) / 2;
                        break;

                    case UIAnchor.MiddleRight:
                        position.X = parentSize.Width - elementSize.Width;
                        position.Y = (parentSize.Height - elementSize.Height) / 2;
                        break;

                    case UIAnchor.BottomLeft:
                        position.Y = parentSize.Height - elementSize.Height;
                        break;

                    case UIAnchor.BottomCenter:
                        position.X = (parentSize.Width - elementSize.Width) / 2;
                        position.Y = parentSize.Height - elementSize.Height;
                        break;

                    case UIAnchor.BottomRight:
                        position.X = parentSize.Width - elementSize.Width;
                        position.Y = parentSize.Height - elementSize.Height;
                        break;

                    case UIAnchor.TopLeft:
                    default:
                        //Keep current position for top-left anchor
                        break;
                }

                return position;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UILayoutSystem: Error calculating element position - {ex.Message}");
                return element.Position;
            }
        }

        /// <summary>
        /// Calculates the size of an element based on constraints and content
        /// </summary>
        /// <param name="element">Element to calculate size for</param>
        /// <returns>Calculated size</returns>
        private System.Drawing.SizeF CalculateElementSize(UIElement element)
        {
            try
            {
                var constraints = GetElementConstraints(element);
                var currentSize = element.Size;

                //Apply size constraints
                var width = System.Math.Max(constraints.MinWidth, System.Math.Min(constraints.MaxWidth, currentSize.Width));
                var height = System.Math.Max(constraints.MinHeight, System.Math.Min(constraints.MaxHeight, currentSize.Height));

                //Use preferred size if current size is zero
                if (currentSize.Width == 0 && constraints.PreferredWidth > 0)
                    width = constraints.PreferredWidth;
                if (currentSize.Height == 0 && constraints.PreferredHeight > 0)
                    height = constraints.PreferredHeight;

                return new System.Drawing.SizeF(width, height);
            }
            catch (Exception ex)
            {
                DLogger.Log($"UILayoutSystem: Error calculating element size - {ex.Message}");
                return element.Size;
            }
        }

        /// <summary>
        /// Gets the anchor for an element (placeholder implementation)
        /// </summary>
        /// <param name="element">Element to get anchor for</param>
        /// <returns>Anchor value</returns>
        private UIAnchor GetElementAnchor(UIElement element)
        {
            //This would be stored in the element's style or layout properties
            //For now, default to top-left
            return UIAnchor.TopLeft;
        }

        /// <summary>
        /// Gets the alignment for an element (placeholder implementation)
        /// </summary>
        /// <param name="element">Element to get alignment for</param>
        /// <returns>Alignment value</returns>
        private UIAlignment GetElementAlignment(UIElement element)
        {
            //This would be stored in the element's style or layout properties
            //For now, default to left
            return UIAlignment.Left;
        }

        /// <summary>
        /// Gets the layout constraints for an element (placeholder implementation)
        /// </summary>
        /// <param name="element">Element to get constraints for</param>
        /// <returns>Layout constraints</returns>
        private UILayoutConstraints GetElementConstraints(UIElement element)
        {
            //This would be stored in the element's style or layout properties
            //For now, return unconstrained
            return UILayoutConstraints.Unconstrained;
        }

        /// <summary>
        /// Invalidates layout for an element and its children
        /// </summary>
        /// <param name="element">Element to invalidate</param>
        public void InvalidateLayout(UIElement element)
        {
            try
            {
                if (element == null)
                    return;

                _needsRecalculation = true;

                //Remove cached layout data
                _layoutData.Remove(element);

                //Invalidate children
                foreach (var child in element.Children)
                {
                    InvalidateLayout(child);
                }

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UILayoutSystem: Layout invalidated");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UILayoutSystem: Error invalidating layout - {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the layout data for an element
        /// </summary>
        /// <param name="element">Element to get layout data for</param>
        /// <returns>Layout data, or null if not found</returns>
        public UILayoutData GetLayoutData(UIElement element)
        {
            try
            {
                _layoutData.TryGetValue(element, out var layoutData);
                return layoutData;
            }
            catch (Exception ex)
            {
                DLogger.Log($"UILayoutSystem: Error getting layout data - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Clears all layout data
        /// </summary>
        public void Clear()
        {
            try
            {
                _layoutData.Clear();
                _needsRecalculation = true;
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UILayoutSystem: Layout data cleared");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UILayoutSystem: Error clearing layout data - {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Internal data structure for storing layout information
    /// </summary>
}

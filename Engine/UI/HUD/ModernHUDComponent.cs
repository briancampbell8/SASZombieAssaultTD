// ====================================================================================================
//  FILE: ModernHUDComponent.cs
//  PATH: Engine/UI/HUD/
//  MODULE: UI/HUD Components (Modernization)
//
//  ROLE:
//      Base class for modernized HUD components that bridge legacy HUDComponent behavior to P80 UI widgets.
//
//  RESPONSIBILITIES:
//      - Maintain backward-compatible HUDComponent API while enabling P80 widget composition.
//      - Provide helper methods to create and manage UIText, UIPanel, UIButton widgets.
//      - Surface layout, style, and rendering toggles for transitional modernization.
//
//  NON-RESPONSIBILITIES:
//      - Rendering backend implementation details (delegated to UIRenderer/P80 systems).
//
//  ARCHITECTURAL NOTES:
//      - This class intentionally favors composition and minimal override patterns to ease migration.
// ====================================================================================================

using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.UI.Widgets;
using SASZombieAssaultTD.Engine.UI.Components;
using SASZombieAssaultTD.Engine.UI.Layout;
using SASZombieAssaultTD.Engine.UI.Styles;
using System;
using System.Drawing;
using System.Diagnostics;
//
using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Extensions;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    ///<summary>
    ///P80 UI/HUD Rendering Modernization - Modern HUD component base class.
    ///Integrates HUD components with P80 UI widgets for modern rendering pipeline.
    ///</summary>
    public abstract class ModernHUDComponent : HUDComponent
    {
        protected readonly Dictionary<string, UIElement> _uiWidgets = new();
        protected UIRoot _uiRoot;
        protected bool _useP80Rendering = true;

        //P80 Layout properties
        private UIPadding _padding = UIPadding.Zero;
        private UIMargin _margin = UIMargin.Zero;
        private UIAnchor _anchor = UIAnchor.TopLeft;
        private UIAlignment _horizontalAlignment = UIAlignment.Left;
        private UIVerticalAlignment _verticalAlignment = UIVerticalAlignment.Top;
        private UILayoutConstraints _constraints = UILayoutConstraints.Unconstrained;

        //P80 Style properties
        private UIStyle _style;
        private UIStyleSheet _styleSheet;

        ///<summary>
        ///Gets or sets whether to use P80 rendering system.
        ///</summary>
        public bool UseP80Rendering
        {
            get => _useP80Rendering;
            set => _useP80Rendering = value;
        }

        ///<summary>
        ///Gets the UI root for P80 widget management.
        ///</summary>
        public UIRoot UIRoot => _uiRoot;

        ///<summary>
        ///Gets or sets the padding for this component.
        ///</summary>
        public UIPadding Padding
        {
            get => _padding;
            set
            {
                _padding = value;
                InvalidateLayout();
            }
        }

        ///<summary>
        ///Gets or sets the margin for this component.
        ///</summary>
        public UIMargin Margin
        {
            get => _margin;
            set
            {
                _margin = value;
                InvalidateLayout();
            }
        }

        ///<summary>
        ///Gets or sets the anchor point for this component.
        ///</summary>
        public UIAnchor Anchor
        {
            get => _anchor;
            set
            {
                _anchor = value;
                InvalidateLayout();
            }
        }

        ///<summary>
        ///Gets or sets the horizontal alignment for this component.
        ///</summary>
        public UIAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                _horizontalAlignment = value;
                InvalidateLayout();
            }
        }

        ///<summary>
        ///Gets or sets the vertical alignment for this component.
        ///</summary>
        public UIVerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                _verticalAlignment = value;
                InvalidateLayout();
            }
        }

        ///<summary>
        ///Gets or sets the layout constraints for this component.
        ///</summary>
        public UILayoutConstraints Constraints
        {
            get => _constraints;
            set
            {
                _constraints = value;
                InvalidateLayout();
            }
        }

        ///<summary>
        ///Gets or sets the style for this component.
        ///</summary>
        public UIStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                ApplyStyle();
            }
        }

        ///<summary>
        ///Gets or sets the style sheet for this component.
        ///</summary>
        public UIStyleSheet StyleSheet
        {
            get => _styleSheet;
            set
            {
                _styleSheet = value;
                ApplyStyleFromSheet();
            }
        }

        ///<summary>
        ///Initializes the modern HUD component.
        ///</summary>
        public override void Initialize()
        {
            base.Initialize();

            if (_useP80Rendering)
            {
                InitializeP80Widgets();
                System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Initialized P80 widgets for {GetType().Name}");
            }
        }

        ///<summary>
        ///Updates the modern HUD component.
        ///</summary>
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (_useP80Rendering && _uiRoot != null)
            {
                _uiRoot.Update(deltaTime);
            }
        }

        ///<summary>
        ///Renders the modern HUD component.
        ///</summary>
        public override void Render()
        {
            if (!_isVisible) return;

            if (_useP80Rendering && _uiRoot != null)
            {
                _uiRoot.Render();
            }
            else
            {
                //Fall back to legacy rendering
                RenderLegacy();
            }
        }

        ///<summary>
        ///Creates a text widget for this component.
        ///</summary>
        protected UIText CreateTextWidget(string name, string text = "", Vector3? position = null, Vector3? size = null)
        {
            var textWidget = new UIText(text);
            
            if (position.HasValue)
            {
                textWidget.Position = new System.Drawing.PointF(position.Value.X, position.Value.Y);
            }
            
            if (size.HasValue)
            {
                textWidget.Size = new System.Drawing.SizeF(size.Value.X, size.Value.Y);
            }

            _uiWidgets[name] = textWidget;
            
            if (_uiRoot != null)
            {
                _uiRoot.AddElement(textWidget);
            }

            System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Created text widget '{name}'");
            return textWidget;
        }

        ///<summary>
        ///Creates a panel widget for this component.
        ///</summary>
        protected UIPanel CreatePanelWidget(string name, Color? backgroundColor = null, Vector3? position = null, Vector3? size = null)
        {
            //TODO: Cannot assign Engine.Core.Color to System.Drawing.Color
            var panel = backgroundColor.HasValue ? new UIPanel(System.Drawing.Color.White) : new UIPanel();
            
            if (position.HasValue)
            {
                panel.Position = new System.Drawing.PointF(position.Value.X, position.Value.Y);
            }
            
            if (size.HasValue)
            {
                panel.Size = new System.Drawing.SizeF(size.Value.X, size.Value.Y);
            }

            _uiWidgets[name] = panel;
            
            if (_uiRoot != null)
            {
                _uiRoot.AddElement(panel);
            }

            System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Created panel widget '{name}'");
            return panel;
        }

        ///<summary>
        ///Gets a widget by name.
        ///</summary>
        protected T GetWidget<T>(string name) where T : UIElement
        {
            if (_uiWidgets.TryGetValue(name, out var widget) && widget is T typedWidget)
            {
                return typedWidget;
            }
            return null;
        }

        ///<summary>
        ///Removes a widget by name.
        ///</summary>
        protected void RemoveWidget(string name)
        {
            if (_uiWidgets.TryGetValue(name, out var widget))
            {
                _uiRoot?.RemoveElement(widget);
                _uiWidgets.Remove(name);
                System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Removed widget '{name}'");
            }
        }

        ///<summary>
        ///Initializes P80 UI widgets for this component.
        ///Override in derived classes to create specific widgets.
        ///</summary>
        protected virtual void InitializeP80Widgets()
        {
            //Create UI root if not exists
            if (_uiRoot == null)
            {
                _uiRoot = new UIRoot();
                _uiRoot.Initialize();
            }

            //Override in derived classes to create specific widgets
            System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: InitializeP80Widgets called for {GetType().Name}");
        }

        ///<summary>
        ///Applies layout constraints to ensure component size meets constraints.
        ///</summary>
        protected void ApplyLayoutConstraints()
        {
            var constrainedSize = _size;

            //Apply minimum constraints
            if (constrainedSize.X < _constraints.MinWidth)
                constrainedSize = new Vector3(_constraints.MinWidth, constrainedSize.Y, constrainedSize.Z);
            if (constrainedSize.Y < _constraints.MinHeight)
                constrainedSize = new Vector3(constrainedSize.X, _constraints.MinHeight, constrainedSize.Z);

            //Apply maximum constraints
            if (constrainedSize.X > _constraints.MaxWidth)
                constrainedSize = new Vector3(_constraints.MaxWidth, constrainedSize.Y, constrainedSize.Z);
            if (constrainedSize.Y > _constraints.MaxHeight)
                constrainedSize = new Vector3(constrainedSize.X, _constraints.MaxHeight, constrainedSize.Z);

            //Apply preferred size if unconstrained
            if (_constraints.PreferredWidth > 0 && _size.X == 0)
                constrainedSize = new Vector3(_constraints.PreferredWidth, constrainedSize.Y, constrainedSize.Z);
            if (_constraints.PreferredHeight > 0 && _size.Y == 0)
                constrainedSize = new Vector3(constrainedSize.X, _constraints.PreferredHeight, constrainedSize.Z);

            if (!constrainedSize.Equals(_size))
            {
                _size = constrainedSize;
                UpdateBounds();
            }
        }

        ///<summary>
        ///Calculates the layout position based on anchor and alignment.
        ///</summary>
        protected Vector3 CalculateLayoutPosition(Vector3 parentPosition, Vector3 parentSize)
        {
            var position = _position;

            //Apply anchor
            switch (_anchor)
            {
                case UIAnchor.TopCenter:
                    position = new Vector3(parentPosition.X + (parentSize.X - _size.X) / 2f, parentPosition.Y, position.Z);
                    break;
                case UIAnchor.TopRight:
                    position = new Vector3(parentPosition.X + parentSize.X - _size.X - _margin.Right, parentPosition.Y, position.Z);
                    break;
                case UIAnchor.MiddleLeft:
                    position = new Vector3(parentPosition.X + _margin.Left, parentPosition.Y + (parentSize.Y - _size.Y) / 2f, position.Z);
                    break;
                case UIAnchor.MiddleCenter:
                    position = new Vector3(parentPosition.X + (parentSize.X - _size.X) / 2f, parentPosition.Y + (parentSize.Y - _size.Y) / 2f, position.Z);
                    break;
                case UIAnchor.MiddleRight:
                    position = new Vector3(parentPosition.X + parentSize.X - _size.X - _margin.Right, parentPosition.Y + (parentSize.Y - _size.Y) / 2f, position.Z);
                    break;
                case UIAnchor.BottomLeft:
                    position = new Vector3(parentPosition.X + _margin.Left, parentPosition.Y + parentSize.Y - _size.Y - _margin.Bottom, position.Z);
                    break;
                case UIAnchor.BottomCenter:
                    position = new Vector3(parentPosition.X + (parentSize.X - _size.X) / 2f, parentPosition.Y + parentSize.Y - _size.Y - _margin.Bottom, position.Z);
                    break;
                case UIAnchor.BottomRight:
                    position = new Vector3(parentPosition.X + parentSize.X - _size.X - _margin.Right, parentPosition.Y + parentSize.Y - _size.Y - _margin.Bottom, position.Z);
                    break;
                case UIAnchor.TopLeft:
                default:
                    position = new Vector3(parentPosition.X + _margin.Left, parentPosition.Y + _margin.Top, position.Z);
                    break;
            }

            //Apply padding to effective size
            var effectiveSize = new Vector3(
                _size.X - _padding.Horizontal,
                _size.Y - _padding.Vertical,
                _size.Z
            );

            //Apply horizontal alignment
            switch (_horizontalAlignment)
            {
                case UIAlignment.Center:
                    position = new Vector3(position.X + _padding.Left + (effectiveSize.X - _size.X) / 2f, position.Y, position.Z);
                    break;
                case UIAlignment.Right:
                    position = new Vector3(position.X + _padding.Left + (effectiveSize.X - _size.X), position.Y, position.Z);
                    break;
                case UIAlignment.Left:
                default:
                    position = new Vector3(position.X + _padding.Left, position.Y, position.Z);
                    break;
            }

            //Apply vertical alignment
            switch (_verticalAlignment)
            {
                case UIVerticalAlignment.Middle:
                    position = new Vector3(position.X, position.Y + _padding.Top + (effectiveSize.Y - _size.Y) / 2f, position.Z);
                    break;
                case UIVerticalAlignment.Bottom:
                    position = new Vector3(position.X, position.Y + _padding.Top + (effectiveSize.Y - _size.Y), position.Z);
                    break;
                case UIVerticalAlignment.Top:
                default:
                    position = new Vector3(position.X, position.Y + _padding.Top, position.Z);
                    break;
            }

            return position;
        }

        ///<summary>
        ///Invalidates the layout to force recalculation.
        ///</summary>
        protected void InvalidateLayout()
        {
            NeedsLayoutUpdate = true;
            System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Layout invalidated for {GetType().Name}");
        }

        ///<summary>
        ///Applies the current style to this component.
        ///</summary>
        protected virtual void ApplyStyle()
        {
            if (_style == null) return;

            try
            {
                //Apply style properties to component
                if (_style.BackgroundColor != Color.Transparent)
                {
                    
                    BackgroundColor = Color.Transparent;
                }

                //Apply padding and margin from style
                if (_style.Padding.Horizontal > 0 || _style.Padding.Vertical > 0)
                {
                    Padding = _style.Padding;
                }

                if (_style.Margin.Horizontal > 0 || _style.Margin.Vertical > 0)
                {
                    Margin = _style.Margin;
                }

                //Apply style to widgets
                foreach (var widget in _uiWidgets.Values)
                {
                    ApplyStyleToWidget(widget, _style);
                }

                System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Applied style to {GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Error applying style - {ex.Message}");
            }
        }

        ///<summary>
        ///Applies style from the style sheet based on component type.
        ///</summary>
        protected virtual void ApplyStyleFromSheet()
        {
            if (_styleSheet == null) return;

            try
            {
                var style = _styleSheet.GetStyle(GetType().Name);
                if (style != null)
                {
                    Style = style;
                    System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Applied style from sheet for {GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Error applying style from sheet - {ex.Message}");
            }
        }

        ///<summary>
        ///Applies a style to a specific widget.
        ///</summary>
        protected virtual void ApplyStyleToWidget(UIElement widget, UIStyle style)
        {
            if (widget == null || style == null) return;

            try
            {
                //Apply style based on widget type
                if (widget is UIText textWidget)
                {
                    textWidget.Color = style.TextColor;
                    textWidget.Font = style.Font;
                    textWidget.FontSize = style.FontSize;
                    textWidget.Alignment = style.TextAlignment;
                    textWidget.WordWrap = style.WordWrap;
                }
                else if (widget is UIPanel panelWidget)
                {
                    panelWidget.BackgroundColor = style.BackgroundColor;
                    if (style.HasBorder)
                    {
                        panelWidget.SetBorder(style.BorderColor, style.BorderThickness);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Applied style to widget {widget.GetType().Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Error applying style to widget - {ex.Message}");
            }
        }

        ///<summary>
        ///Legacy rendering fallback for when P80 rendering is disabled.
        ///Override in derived classes for legacy rendering implementation.
        ///</summary>
        protected virtual void RenderLegacy()
        {
            //Override in derived classes for legacy rendering
            System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Legacy rendering called for {GetType().Name}");
        }

        ///<summary>
        ///Cleans up the modern HUD component.
        ///</summary>
        public new void Cleanup()
        {
            if (_uiRoot != null)
            {
                _uiRoot.Shutdown();
                _uiRoot = null;
            }

            _uiWidgets.Clear();
            base.Cleanup();

            System.Diagnostics.Debug.WriteLine($"ModernHUDComponent: Cleaned up {GetType().Name}");
        }
    }
}

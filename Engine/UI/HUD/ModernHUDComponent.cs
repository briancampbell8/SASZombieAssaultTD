// =====================================================================================================
//  FILE: ModernHUDComponent.cs
//  PATH: Engine/UI/HUD/ModernHUDComponent.cs
//  SUBSYSTEM: HUD Modernization Layer
//
//  ROLE:
//      Provides the deterministic modernization bridge between legacy HUDComponent behavior and the
//      P80 widget-driven UI pipeline. Establishes the unified lifecycle, layout contract, styling
//      hooks, and rendering pathways required for all modern HUD modules.
//
//  RESPONSIBILITIES:
//      - Define the deterministic lifecycle surface for modern HUD components.
//      - Provide unified layout, padding, margin, anchoring, and alignment controls.
//      - Manage P80 widget creation, registration, and styling through UIRoot.
//      - Apply UIStyle and static UIStyleSheet mappings to HUD widgets.
//      - Serve as the foundational base class for all upgraded HUD modules.
//
//  NON-RESPONSIBILITIES:
//      - Implementing concrete HUD visuals (handled by derived HUD components).
//      - Managing game state, engine systems, or gameplay logic.
//      - Performing low-level rendering commands (handled by UIRenderer).
//
//  ARCHITECTURAL NOTES:
//      - This modernization layer replaces the legacy HUDComponent inheritance chain.
//      - Derived HUD modules implement RenderLegacy() for fallback behavior.
//      - P80 widgets are created through unified factory helpers and attached to UIRoot.
//      - Static UIStyleSheet values are consumed directly; the sheet itself is never stored.
//      - All modern HUD modules MUST inherit from this class without exception.
// =====================================================================================================

using System.Collections.Generic;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.UI.Elements;
using SASZombieAssaultTD.Engine.UI.Layout;
using SASZombieAssaultTD.Engine.UI.Styles;
using SASZombieAssaultTD.Engine.VectorMath;
// Resolve ambiguity between Elements.UIAnchor and Layout.UIAnchor
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    public abstract class ModernHUDComponent
    {
        // Deterministic layout properties
        protected Vector3 _position = Vector3.Zero;
        protected Vector3 _size = Vector3.Zero;
        protected bool _isVisible = true;
        protected bool NeedsLayoutUpdate = false;

        protected Color BackgroundColor = Color.Transparent;

        // Modern HUD System Backing Fields
        protected readonly Dictionary<string, UIElement> _uiWidgets = new();
        protected UIRoot _uiRoot;
        protected bool _useP80Rendering = true;

        private UIPadding _padding = UIPadding.Zero;
        private UIMargin _margin = UIMargin.Zero;
        private UIAnchor _anchor = UIAnchor.TopLeft;
        private UIAlignment _horizontalAlignment = UIAlignment.Left;
        private UIVerticalAlignment _verticalAlignment = UIVerticalAlignment.Top;
        private UILayoutConstraints _constraints = UILayoutConstraints.Unconstrained;

        private UIStyle _style;

        // Public Interface
        public bool UseP80Rendering
        {
            get => _useP80Rendering;
            set => _useP80Rendering = value;
        }

        public UIRoot UIRoot => _uiRoot;

        public UIPadding Padding
        {
            get => _padding;
            set { _padding = value; InvalidateLayout(); }
        }

        public UIMargin Margin
        {
            get => _margin;
            set { _margin = value; InvalidateLayout(); }
        }

        public UIAnchor Anchor
        {
            get => _anchor;
            set { _anchor = value; InvalidateLayout(); }
        }

        public UIAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set { _horizontalAlignment = value; InvalidateLayout(); }
        }

        public UIVerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set { _verticalAlignment = value; InvalidateLayout(); }
        }

        public UILayoutConstraints Constraints
        {
            get => _constraints;
            set { _constraints = value; InvalidateLayout(); }
        }

        public UIStyle Style
        {
            get => _style;
            set { _style = value; ApplyStyle(); }
        }

        // Lifecycle
        public virtual void Initialize()
        {
            if (_useP80Rendering)
            {
                InitializeP80Widgets();
                ApplyStyle();
                InvalidateLayout();
                DLogger.Log($"ModernHUDComponent: Initialized P80 widgets for {GetType().Name}");
            }
        }

        public virtual void Update(float deltaTime)
        {
            // Modern HUD components do not drive UIRoot.
            // They only update internal state if needed.
        }

        public virtual void Render()
        {
            if (!_isVisible) return;

            if (_useP80Rendering && _uiRoot != null)
            {
                // Rendering is delegated to UIRoot; HUD components do not issue draw calls.
                _uiRoot.Render();
            }
            else
            {
                RenderLegacy();
            }
        }

        protected abstract void RenderLegacy();

        // Layout & Styling
        protected virtual void InvalidateLayout() => NeedsLayoutUpdate = true;

        protected virtual void InitializeP80Widgets() { }

        protected virtual void ApplyStyle()
        {
            if (_style == null) return;

            foreach (var widget in _uiWidgets.Values)
                ApplyStyleToWidget(widget, _style);
        }

        protected virtual void ApplyStyleToWidget(UIElement widget, UIStyle style)
        {
            switch (widget)
            {
                case UIText textWidget:
                    textWidget.Color = style.ForegroundColor;
                    textWidget.Opacity = style.Opacity;
                    break;

                case UIPanel panelWidget:
                    panelWidget.BackgroundColor = style.BackgroundColor;
                    panelWidget.BorderColor = style.BorderColor;
                    break;
            }
        }

        // Widget Factory Helpers (pure construction, no layout mutation)
        protected UIText CreateTextWidget(string name, string text = "")
        {
            var textWidget = new UIText(text);

            _uiWidgets[name] = textWidget;
            _uiRoot?.AddElement(textWidget);

            DLogger.Log($"ModernHUDComponent: Created text widget '{name}'");
            return textWidget;
        }

        protected UIPanel CreatePanelWidget(string name, Color? backgroundColor = null, Vector3 position = default)
        {
            var panel = backgroundColor.HasValue
                ? new UIPanel(backgroundColor.Value)
                : new UIPanel();

            _uiWidgets[name] = panel;
            _uiRoot?.AddElement(panel);

            DLogger.Log($"ModernHUDComponent: Created panel widget '{name}'");
            return panel;
        }

        // Cleanup
        internal virtual void Cleanup()
        {
            if (_uiRoot != null)
            {
                foreach (var widget in _uiWidgets.Values)
                    _uiRoot.RemoveElement(widget);
            }

            _uiWidgets.Clear();
            _uiRoot = null;

            DLogger.Log($"ModernHUDComponent: Cleaned up {GetType().Name}");
        }
    }
}

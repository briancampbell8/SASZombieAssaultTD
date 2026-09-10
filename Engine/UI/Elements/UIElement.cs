// =====================================================================================================
//  FILE: UIElement.cs
//  PATH: Engine/UI/UIElement.cs
//  SUBSYSTEM: UI Subsystem / Core Components
//
//  ROLE:
//      Base building block component representing an interactive or graphical layout element.
//      Provides position, sizing, visibility, hierarchy management, and event-routing wrappers
//      for downstream elements and HUD elements.
//
//  RESPONSIBILITIES:
//      - Provide Update(), Render(), and UpdateLayout() execution hooks for the UI hierarchy.
//      - Manage parent and child component registration structures deterministically.
//      - Perform coordinate accumulation to determine absolute screen position bounds.
//      - Handle hit-testing and input target routing queries across children elements.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence, asset database management, or file serialization.
// =====================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Input;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using SASZombieAssaultTD.Engine.UI.Styles;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

namespace SASZombieAssaultTD.Engine.UI.Elements
{
    public class UIElement : UIElementBase
    {
        // --------------------------------------------------------------------------------------------
        // CORE FIELDS
        // --------------------------------------------------------------------------------------------
        private string _id = string.Empty;
        private Components.PointF _position;
        private SizeF _size;
        private bool _isVisible = true;
        private bool _needsLayoutUpdate = true;

        private UIElement? _parent;
        private readonly List<UIElement> _children = new();

        private float _opacity = 1f;

        // --------------------------------------------------------------------------------------------
        // FOCUS EVENTS
        // --------------------------------------------------------------------------------------------
        public Action? OnFocusGained { get; set; }
        public Action? OnFocusLost { get; set; }
        public Action? OnFocus { get; set; }

        // --------------------------------------------------------------------------------------------
        // STATE FLAGS
        // --------------------------------------------------------------------------------------------
        public bool IsEnabled { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsFocusable { get; set; } = true;

        // --------------------------------------------------------------------------------------------
        // STYLE
        // --------------------------------------------------------------------------------------------
        public UIStyle? Style { get; set; }

        // --------------------------------------------------------------------------------------------
        // IDENTIFIERS
        // --------------------------------------------------------------------------------------------
        public string Id
        {
            get => _id;
            set => _id = value ?? string.Empty;
        }

        public object? Value { get; set; }
        public object? Name { get; set; }

        // --------------------------------------------------------------------------------------------
        // POSITION / SIZE / VISIBILITY
        // --------------------------------------------------------------------------------------------
        public Components.PointF Position
        {
            get => _position;
            set
            {
                if (_position.X == value.X && _position.Y == value.Y)
                    return;

                _position = value;
                InvalidateLayout();
            }
        }

        public SizeF Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    InvalidateLayout();
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    InvalidateLayout();
                }
            }
        }

        public float Opacity
        {
            get => _opacity;
            set => _opacity = value;
        }

        public bool NeedsLayoutUpdate => _needsLayoutUpdate;

        // --------------------------------------------------------------------------------------------
        // HIERARCHY
        // --------------------------------------------------------------------------------------------
        public UIElement? Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                    return;

                if (_parent != null)
                    _parent.RemoveChild(this);

                _parent = value;

                if (_parent != null)
                    _parent.AddChild(this);

                InvalidateLayout();
            }
        }

        public IReadOnlyList<UIElement> Children => _children.AsReadOnly();

        // --------------------------------------------------------------------------------------------
        // ABSOLUTE POSITION
        // --------------------------------------------------------------------------------------------
        public Components.PointF AbsolutePosition
        {
            get
            {
                if (_parent == null)
                    return _position;

                var p = _parent.AbsolutePosition;
                return new Components.PointF(p.X + _position.X, p.Y + _position.Y);
            }
        }

        public RectangleF Bounds =>
            new RectangleF(AbsolutePosition.X, AbsolutePosition.Y, Size.Width, Size.Height);

        public Action? Initialize { get; internal set; }

        // --------------------------------------------------------------------------------------------
        // CONSTRUCTORS
        // --------------------------------------------------------------------------------------------

        public UIElement()
        {
            DLogger.Log(LogSubsystems.ResourcesPipeline, "UIElement: Created new UI element");
        }

        public UIElement(string id, Components.PointF position, SizeF size)
        {
            Id = id;
            Position = position;
            Size = size;
        }

        // --------------------------------------------------------------------------------------------
        // INPUT
        // --------------------------------------------------------------------------------------------
        public virtual void HandleInput(InputState input)
        {
            // default no-op
        }

        public void RequestFocus()
        {
            OnFocusGained?.Invoke();
            OnFocus?.Invoke();
        }

        public void ReleaseFocus()
        {
            OnFocusLost?.Invoke();
        }

        public bool HitTest(Components.PointF point)
        {
            return Bounds.Contains(point.X, point.Y);
        }

        public bool ContainsPoint(Components.PointF point)
        {
            return point.X >= Position.X &&
                   point.X <= Position.X + Size.Width &&
                   point.Y >= Position.Y &&
                   point.Y <= Position.Y + Size.Height;
        }

        // --------------------------------------------------------------------------------------------
        // CHILD MANAGEMENT
        // --------------------------------------------------------------------------------------------
        public virtual void AddChild(UIElement child)
        {
            if (child == null)
            {
                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIElement: Cannot add null child");
                return;
            }

            if (_children.Contains(child))
                return;

            if (child._parent != null && child._parent != this)
                child._parent.RemoveChild(child);

            _children.Add(child);
            child._parent = this;

            InvalidateLayout();
        }

        public virtual void RemoveChild(UIElement child)
        {
            if (child == null)
                return;

            if (_children.Remove(child))
            {
                child._parent = null;
                InvalidateLayout();
            }
        }

        // --------------------------------------------------------------------------------------------
        // LAYOUT
        // --------------------------------------------------------------------------------------------
        public virtual void InvalidateLayout()
        {
            _needsLayoutUpdate = true;

            foreach (var child in _children)
                child.InvalidateLayout();
        }

        public virtual void UpdateLayout()
        {
            foreach (var child in _children)
            {
                if (child.NeedsLayoutUpdate)
                    child.UpdateLayout();
            }

            _needsLayoutUpdate = false;
        }

        // --------------------------------------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------------------------------------
        public override void Update(float deltaTime)
        {
            foreach (var child in _children)
                child.Update(deltaTime);
        }

        // --------------------------------------------------------------------------------------------
        // RENDER (D3D11Adapter_Core)
        // --------------------------------------------------------------------------------------------
        public void Render(D3D11Adapter_Core adapter_Core)
        {
            if (!_isVisible)
                return;

            foreach (var child in _children)
                child.Render(adapter_Core);
        }

        // --------------------------------------------------------------------------------------------
        // RENDER (D3D11Adapter_Core)
        // REQUIRED BY UIElementBase
        // --------------------------------------------------------------------------------------------
        public override void Render(D3D11Adapter_Core adapter, float deltaTime)
        {
            if (!_isVisible)
                return;

            foreach (var child in _children)
                child.Render(adapter, deltaTime);
        }

        // --------------------------------------------------------------------------------------------
        // CLEANUP
        // --------------------------------------------------------------------------------------------
        public virtual void Cleanup()
        {
            try
            {
                DLogger.Log("UIElement: Cleanup performed");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIElement: Error during cleanup - {ex.Message}");
            }
        }

        internal void Draw(D3D11Adapter_Core adapter_Core)
        {
            // If there is no rendering adapter or this element is not visible, nothing to draw.
            if (adapter_Core == null || !_isVisible)
                return;

            // Draw child elements in order. Use a snapshot to avoid issues if children are
            // modified during iteration (add/remove while drawing).
            if (_children == null || _children.Count == 0)
                return;

            var snapshot = _children.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                var child = snapshot[i];
                if (child == null)
                    continue;
                if (!child.IsVisible)
                    continue;

                // Delegate drawing to the child. Child implementations are responsible for their own drawing.
                child.Draw(adapter_Core);
            }
        }

        internal void Render()
        {
            // Basic, safe rendering routine for the UI element and its children.
            try
            {
                // If the element is not visible, nothing to render.
                if (!_isVisible)
                {
                    return;
                }

                // Ensure layout is up to date before rendering.
                if (_needsLayoutUpdate)
                {
                    UpdateLayout();
                }

                // Run one-time or per-frame initialization if provided.
                Initialize?.Invoke();

                // Perform element-specific drawing logic if a Draw method exists.
                // Prefer Draw(adapter_Core) overloads in other rendering paths; here we just call Draw if available.
                try
                {
                    Draw?.Invoke(null);
                }
                catch
                {
                    // Some builds may not have Draw as a delegate; ignore if invoking fails.
                }

                // Render children elements.
                if (_children != null)
                {
                    for (int i = 0; i < _children.Count; i++)
                    {
                        var child = _children[i];
                        if (child != null)
                        {
                            child.Render();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and swallow exceptions to avoid breaking UI loop.
                DLogger.Log($"UIElement: Error during render - {ex.Message}");
            }
        }

        internal void CleanUp()
        {
            // Allow derived types to release any specialized resources first.
            try
            {
                Cleanup();
            }
            catch
            {
                // Swallow exceptions to ensure cleanup continues for other elements.
            }

            // If this element is attached to a parent, remove it from the parent.
            if (_parent != null)
            {
                try
                {
                    _parent.RemoveChild(this);
                }
                catch
                {
                    // Ignore errors from parent removal to continue cleanup.
                }

                _parent = null;
            }

            // Release focus if held.
            try
            {
                ReleaseFocus();
            }
            catch
            {
                // Ignore focus errors.
            }

            // Recursively clean up children. Copy to array to avoid collection-modified issues.
            if (_children != null && _children.Count > 0)
            {
                var childrenCopy = _children.ToArray();
                for (int i = 0; i < childrenCopy.Length; i++)
                {
                    var child = childrenCopy[i];
                    if (child == null) continue;

                    try
                    {
                        child.CleanUp();
                    }
                    catch
                    {
                        // Continue cleaning remaining children even if one fails.
                    }
                }

                _children.Clear();
            }

            // Clear event handlers and reset simple state to defaults.
            OnFocusGained = null;
            OnFocusLost = null;
            OnFocus = null;
            Initialize = null;

            _id = string.Empty;
            _isVisible = false;
            _needsLayoutUpdate = false;
            _opacity = 1f;
            _size = default;
            _position = default;
        }

        internal void AnimateOpacityTo(float v, float duration)
        {
            // If duration is non-positive or invalid, apply the value immediately.
            if (duration <= 0f || float.IsNaN(duration))
            {
                Opacity = v;
                return;
            }

            // No per-frame animation state is present on this class; as a safe fallback
            // assign the final opacity. If an animation system is added later,
            // this method can be extended to enqueue an animated transition.
            Opacity = v;
        }
    }
}

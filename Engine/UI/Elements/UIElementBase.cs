// =====================================================================================================
//  FILE: UIElementBase.cs
//  PATH: Engine/UI/Components/UIElementBase.cs
//  SUBSYSTEM: UI Framework — Core Element Base
//
//  ROLE:
//      Base type for all UI elements in the engine. Provides layout, visibility, interaction,
//      fade transitions, anchoring, and hierarchical behavior using System.Drawing primitives.
// =====================================================================================================

using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.Render.D3D11.Adapter;
using static SASZombieAssaultTD.Engine.UI.UIEnums;

namespace SASZombieAssaultTD.Engine.UI.Elements
{


    public abstract class UIElementBase
    {
        // --------------------------------------------------------------------------------------------
        // CORE FIELDS
        // --------------------------------------------------------------------------------------------
        private bool _isVisible = true;
        private bool _isEnabled = true;

        private Rectangle _bounds;

        private float _alpha = 1.0f;
        private float _targetAlpha = 1.0f;
        private float _fadeSpeed = 2.0f;
        private bool _isFading = false;

        private bool _isMouseOver = false;

        public UIElementBase? Parent { get; set; }

        public string Id { get; set; } = string.Empty;

        // --------------------------------------------------------------------------------------------
        // PROPERTIES
        // --------------------------------------------------------------------------------------------
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public int ZIndex { get; set; } = 0;

        public UIAnchor Anchor { get; set; } = UIAnchor.TopLeft;

        public float Alpha
        {
            get => _alpha;
            set
            {
                _targetAlpha = System.Math.Clamp(value, 0f, 1f);
                _isFading = System.Math.Abs(_alpha - _targetAlpha) > 0.001f;
            }
        }

        public float FadeSpeed
        {
            get => _fadeSpeed;
            set => _fadeSpeed = System.Math.Max(0.1f, value);
        }

        public bool IsFading => _isFading;

        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public Point Position
        {
            get => new Point(_bounds.X, _bounds.Y);
            set => _bounds = new Rectangle(value.X, value.Y, _bounds.Width, _bounds.Height);
        }

        // --------------------------------------------------------------------------------------------
        // EVENTS
        // --------------------------------------------------------------------------------------------
        public event EventHandler<EventArgs>? Clicked;
        public event EventHandler<EventArgs>? MouseEnter;
        public event EventHandler<EventArgs>? MouseLeave;
        public event EventHandler<EventArgs>? MousePress;
        public event EventHandler<EventArgs>? MouseRelease;

        // --------------------------------------------------------------------------------------------
        // CONSTRUCTORS
        // --------------------------------------------------------------------------------------------
        protected UIElementBase()
        {
            _bounds = new Rectangle(0, 0, 100, 50);
        }

        protected UIElementBase(Rectangle bounds)
        {
            _bounds = bounds;
        }

        // --------------------------------------------------------------------------------------------
        // RENDER — MODERN SIGNATURES
        // --------------------------------------------------------------------------------------------
        public abstract void Render(D3D11Adapter_Core adapter, float deltaTime);

        public virtual void Render(D3D11Adapter_Core context)
        {
            // Base element does not draw; children override
        }

        // --------------------------------------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------------------------------------
        public virtual void Update(float deltaTime)
        {
            UpdateFadeTransition(deltaTime);
            UpdateAnchoring();
        }

        // --------------------------------------------------------------------------------------------
        // INPUT HANDLING
        // --------------------------------------------------------------------------------------------
        public virtual bool HandleClick(Point position)
        {
            if (!_isVisible || !_isEnabled)
                return false;

            if (_bounds.Contains(position))
            {
                OnClicked();
                return true;
            }

            return false;
        }

        public virtual bool HandleMouseMove(Point position)
        {
            if (!_isVisible)
                return false;

            bool isOver = _bounds.Contains(position);
            bool wasOver = _isMouseOver;

            _isMouseOver = isOver;

            if (isOver && !wasOver)
                OnMouseEnter();
            else if (!isOver && wasOver)
                OnMouseLeave();

            return isOver;
        }

        protected virtual void OnClicked() => Clicked?.Invoke(this, EventArgs.Empty);
        protected virtual void OnMouseEnter() => MouseEnter?.Invoke(this, EventArgs.Empty);
        protected virtual void OnMouseLeave() => MouseLeave?.Invoke(this, EventArgs.Empty);
        protected virtual void OnMousePress() => MousePress?.Invoke(this, EventArgs.Empty);
        protected virtual void OnMouseRelease() => MouseRelease?.Invoke(this, EventArgs.Empty);

        // --------------------------------------------------------------------------------------------
        // SCREEN BOUNDS
        // --------------------------------------------------------------------------------------------
        public virtual Rectangle GetScreenBounds()
        {
            if (Parent != null)
            {
                var p = Parent.GetScreenBounds();
                return new Rectangle(
                    p.X + _bounds.X,
                    p.Y + _bounds.Y,
                    _bounds.Width,
                    _bounds.Height);
            }

            return _bounds;
        }

        public void SetBounds(int x, int y, int width, int height)
        {
            _bounds = new Rectangle(x, y, width, height);
        }

        public void CenterIn(Rectangle containerBounds)
        {
            int nx = containerBounds.X + (containerBounds.Width - _bounds.Width) / 2;
            int ny = containerBounds.Y + (containerBounds.Height - _bounds.Height) / 2;

            _bounds = new Rectangle(nx, ny, _bounds.Width, _bounds.Height);
        }

        // --------------------------------------------------------------------------------------------
        // VISIBILITY
        // --------------------------------------------------------------------------------------------
        public virtual void Show(bool fadeIn = true)
        {
            _isVisible = true;

            if (fadeIn)
                Alpha = 1.0f;
            else
            {
                _alpha = 1.0f;
                _targetAlpha = 1.0f;
                _isFading = false;
            }
        }

        public virtual void Hide(bool fadeOut = true)
        {
            if (fadeOut)
                Alpha = 0.0f;
            else
            {
                _isVisible = false;
                _alpha = 0.0f;
                _targetAlpha = 0.0f;
                _isFading = false;
            }
        }

        public virtual void ToggleVisibility()
        {
            if (_isVisible)
                Hide();
            else
                Show();
        }

        // --------------------------------------------------------------------------------------------
        // FADE TRANSITION
        // --------------------------------------------------------------------------------------------
        private void UpdateFadeTransition(float deltaTime)
        {
            if (System.Math.Abs(_alpha - _targetAlpha) > 0.001f)
            {
                _isFading = true;

                float dir = _targetAlpha > _alpha ? 1f : -1f;
                _alpha += dir * _fadeSpeed * deltaTime;
                _alpha = System.Math.Clamp(_alpha, 0f, 1f);

                if (_alpha <= 0f && _targetAlpha <= 0f)
                    _isVisible = false;
                else if (_alpha > 0f && _targetAlpha > 0f && !_isVisible)
                    _isVisible = true;
            }
            else
            {
                _alpha = _targetAlpha;
                _isFading = false;
            }
        }

        // --------------------------------------------------------------------------------------------
        // ANCHORING
        // --------------------------------------------------------------------------------------------
        private void UpdateAnchoring()
        {
            if (Parent == null)
                return;

            var parentBounds = Parent.Bounds;

            int localX = _bounds.X;
            int localY = _bounds.Y;

            switch (Anchor)
            {
                case UIAnchor.TopLeft:
                    localX = 0;
                    localY = 0;
                    break;

                case UIAnchor.TopRight:
                    localX = parentBounds.Width - _bounds.Width;
                    localY = 0;
                    break;

                case UIAnchor.BottomLeft:
                    localX = 0;
                    localY = parentBounds.Height - _bounds.Height;
                    break;

                case UIAnchor.BottomRight:
                    localX = parentBounds.Width - _bounds.Width;
                    localY = parentBounds.Height - _bounds.Height;
                    break;

                case UIAnchor.Center:
                case UIAnchor.MiddleCenter:
                    localX = (parentBounds.Width - _bounds.Width) / 2;
                    localY = (parentBounds.Height - _bounds.Height) / 2;
                    break;

                case UIAnchor.TopCenter:
                    localX = (parentBounds.Width - _bounds.Width) / 2;
                    localY = 0;
                    break;

                case UIAnchor.BottomCenter:
                    localX = (parentBounds.Width - _bounds.Width) / 2;
                    localY = parentBounds.Height - _bounds.Height;
                    break;

                case UIAnchor.LeftCenter:
                case UIAnchor.MiddleLeft:
                    localX = 0;
                    localY = (parentBounds.Height - _bounds.Height) / 2;
                    break;

                case UIAnchor.RightCenter:
                case UIAnchor.MiddleRight:
                    localX = parentBounds.Width - _bounds.Width;
                    localY = (parentBounds.Height - _bounds.Height) / 2;
                    break;
            }

            _bounds = new Rectangle(localX, localY, _bounds.Width, _bounds.Height);
        }

        internal void RenderWithContext(D3D11Adapter_Core context)
        {
            // Only render when visible to avoid unnecessary work
            if (!IsVisible)
                return;

            // Delegate actual rendering to the class' Render overload that accepts the context.
            // This keeps rendering responsibilities centralized in a single method.
            Render(context);
        }
    }
}

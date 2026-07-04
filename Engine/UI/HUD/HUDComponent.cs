//
using SASZombieAssaultTD.Engine.Dictionary;
using SASZombieAssaultTD.Engine.Rendering;
using SASZombieAssaultTD.Engine.VectorMath;
using System;
using System.Security.AccessControl;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.HUD
{
    ///<summary>
    ///Base class for HUD components.
    ///Provides common functionality for all HUD elements.
    ///Phase 4: UI Component Completion - Fix CS1061 errors
    ///</summary>
    public abstract class HUDComponent
    {
        protected bool _isVisible = true;
        protected Vector3 _position = Vector3.Zero;
        protected Vector3 _size = Vector3.One;
        protected bool _needsLayoutUpdate = true;
        protected Color _backgroundColor = Color.Transparent;
        protected float _opacity = 1.0f;
        protected Rectangle _bounds = Rectangle.Empty;
        private object TheType;
        private object TheMember;

        ///<summary>
        ///Gets or sets whether the component is visible.
        ///</summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnVisibilityChanged();
                }
            }
        }

        ///<summary>
        ///Gets or sets the position of the component.
        ///</summary>
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (!_position.Equals(value))
                {
                    _position = value;
                    _needsLayoutUpdate = true;
                    UpdateBounds();
                    OnPositionChanged();
                }
            }
        }

        ///<summary>
        ///Gets or sets the size of the component.
        ///</summary>
        public Vector3 Size
        {
            get => _size;
            set
            {
                if (!_size.Equals(value))
                {
                    _size = value;
                    _needsLayoutUpdate = true;
                    UpdateBounds();
                    OnSizeChanged();
                }
            }
        }

        ///<summary>
        ///Gets or sets the background color of the component.
        ///Phase 4: Add missing UI properties for CS1061 fixes
        ///</summary>
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (!_backgroundColor.Equals(value))
                {
                    _backgroundColor = value;
                    OnBackgroundColorChanged();
                }
            }
        }

        ///<summary>
        ///Gets or sets the opacity of the component (0.0 to 1.0).
        ///</summary>
        public float Opacity
        {
            get => _opacity;
            set
            {
                if (_opacity != value)
                {
                    _opacity = System.Math.Clamp(value, 0f, 1f);
                    OnOpacityChanged();
                }
            }
        }

        ///<summary>
        ///Gets or sets the bounds rectangle of the component.
        ///</summary>
        public Rectangle Bounds
        {
            get => _bounds;
            set
            {
                if (!_bounds.Equals(value))
                {
                    _bounds = value;
                    OnBoundsChanged();
                }
            }
        }

        ///<summary>
        ///Gets or sets whether the component needs layout update.
        ///</summary>
        public bool NeedsLayoutUpdate
        {
            get => _needsLayoutUpdate;
            protected set
            {
                _needsLayoutUpdate = value;
            }
        }

        ///<summary>
        ///Gets the effective background color with opacity applied.
        ///</summary>
        public Color EffectiveBackgroundColor => Color.FromArgb((byte)(_opacity * 255), (byte)_backgroundColor.R, (byte)_backgroundColor.G, (byte)_backgroundColor.B);

        ///<summary>
        ///Gets whether the component is currently within screen bounds.
        ///</summary>
        public bool IsOnScreen => _bounds.X >= 0 && _bounds.Y >= 0 &&
                                _bounds.X + _bounds.Width <= 1920 &&
                                _bounds.Y + _bounds.Height <= 1080;

        ///<summary>
        ///Gets the center position of the component.
        ///</summary>
        public Vector3 Center => new Vector3(_position.X + _size.X / 2f, _position.Y + _size.Y / 2f, _position.Z);

        ///<summary>
        ///Initializes the HUD component.
        ///</summary>
        public virtual void Initialize()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Updates the component.
        ///</summary>
        ///<param name="deltaTime">Time since last update</param>
        public virtual void Update(float deltaTime)
        {
            if (_needsLayoutUpdate)
            {
                UpdateLayout();
                _needsLayoutUpdate = false;
            }
        }

        ///<summary>
        ///Handles input for the component.
        ///</summary>
        ///<param name="inputPosition">Input position.</param>
        ///<param name="isClicked">Whether input was a click.</param>
        public virtual void HandleInput(Vector3 inputPosition, bool isClicked)
        {
            //Default implementation - override in derived classes
        }

        ///<summary>
        ///Handles input for the component with coordinate components.
        ///Adapts component-based input handling calls to the canonical HandleInput implementation.
        ///</summary>
        ///<param name="x">X coordinate.</param>
        ///<param name="y">Y coordinate.</param>
        ///<param name="isClicked">Whether input was a click.</param>
        public virtual void HandleInput(float x, float y, bool isClicked)
        {
            var inputPosition = new Vector3(x, y, 0);
            HandleInput(inputPosition, isClicked);
        }

        ///<summary>
        ///Renders the component.
        ///</summary>
        public virtual void Render()
        {
            if (!_isVisible) return;
            //Override in derived classes
        }

        ///<summary>
        ///Renders the component with a renderer.
        ///Adapts renderer-based render calls to the canonical Render implementation.
        ///</summary>
        ///<param name="renderer">The renderer to use.</param>
        public virtual void Render(object renderer)
        {
            //Default implementation calls parameterless Render
            //Derived classes can override for specific renderer handling
            Render();
        }

        ///<summary>
        ///Shows the component.
        ///</summary>
        public virtual void Show()
        {
            IsVisible = true;
        }

        ///<summary>
        ///Hides the component.
        ///</summary>
        public virtual void Hide()
        {
            IsVisible = false;
        }

        ///<summary>
        ///Updates the layout of the component.
        ///</summary>
        protected virtual void UpdateLayout()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Updates the bounds rectangle based on position and size.
        ///Phase 4: Add bounds management for CS1061 fixes
        ///</summary>
        protected virtual void UpdateBounds()
        {
            _bounds = new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y);
        }

        ///<summary>
        ///Called when visibility changes.
        ///</summary>
        protected virtual void OnVisibilityChanged()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Called when position changes.
        ///</summary>
        protected virtual void OnPositionChanged()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Called when size changes.
        ///</summary>
        protected virtual void OnSizeChanged()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Called when background color changes.
        ///Phase 4: Add missing event handlers for CS1061 fixes
        ///</summary>
        protected virtual void OnBackgroundColorChanged()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Called when opacity changes.
        ///</summary>
        protected virtual void OnOpacityChanged()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Called when bounds change.
        ///</summary>
        protected virtual void OnBoundsChanged()
        {
            //Override in derived classes
        }

        ///<summary>
        ///Sets the component's position and size simultaneously.
        ///</summary>
        ///<param name="position">New position</param>
        ///<param name="size">New size</param>
        public virtual void SetBounds(Vector3 position, Vector3 size)
        {
            _position = position;
            _size = size;
            UpdateBounds();
            _needsLayoutUpdate = true;
            OnPositionChanged();
            OnSizeChanged();
        }

        ///<summary>
        ///Animates the component's opacity over time.
        ///</summary>
        ///<param name="targetOpacity">Target opacity (0.0 to 1.0)</param>
        ///<param name="duration">Animation duration in seconds</param>
        public virtual void FadeTo(float targetOpacity, float duration)
        {
            //Override in derived classes for animation support
            Opacity = targetOpacity;
        }

        ///<summary>
        ///Animates the component's position over time.
        ///</summary>
        ///<param name="targetPosition">Target position</param>
        ///<param name="duration">Animation duration in seconds</param>
        public virtual void MoveTo(Vector3 targetPosition, float duration)
        {
            //Override in derived classes for animation support
            Position = targetPosition;
        }

        ///<summary>
        ///Checks if a point is within the component's bounds.
        ///</summary>
        ///<param name="point">Point to test</param>
        ///<returns>True if point is within bounds</returns>
        public virtual bool ContainsPoint(Vector3 point)
        {
            return point.X >= _bounds.X && point.X <= _bounds.X + _bounds.Width &&
                   point.Y >= _bounds.Y && point.Y <= _bounds.Y + _bounds.Height;
        }

        ///<summary>
        ///Gets the component's screen-space rectangle.
        ///</summary>
        ///<returns>Rectangle in screen coordinates</returns>
        public virtual Rectangle GetScreenBounds()
        {
            return _bounds;
        }

        ///<summary>
        ///Forces an immediate layout update.
        ///</summary>
        public virtual void ForceLayoutUpdate()
        {
            UpdateBounds();
            UpdateLayout();
            _needsLayoutUpdate = false;
        }

        internal void Cleanup()
        {
            NotImplementedGuard.Hit("NOT_IMPLEMENTED");

            throw new NotImplementedException();
        }
    }
}

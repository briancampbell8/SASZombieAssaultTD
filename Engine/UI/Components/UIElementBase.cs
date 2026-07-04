/*
File:    UIElementBase.cs
Purpose: Base class for all UI components in the game.
Features: Common UI properties, positioning, visibility, event handling.

Created: February 14, 2026
Notes: Provides foundation for Button, Panel, HUD, and other UI elements.
*/

using SASZombieAssaultTD.Engine.Rendering;
using System;
using System.Drawing;
using SASZombieAssaultTD.Engine.Core;
using SASZombieAssaultTD.Engine.VectorMath;
using SASZombieAssaultTD.Engine.Extensions;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI
{
    ///<summary>
    ///Represents different anchoring positions for UI elements.
    ///</summary>
    public enum UIAnchor
    {
        ///<summary>Anchor to top-left corner</summary>
        TopLeft,
        ///<summary>Anchor to top-right corner</summary>
        TopRight,
        ///<summary>Anchor to center of screen</summary>
        Center,
        ///<summary>Anchor to bottom-left corner</summary>
        BottomLeft,
        ///<summary>Anchor to bottom-right corner</summary>
        BottomRight,
        ///<summary>Anchor to top-center</summary>
        TopCenter,
        ///<summary>Anchor to bottom-center</summary>
        BottomCenter,
        ///<summary>Anchor to left-center</summary>
        LeftCenter,
        ///<summary>Anchor to right-center</summary>
        RightCenter,
        MiddleLeft,
        MiddleCenter,
        MiddleRight
    }

    ///<summary>
    ///Base class for all UI elements in the game interface.
    ///Provides common functionality for positioning, visibility, and basic interaction.
    ///P11-04-09-E: Enhanced with anchoring, visibility toggles, layering/z-index,
    ///and optional fade-in/fade-out transitions. All properties are ECS-friendly.
    ///</summary>
    public abstract class UIElementBase
    {
        private bool _isVisible = true;
        private bool _isEnabled = true;
        private Rectangle _bounds;
        private float _alpha = 1.0f;
        private float _targetAlpha = 1.0f;
        private float _fadeSpeed = 2.0f;
        private bool _isFading = false;
        private bool _isMouseOver = false;
        internal System.Numerics.Matrix3x2 Transform;

        ///<summary>
        ///Gets or sets the unique identifier for this UI element.
        ///</summary>
        public string Id { get; set; } = string.Empty;

        ///<summary>
        ///Gets or sets the position of the UI element.
        ///</summary>
        public Point Position
        {
            get => new Point((int)_bounds.X, (int)_bounds.Y);
            set => _bounds = Rectangle.FromPositionAndSize(value.X, value.Y, _bounds.Width, _bounds.Height);
        }

        ///<summary>
        ///Gets or sets the size of the UI element.
        ///</summary>
        public Core.Size Size
        {
            get => _bounds.Size;
            set => _bounds = new Rectangle(_bounds.X, _bounds.Y, value.Width, value.Height);
        }

        ///<summary>
        ///Gets or sets the bounds rectangle of the UI element.
        ///</summary>
        public Rectangle Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        ///<summary>
        ///Gets or sets whether the UI element is visible.
        ///</summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        ///<summary>
        ///Gets or sets whether the UI element is enabled for interaction.
        ///</summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        ///<summary>
        ///Gets or sets the parent UI element.
        ///</summary>
        public UIElementBase? Parent { get; set; }

        ///<summary>
        ///Gets or sets the z-order depth for rendering.
        ///Higher values render on top of lower values.
        ///P11-04-09-E: Enhanced layering support for UI elements.
        ///</summary>
        public int ZIndex { get; set; } = 0;

        ///<summary>
        ///Gets or sets the anchoring position for this UI element.
        ///P11-04-09-E: Supports various anchoring positions for automatic positioning.
        ///</summary>
        public UIAnchor Anchor { get; set; } = UIAnchor.TopLeft;

        ///<summary>
        ///Gets or sets the alpha transparency (0.0 to 1.0).
        ///P11-04-09-E: Supports fade-in/fade-out transitions.
        ///</summary>
        public float Alpha
        {
            get => _alpha;
            set => _targetAlpha = System.Math.Clamp(value, 0f, 1f);
        }

        ///<summary>
        ///Gets or sets the fade transition speed in alpha units per second.
        ///P11-04-09-E: Controls fade-in/fade-out animation speed.
        ///</summary>
        public float FadeSpeed
        {
            get => _fadeSpeed;
            set => _fadeSpeed = System.Math.Max(0.1f, value);
        }

        ///<summary>
        ///Gets whether the element is currently fading.
        ///P11-04-09-E: Indicates if fade transition is in progress.
        ///</summary>
        public bool IsFading => _isFading;

        ///<summary>
        ///Event triggered when the UI element is clicked.
        ///</summary>
        public event EventHandler<EventArgs>? Clicked;

        ///<summary>
        ///Event triggered when the mouse enters the UI element bounds.
        ///</summary>
        public event EventHandler<EventArgs>? MouseEnter;

        ///<summary>
        ///Event triggered when mouse leaves UI element bounds.
        ///</summary>
        public event EventHandler<EventArgs>? MouseLeave;

        ///<summary>
        ///Event triggered when mouse button is pressed on UI element.
        ///</summary>
        public event EventHandler<EventArgs>? MousePress;

        ///<summary>
        ///Event triggered when mouse button is released on UI element.
        ///</summary>
        public event EventHandler<EventArgs>? MouseRelease;

        ///<summary>
        ///Event triggered when mouse exits UI element bounds.
        ///</summary>
        public event EventHandler<EventArgs>? MouseExit;

        ///<summary>
        ///Initializes a new instance of the UIElementBase class.
        ///</summary>
        protected UIElementBase()
        {
            _bounds = new Rectangle(0, 0, 100, 50);
        }

        ///<summary>
        ///Initializes a new instance of the UIElementBase class with specified bounds.
        ///</summary>
        ///<param name="bounds">The bounds rectangle for the UI element.</param>
        protected UIElementBase(Rectangle bounds)
        {
            _bounds = bounds;
        }

        ///<summary>
        ///Renders the UI element.
        ///Must be implemented by derived classes.
        ///</summary>
        ///<param name="context">The rendering context.</param>
        public abstract void Render(IRenderContext context);

        ///<summary>
        ///Updates the UI element state.
        ///P11-04-09-E: Enhanced to handle fade transitions and anchoring updates.
        ///</summary>
        ///<param name="deltaTime">Time elapsed since last update.</param>
        public virtual void Update(float deltaTime)
        {
            UpdateFadeTransition(deltaTime);
            UpdateAnchoring();
        }

        ///<summary>
        ///Handles mouse click events.
        ///</summary>
        ///<param name="position">The mouse click position.</param>
        ///<returns>True if the click was handled, false otherwise.</returns>
        public virtual bool HandleClick(Point position)
        {
            if (!_isVisible || !_isEnabled)
                return false;

            if (_bounds.Contains(new Vector3(position.X, position.Y, 0f)))
            {
                OnClicked();
                return true;
            }

            return false;
        }

        ///<summary>
        ///Handles mouse move events.
        ///</summary>
        ///<param name="position">The current mouse position.</param>
        ///<returns>True if the mouse is over this element, false otherwise.</returns>
        public virtual bool HandleMouseMove(Point position)
        {
            if (!_isVisible)
                return false;

            bool isOver = _bounds.Contains(new Vector3(position.X, position.Y, 0f));

            //Mouse enter/leave tracking
            bool wasOver = _isMouseOver;
            _isMouseOver = isOver;

            if (isOver && !wasOver)
            {
                OnMouseEnter();
            }
            else if (!isOver && wasOver)
            {
                OnMouseLeave();
            }

            return isOver;
        }

        ///<summary>
        ///Triggers the Clicked event.
        ///</summary>
        protected virtual void OnClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        ///<summary>
        ///Triggers the MouseEnter event.
        ///</summary>
        protected virtual void OnMouseEnter()
        {
            MouseEnter?.Invoke(this, EventArgs.Empty);
        }

        ///<summary>
        ///Triggers the MouseLeave event.
        ///</summary>
        protected virtual void OnMouseLeave()
        {
            MouseLeave?.Invoke(this, EventArgs.Empty);
        }

        ///<summary>
        ///Triggers the MouseExit event.
        ///</summary>
        protected virtual void OnMouseExit()
        {
            MouseExit?.Invoke(this, EventArgs.Empty);
        }

        ///<summary>
        ///Triggers the MousePress event.
        ///</summary>
        protected virtual void OnMousePress()
        {
            MousePress?.Invoke(this, EventArgs.Empty);
        }

        ///<summary>
        ///Triggers the MouseRelease event.
        ///</summary>
        protected virtual void OnMouseRelease()
        {
            MouseRelease?.Invoke(this, EventArgs.Empty);
        }

        ///<summary>
        ///Gets the screen-space bounds of this UI element, accounting for parent transforms.
        ///</summary>
        ///<returns>The screen-space bounds rectangle.</returns>
        public virtual Rectangle GetScreenBounds()
        {
            if (Parent != null)
            {
                var parentBounds = Parent.GetScreenBounds();
                return new Rectangle(
                parentBounds.X + _bounds.X,
                parentBounds.Y + _bounds.Y,
                _bounds.Width,
                _bounds.Height);
            }

            return _bounds;
        }

        ///<summary>
        ///Sets the bounds of the UI element.
        ///</summary>
        ///<param name="x">The x-coordinate.</param>
        ///<param name="y">The y-coordinate.</param>
        ///<param name="width">The width.</param>
        ///<param name="height">The height.</param>
        public void SetBounds(int x, int y, int width, int height)
        {
            _bounds = new Rectangle(x, y, width, height);
        }

        ///<summary>
        ///Centers the UI element within the specified bounds.
        ///</summary>
        ///<param name="containerBounds">The container bounds to center within.</param>
        public void CenterIn(Rectangle containerBounds)
        {
            float nx = containerBounds.X + (containerBounds.Width - _bounds.Width) / 2;
            float ny = containerBounds.Y + (containerBounds.Height - _bounds.Height) / 2;
            _bounds = new Rectangle(nx, ny, _bounds.Width, _bounds.Height);
        }

        ///<summary>
        ///Shows the UI element with optional fade-in effect.
        ///P11-04-09-E: Enhanced visibility control with fade transitions.
        ///</summary>
        ///<param name="fadeIn">Whether to fade in the element (default: true)</param>
        public virtual void Show(bool fadeIn = true)
        {
            _isVisible = true;
            if (fadeIn)
            {
                Alpha = 1.0f;
            }
            else
            {
                _alpha = 1.0f;
                _targetAlpha = 1.0f;
                _isFading = false;
            }
        }

        ///<summary>
        ///Hides the UI element with optional fade-out effect.
        ///P11-04-09-E: Enhanced visibility control with fade transitions.
        ///</summary>
        ///<param name="fadeOut">Whether to fade out the element (default: true)</param>
        public virtual void Hide(bool fadeOut = true)
        {
            if (fadeOut)
            {
                Alpha = 0.0f;
            }
            else
            {
                _isVisible = false;
                _alpha = 0.0f;
                _targetAlpha = 0.0f;
                _isFading = false;
            }
        }

        ///<summary>
        ///Toggles the visibility of the UI element.
        ///P11-04-09-E: Simple visibility toggle functionality.
        ///</summary>
        public virtual void ToggleVisibility()
        {
            if (_isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        ///<summary>
        ///Updates fade transition for the UI element.
        ///P11-04-09-E: Handles smooth fade-in/fade-out animations.
        ///</summary>
        private void UpdateFadeTransition(float deltaTime)
        {
            if (System.Math.Abs(_alpha - _targetAlpha) > 0.001f)
            {
                _isFading = true;
                float fadeDirection = _targetAlpha > _alpha ? 1f : -1f;
                _alpha += fadeDirection * _fadeSpeed * deltaTime;
                _alpha = System.Math.Clamp(_alpha, 0f, 1f);

                //Hide element when fully faded out
                if (_alpha <= 0f && _targetAlpha <= 0f)
                {
                    _isVisible = false;
                }
                //Show element when starting to fade in
                else if (_alpha > 0f && _targetAlpha > 0f && !_isVisible)
                {
                    _isVisible = true;
                }
            }
            else
            {
                _isFading = false;
            }
        }

        ///<summary>
        ///Updates element position based on anchoring.
        ///P11-04-09-E: Handles automatic positioning based on anchor settings.
        ///</summary>
        private void UpdateAnchoring()
        {
            //This would typically use the screen dimensions from a render context
            //For now, this is a placeholder that derived classes can override
            //to implement specific anchoring behavior
        }

        ///<summary>
        ///Gets the effective alpha value for rendering.
        ///P11-04-09-E: Combines element alpha with parent alpha for hierarchical transparency.
        ///</summary>
        protected float GetEffectiveAlpha()
        {
            if (Parent != null && Parent is UIElementBase parentElement)
            {
                return _alpha * parentElement.GetEffectiveAlpha();
            }
            return _alpha;
        }
    }
}





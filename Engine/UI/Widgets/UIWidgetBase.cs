//
using System;

namespace SASZombieAssaultTD.Engine.UI.Widgets
{
    ///<summary>
    ///Shared widget utilities for all UI widgets
    ///P80-04-04: UIWidgetBase defining shared widget utilities for all UI widgets
    ///</summary>
    public abstract class UIWidgetBase : UIElement
    {
        private bool _isHovered = false;
        private bool _isPressed = false;
        private bool _isDisabled = false;
        private string _tooltip = string.Empty;

        ///<summary>
        ///Gets whether the widget is currently hovered
        ///</summary>
        public bool IsHovered => _isHovered;

        ///<summary>
        ///Gets whether the widget is currently pressed
        ///</summary>
        public bool IsPressed => _isPressed;

        ///<summary>
        ///Gets whether the widget is disabled
        ///</summary>
        public bool IsDisabled => _isDisabled;

        ///<summary>
        ///Gets or sets the tooltip text
        ///</summary>
        public string Tooltip
        {
            get => _tooltip;
            set
            {
                if (_tooltip != value)
                {
                    _tooltip = value ?? string.Empty;
                    System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Tooltip set to '{_tooltip}'");
                }
            }
        }

        ///<summary>
        ///Initializes a new UIWidgetBase
        ///</summary>
        protected UIWidgetBase()
        {
            System.Diagnostics.Debug.WriteLine("UIWidgetBase: Created new widget base");
        }

        ///<summary>
        ///Called when the mouse enters the widget
        ///</summary>
        public override void OnMouseEnter()
        {
            try
            {
                _isHovered = true;
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Mouse entered widget");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error in OnMouseEnter - {ex.Message}");
            }
        }

        ///<summary>
        ///Called when the mouse leaves the widget
        ///</summary>
        public override void OnMouseExit()
        {
            try
            {
                _isHovered = false;
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Mouse exited widget");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error in OnMouseExit - {ex.Message}");
            }
        }

        ///<summary>
        ///Called when the widget is pressed
        ///</summary>
        protected virtual void OnPressed()
        {
            try
            {
                _isPressed = true;
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Widget pressed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error in OnPressed - {ex.Message}");
            }
        }

        ///<summary>
        ///Called when the widget is released
        ///</summary>
        protected virtual void OnReleased()
        {
            try
            {
                _isPressed = false;
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Widget released");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error in OnReleased - {ex.Message}");
            }
        }

        ///<summary>
        ///Sets the widget disabled state
        ///</summary>
        ///<param name="disabled">Whether the widget should be disabled</param>
        public void SetDisabled(bool disabled)
        {
            try
            {
                if (_isDisabled != disabled)
                {
                    _isDisabled = disabled;
                    System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Widget disabled state set to {disabled}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error setting disabled state - {ex.Message}");
            }
        }

        ///<summary>
        ///Updates the widget state
        ///</summary>
        ///<param name="deltaTime">Time since last update in seconds</param>
        public override void Update(float deltaTime)
        {
            try
            {
                base.Update(deltaTime);

                //Reset hover state (will be updated by input system)
                if (_isHovered && !_isDisabled)
                {
                    //Update hover animation or effects
                    UpdateHoverState(deltaTime);
                }

                //Update pressed state animation
                if (_isPressed && !_isDisabled)
                {
                    UpdatePressedState(deltaTime);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error during update - {ex.Message}");
            }
        }

        ///<summary>
        ///Renders the widget
        ///</summary>
        public override void Render()
        {
            try
            {
                if (_isDisabled)
                {
                    //Render disabled state
                    RenderDisabledState();
                }
                else
                {
                    base.Render();

                    //Render hover state
                    if (_isHovered)
                    {
                        RenderHoverState();
                    }

                    //Render pressed state
                    if (_isPressed)
                    {
                        RenderPressedState();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error during render - {ex.Message}");
            }
        }

        ///<summary>
        ///Updates hover state (placeholder implementation)
        ///</summary>
        ///<param name="deltaTime">Time since last update in seconds</param>
        protected virtual void UpdateHoverState(float deltaTime)
        {
            //Override in derived classes for hover animations
        }

        ///<summary>
        ///Updates pressed state (placeholder implementation)
        ///</summary>
        ///<param name="deltaTime">Time since last update in seconds</param>
        protected virtual void UpdatePressedState(float deltaTime)
        {
            //Override in derived classes for press animations
        }

        ///<summary>
        ///Renders disabled state (placeholder implementation)
        ///</summary>
        protected virtual void RenderDisabledState()
        {
            //Override in derived classes for disabled appearance
        }

        ///<summary>
        ///Renders hover state (placeholder implementation)
        ///</summary>
        protected virtual void RenderHoverState()
        {
            //Override in derived classes for hover appearance
        }

        ///<summary>
        ///Renders pressed state (placeholder implementation)
        ///</summary>
        protected virtual void RenderPressedState()
        {
            //Override in derived classes for pressed appearance
        }

        ///<summary>
        ///Resets the widget state
        ///</summary>
        public void ResetState()
        {
            try
            {
                _isHovered = false;
                _isPressed = false;
                _isDisabled = false;
                _tooltip = string.Empty;

                System.Diagnostics.Debug.WriteLine("UIWidgetBase: Widget state reset");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UIWidgetBase: Error resetting state - {ex.Message}");
            }
        }
    }
}





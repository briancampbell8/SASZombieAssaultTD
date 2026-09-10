// ====================================================================================================
//  FILE: UIWidgetBase.cs
//  PATH: ./Engine/UI/Widgets/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide OnMouseEnter() behavior for the UI subsystem.
//      - Provide OnMouseExit() behavior for the UI subsystem.
//      - Provide OnPressed() behavior for the UI subsystem.
//      - Provide OnReleased() behavior for the UI subsystem.
//      - Provide SetDisabled() behavior for the UI subsystem.
//      - Provide Update() behavior for the UI subsystem.
//      - Provide Render() behavior for the UI subsystem.
//      - Provide UpdateHoverState() behavior for the UI subsystem.
//      - Provide UpdatePressedState() behavior for the UI subsystem.
//      - Provide RenderDisabledState() behavior for the UI subsystem.
//      - Provide RenderHoverState() behavior for the UI subsystem.
//      - Provide RenderPressedState() behavior for the UI subsystem.
//      - Provide ResetState() behavior for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//
using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.UI.Elements
{
    /// <summary>
    /// Shared widget utilities for all UI widgets P80-04-04: UIWidgetBase defining shared widget utilities for all UI
    /// widgets
    /// </summary>
    public abstract class UIWidgetBase : UIElement
    {
        private bool _isHovered = false;
        private bool _isPressed = false;
        private bool _isDisabled = false;
        private string _tooltip = string.Empty;

        /// <summary>
        /// Gets whether the widget is currently hovered
        /// </summary>
        public bool IsHovered => _isHovered;

        /// <summary>
        /// Gets whether the widget is currently pressed
        /// </summary>
        public bool IsPressed => _isPressed;

        /// <summary>
        /// Gets whether the widget is disabled
        /// </summary>
        public bool IsDisabled => _isDisabled;

        /// <summary>
        /// Gets or sets the tooltip text
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set
            {
                if (_tooltip != value)
                {
                    _tooltip = value ?? string.Empty;
                    DLogger.Log($"UIWidgetBase: Tooltip set to '{_tooltip}'");
                }
            }
        }

        /// <summary>
        /// Initializes a new UIWidgetBase
        /// </summary>
        protected UIWidgetBase() => DLogger.Log(LogSubsystems.ResourcesPipeline, "UIWidgetBase: Created new widget base");

        /// <summary>
        /// Called when the mouse enters the widget
        /// </summary>
        public void OnMouseEnter() // Fixed CS0507: Removed 'override' keyword
        {
            try
            {
                _isHovered = true;
                DLogger.Log($"UIWidgetBase: Mouse entered widget");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIWidgetBase: Error in OnMouseEnter - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when the mouse leaves the widget
        /// </summary>
        public void OnMouseExit() // Fixed CS0115: Removed 'override' keyword
        {
            try
            {
                _isHovered = false;
                DLogger.Log($"UIWidgetBase: Mouse exited widget");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIWidgetBase: Error in OnMouseExit - {ex.Message}");
            }
        }


        /// <summary>
        /// Called when the widget is pressed
        /// </summary>
        protected virtual void OnPressed()
        {
            try
            {
                _isPressed = true;
                DLogger.Log($"UIWidgetBase: Widget pressed");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIWidgetBase: Error in OnPressed - {ex.Message}");
            }
        }

        /// <summary>
        /// Called when the widget is released
        /// </summary>
        protected virtual void OnReleased()
        {
            try
            {
                _isPressed = false;
                DLogger.Log($"UIWidgetBase: Widget released");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIWidgetBase: Error in OnReleased - {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the widget disabled state
        /// </summary>
        /// <param name="disabled">Whether the widget should be disabled</param>
        public void SetDisabled(bool disabled)
        {
            try
            {
                if (_isDisabled != disabled)
                {
                    _isDisabled = disabled;
                    DLogger.Log($"UIWidgetBase: Widget disabled state set to {disabled}");
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIWidgetBase: Error setting disabled state - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the widget state
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
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
                DLogger.Log($"UIWidgetBase: Error during update - {ex.Message}");
            }
        }

        /// <summary>
        /// Renders the widget
        /// </summary>
        public void Render()
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
                DLogger.Log($"UIWidgetBase: Error during render - {ex.Message}");
            }
        }

        /// <summary>
        /// Updates hover state (placeholder implementation)
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        protected virtual void UpdateHoverState(float deltaTime)
        {
            //Override in derived classes for hover animations
        }

        /// <summary>
        /// Updates pressed state (placeholder implementation)
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        protected virtual void UpdatePressedState(float deltaTime)
        {
            //Override in derived classes for press animations
        }

        /// <summary>
        /// Renders disabled state (placeholder implementation)
        /// </summary>
        protected virtual void RenderDisabledState()
        {
            //Override in derived classes for disabled appearance
        }

        /// <summary>
        /// Renders hover state (placeholder implementation)
        /// </summary>
        protected virtual void RenderHoverState()
        {
            //Override in derived classes for hover appearance
        }

        /// <summary>
        /// Renders pressed state (placeholder implementation)
        /// </summary>
        protected virtual void RenderPressedState()
        {
            //Override in derived classes for pressed appearance
        }

        /// <summary>
        /// Resets the widget state
        /// </summary>
        public void ResetState()
        {
            try
            {
                _isHovered = false;
                _isPressed = false;
                _isDisabled = false;
                _tooltip = string.Empty;

                DLogger.Log(LogSubsystems.ResourcesPipeline, "UIWidgetBase: Widget state reset");
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIWidgetBase: Error resetting state - {ex.Message}");
            }
        }
    }
}

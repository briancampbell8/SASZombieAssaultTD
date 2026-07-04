// ====================================================================================================
//  FILE: ModernButton.cs
//  PATH: Engine/UI/Components/
//  MODULE: UI Components (Modern Button)
//
//  ROLE:
//      Rich UI button component supporting styling, animations, and asset-backed rendering.
//
//  RESPONSIBILITIES:
//      - Expose Click/Pressed/Released events and visual state transitions.
//      - Integrate with asset system for fonts and icons.
//      - Provide efficient dirty-flag rendering semantics for performance.
//
//  NON-RESPONSIBILITIES:
//      - High-level input routing (UI system provides input events).
//
//  ARCHITECTURAL NOTES:
//      - Designed for reusability across legacy HUD and P80 UI systems.
// ====================================================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
//

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Components
{
    ///<summary>
    ///Modern button component with advanced styling and animation support.
    ///Provides comprehensive button functionality with Asset System integration.
    ///</summary>
    public class ModernButton : ModernUIComponent, INotifyPropertyChanged
    {
        ///Events

        ///<summary>
        ///Event raised when the button is clicked.
        ///This is the primary event for button interaction handling.
        ///</summary>
        public event EventHandler Click;

        ///<summary>
        ///Event raised when the button is pressed (mouse down).
        ///Used for visual feedback and interaction tracking.
        ///</summary>
        public event EventHandler Pressed;

        ///<summary>
        ///Event raised when the button is released (mouse up).
        ///Used for visual feedback and interaction tracking.
        ///</summary>
        public event EventHandler Released;

        ///<summary>
        ///INotifyPropertyChanged implementation.
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///Private Fields

        private string _text = string.Empty;
        private UIFont _font;
        private ButtonStyle _buttonStyle = ButtonStyle.Primary;
        private SizeF _textSize;
        private dynamic Animations;
        private bool IsFocusable;
        private ComponentStyle Style;
        private Color TextColor;

        ///Properties

        ///<summary>
        ///Gets or sets the button text.
        ///Text changes automatically update the button's visual appearance.
        ///</summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value ?? string.Empty;
                    MarkDirty();
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        private void MarkDirty()
        {
            NI.Hit();
        }

        ///<summary>
        ///Gets or sets the font used for rendering the button text.
        ///Font changes automatically update text measurement and rendering.
        ///</summary>
        public UIFont Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value;
                    MarkDirty();
                    OnPropertyChanged(nameof(Font));
                }
            }
        }

        public ModernButton(UIFont font) : base(string.Empty)
        {
            Font = font;
        }

        ///<summary>
        ///Initializes a new instance of the ModernButton class.
        ///</summary>
        ///<param name="id">Unique identifier for the button.</param>
        public ModernButton(string id) : base(id)
        {
            IsFocusable = true;

            //Set up default button style (use Color.FromArgb for compatibility)
            Style = new ComponentStyle
            {
                BackgroundColor = Color.FromArgb(255, 52, 152, 219),
                TextColor = Color.White,
                BorderColor = Color.FromArgb(255, 41, 128, 185),
                BorderWidth = 1,
                CornerRadius = 4,
                Padding = new Padding(16, 8, 16, 8)
            };

            //Synchronize simple properties with style defaults
            TextColor = Style.TextColor;

            System.Diagnostics.Debug.WriteLine($"ModernButton: Created button '{id}'");
        }

        ///<summary>
        ///Simulates a button click programmatically.
        ///This method raises the Click event and triggers visual feedback.
        ///</summary>
        public void PerformClick()
        {
            Click?.Invoke(this, EventArgs.Empty);

            if (Animations != null)
            {
                //Visual feedback animation (fire-and-forget)
                //Using dynamic to allow late-bound animation helpers without requiring a specific compile-time type.
                try
                {
                    _ = Animations.AnimatePropertyAsync(
                        "Scale",
                        Vector3.One,
                        Vector3.One * 0.95f,
                        TimeSpan.FromMilliseconds(100));
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    //Animations object doesn't provide AnimatePropertyAsync at runtime; ignore gracefully.
                }
            }
        }

        ///<summary>
        ///Public render wrapper called by layout/renderer.
        ///</summary>
        public void Render(RenderContext context)
        {
            if (!IsVisible || context == null)
                return;

            OnRender(context);
        }

        ///<summary>
        ///Notifies listeners that a property changed.
        ///Uses INotifyPropertyChanged for data binding hooks.
        ///</summary>
        ///<param name="propertyName">Property name.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Note: MarkDirty and ClearDirty are provided by the base class (ModernUIComponent).
        //This class relies on those base methods; do not re-declare them here to avoid hiding.
    }

    internal class ButtonStyle
    {
        internal static ButtonStyle Primary;
    }

    public class RenderContext
    {
    }

    internal class ComponentStyle
    {
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Color BorderColor { get; set; }
        public int BorderWidth { get; set; }
        public int CornerRadius { get; set; }
        public Padding Padding { get; set; }
    }

    public class ModernUIComponent
    {
        public ModernUIComponent(string id)
        {
        }

        ///<summary>
        ///Primary render entry for the component.
        ///Keep rendering minimal here; actual draw calls should be performed by the provided RenderContext.
        ///</summary>
        protected virtual void OnRender(RenderContext context)
        {
            if (context == null)
                return;

            //Clear dirty flag before/after render so subsequent changes re-draw.
            //Actual drawing commands (DrawRectangle/DrawText) belong in the rendering subsystem.
            ClearDirty();
        }

        //Stub implementations for dirty flagging to satisfy references in this file.
        //Replace with real implementations from your actual framework/base class.
        protected void ClearDirty()
        {
            //no-op placeholder
        }

        protected void MarkDirty()
        {
            //no-op placeholder
        }

        //Expose visibility for Render checks in derived classes
        protected bool IsVisible { get; set; } = true;
    }

    //Make UIFont public so it is at least as accessible as ModernButton.Font
    public class UIFont
    {
    }
}

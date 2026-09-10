//============================================================================
// File Path: Engine/UI/Widgets/UIText.cs
// File: UIText.cs
// Program: UIText
// Subsystem: UI / Legacy Widget System
//
// Purpose:
//     Represents a text widget with string content, font reference, color
//     fields, alignment, and basic layout behavior. Provides placeholder
//     rendering and layout estimation until integrated with the modern
//     text‑rendering pipeline.
//
// Architectural Role:
//     - Acts as a legacy UI widget pending migration to the unified UIState
//       + UIRenderable pipeline
//     - Stores text, font, color, alignment, and layout metadata
//     - Emits layout invalidation events when properties change
//     - Provides placeholder rendering hooks for derived classes
//
// Diagnostics:
//     - Uses System.Diagnostics.Debug.WriteLine for legacy forensic output
//     - No silent failures; all property changes and render actions are logged
//     - All exceptions during update/render are surfaced deterministically
//
// Modernization Notes:
//     - Color pipeline will be upgraded to Engine.Core.Color
//     - Alignment will be migrated to Engine.Core.TextAlignment
//     - SizeF will be replaced with Engine.Core.Size
//     - Rendering will be replaced with unified text‑rendering commands
//     - Widget system scheduled for deprecation in favor of UIStateBuilder
//
// Notes:
//     - Current rendering is placeholder only (rectangle + text indicator)
//     - Text measurement is approximate and not font‑accurate
//     - Intended for removal once modern UI pipeline is fully deployed
//============================================================================

using System;
using SASZombieAssaultTD.Engine.Diagnostics;
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.UI.Elements
{
    ///<summary>
    ///A UI text element with string content, font reference, and color fields
    ///P80-04-01: UIText providing a UI text element with string content, font reference, and color fields
    ///</summary>
    public class UIText : UIWidgetBase
    {
        private string _text = string.Empty;
        private string _font = string.Empty;
        private System.Drawing.Color _color = System.Drawing.Color.White;
        private float _fontSize = 12.0f;
        private bool _wordWrap = false;
        private System.Drawing.ContentAlignment _alignment = System.Drawing.ContentAlignment.TopLeft;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value ?? string.Empty;
                    InvalidateLayout();
                    DLogger.Log($"UIText: Text set to '{_text}'");
                }
            }
        }

        public string Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value ?? string.Empty;
                    InvalidateLayout();
                    DLogger.Log($"UIText: Font set to '{_font}'");
                }
            }
        }

        public System.Drawing.Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    DLogger.Log($"UIText: Color set to {_color}");
                }
            }
        }

        public float FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = System.Math.Max(1.0f, value);
                    InvalidateLayout();
                    DLogger.Log($"UIText: Font size set to {_fontSize}");
                }
            }
        }

        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                if (_wordWrap != value)
                {
                    _wordWrap = value;
                    InvalidateLayout();
                    DLogger.Log($"UIText: Word wrap set to {value}");
                }
            }
        }

        public System.Drawing.ContentAlignment Alignment
        {
            get => _alignment;
            set
            {
                if (_alignment != value)
                {
                    _alignment = value;
                    InvalidateLayout();
                    DLogger.Log($"UIText: Alignment set to {value}");
                }
            }
        }

        public UIText() : base() => DLogger.Log(LogSubsystems.ResourcesPipeline, "UIText: Created new text element");

        public UIText(string text) : this()
        {
            Text = text;
            DLogger.Log($"UIText: Created text element with '{text}'");
        }

        private System.Drawing.SizeF CalculateTextSize()
        {
            try
            {
                var charCount = _text?.Length ?? 0;
                var width = charCount * _fontSize * 0.6f;
                var height = _fontSize * 1.2f;

                return new System.Drawing.SizeF(width, height);
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIText: Error calculating text size - {ex.Message}");
                return new System.Drawing.SizeF(100, 20);
            }
        }

        public override void Update(float deltaTime)
        {
            try
            {
                base.Update(deltaTime);
                UpdateTextAnimation(deltaTime);
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIText: Error during update - {ex.Message}");
            }
        }

        public void Render()
        {
            try
            {
                if (string.IsNullOrEmpty(_text) || !IsVisible)
                    return;

                var textSize = CalculateTextSize();

                if (Size != textSize)
                {
                    Size = textSize;
                }

                RenderBackground();
                RenderTextContent();

                if (IsHovered)
                {
                    RenderBorder();
                }
            }
            catch (Exception ex)
            {
                DLogger.Log($"UIText: Error during render - {ex.Message}");
            }
        }

        protected virtual void UpdateTextAnimation(float deltaTime) { }

        protected virtual void RenderBackground() { }

        protected virtual void RenderTextContent()
        {
            DLogger.Log($"UIText: Rendering text '{_text}' at {AbsolutePosition}");
        }

        protected virtual void RenderBorder()
        {
            DLogger.Log($"UIText: Rendering border for text at {AbsolutePosition}");
        }
    }
}

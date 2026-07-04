using System;
using System.Numerics;
using SASZombieAssaultTD.Engine.Diagnostics;
using SASZombieAssaultTD.Engine.Rendering;
namespace SASZombieAssaultTD.Engine.UI.Components

{
    ///<summary>
    ///Label UI element for displaying text.
    ///P20-06-04: Implements text rendering with font loading support.
    ///</summary>
    public class Label : UIElement
    {
        private string _text;
        private Font _font;
        private Color _textColor;
        private Color _backgroundColor;
        private bool _drawBackground;
        private TextAlignment _alignment;
        private float _fontSize;
        private bool _wordWrap;
        private int _maxWidth;

        ///<summary>
        ///Gets or sets the text to display.
        ///</summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value ?? string.Empty;
                    InvalidateTextCache();
                }
            }
        }

        ///<summary>
        ///Gets or sets the font to use for rendering.
        ///</summary>
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets or sets the text color.
        ///</summary>
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets or sets the background color.
        ///</summary>
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets or sets whether to draw background.
        ///</summary>
        public bool DrawBackground
        {
            get => _drawBackground;
            set
            {
                _drawBackground = value;
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets or sets the text alignment.
        ///</summary>
        public TextAlignment Alignment
        {
            get => _alignment;
            set
            {
                _alignment = value;
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets or sets the font size.
        ///</summary>
        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = System.Math.Max(1, value);
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets or sets whether to wrap text.
        ///</summary>
        public bool WordWrap
        {
            get => _wordWrap;
            set
            {
                _wordWrap = value;
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets or sets the maximum width for text wrapping.
        ///</summary>
        public int MaxWidth
        {
            get => _maxWidth;
            set
            {
                _maxWidth = System.Math.Max(0, value);
                InvalidateTextCache();
            }
        }

        ///<summary>
        ///Gets the measured text size.
        ///</summary>
        public Vector3 TextSize { get; private set; }

        ///<summary>
        ///Event fired when text changes.
        ///</summary>
        public event Action<string> OnTextChanged;

        ///<summary>
        ///Event fired when font changes.
        ///</summary>
        public event Action<Font> OnFontChanged;

        ///<summary>
        ///Event fired when text color changes.
        ///</summary>
        public event Action<Color> OnTextColorChanged;

        ///<summary>
        ///Initializes a new label.
        ///</summary>
        ///<param name="id">Unique identifier for the label.</param>
        ///<param name="text">Initial text to display.</param>
        ///<param name="font">Initial font to use.</param>
        ///<param name="position">Initial position.</param>
        ///<param name="textColor">Initial text color.</param>
        public Label(string? id = null, string? text = null, Font? font = null,
        Vector3? position = null, Color? textColor = null)
        : base()
        {
            _text = text ?? string.Empty;
            _font = font ?? LoadDefaultFont();
            _textColor = textColor ?? Color.White;
            _backgroundColor = Color.Transparent;
            _drawBackground = false;
            _alignment = TextAlignment.Left;
            _fontSize = 16;
            _wordWrap = false;
            _maxWidth = 0;

            InvalidateTextCache();
            //TODO: SizeF type not found - using Vector3 instead
            Size = new System.Drawing.SizeF(TextSize.X, TextSize.Y);

            DLogger.Log(LogSubsystems.UI, LogLevel.Debug, $"Label: Created '{Id}' with text '{_text}'");
        }

        ///<summary>
        ///Sets the text and updates the measured size.
        ///</summary>
        ///<param name="text">The new text to display.</param>
        public void SetText(string text)
        {
            Text = text;
        }

        ///<summary>
        ///Measures text dimensions with current settings.
        ///</summary>
        ///<param name="text">The text to measure.</param>
        ///<returns>The measured text size.</returns>
        public Vector3 MeasureText(string? text = null)
        {
            var textToMeasure = text ?? _text;

            //In a real implementation, this would use the font to measure text
            //For now, we'll estimate based on character count and font size
            var charCount = textToMeasure?.Length ?? _text?.Length ?? 0;
            var estimatedWidth = charCount * (int)(_fontSize * 0.6); //Approximate character width
            var estimatedHeight = _fontSize;

            return new Vector3(estimatedWidth, estimatedHeight, 0f);
        }

        ///<summary>
        ///Renders the label.
        ///</summary>
        ///<param name="renderer">The renderer to use.</param>
        public virtual void Render(Renderer renderer)
        {
            //TODO: base.Render() doesn't take parameters
            //base.Render(renderer);

            if (string.IsNullOrEmpty(_text) || _font == null)
                return;

            //Draw background if enabled
            if (_drawBackground && _backgroundColor.A > 0)
            {
                //This would use the renderer to draw background rectangle
            }
        }

        ///<param name="isClicked">Whether input was clicked this frame.</param>
        public virtual void HandleInput(Vector3 inputPosition, bool isClicked)
        {
            //TODO: base.HandleInput() doesn't take these parameters
            //base.HandleInput(inputPosition, isClicked);

            //Labels typically don't handle input unless specifically enabled
            //This can be overridden for interactive labels
        }

        ///<summary>
        ///Loads the default font.
        ///</summary>
        ///<returns>The default font instance.</returns>
        private Font LoadDefaultFont()
        {
            //In a real implementation, this would load a default font from resources
            //For now, we'll create a placeholder font
            return new Font("Default", _fontSize);
        }

        ///<summary>
        ///Invalidates the text measurement cache.
        ///</summary>
        private void InvalidateTextCache()
        {
            TextSize = MeasureText();
           DLogger.Log(LogSubsystems.Unknown, LogLevel.Trace, "TRACE", $"Label: Invalidated text cache for '{Id}'");
        }

        ///<summary>
        ///Gets label information as a string.
        ///</summary>
        public override string ToString()
        {
            return $"Label: Id='{Id}', Text='{_text}', " +
            $"Font={_font?.Name ?? "None"}, Size={TextSize}, " +
            $"Color={_textColor}, Alignment={_alignment}";
        }
    }

    ///<summary>
    ///Font class for text rendering.
    ///</summary>
    public class Font
    {
        public string Name { get; }
        public float Size { get; }
        public bool IsLoaded { get; }

        public Font(string name, float size)
        {
            Name = name;
            Size = size;
            IsLoaded = true; //In real implementation, this would check if font loaded successfully
        }

        public override string ToString()
        {
            return $"Font: {Name} ({Size}px)";
        }
    }

    ///<summary>
    ///Text alignment options.
    ///</summary>
    //public enum TextAlignment DUPLICATE
    //{
    //   ///<summary>Align text to the left.</summary>
    //   Left,

    //   ///<summary>Align text to the center.</summary>
    //   Center,

    //   ///<summary>Align text to the right.</summary>
    //   Right
    //}
}





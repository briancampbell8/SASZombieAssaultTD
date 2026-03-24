namespace SASZombieAssaultTD.Engine.UI.Layout
{
    /// <summary>
    /// Layout alignment options for UI elements
    /// P80-02-02: UILayoutTypes defining layout enums and structs for alignment, anchoring, and padding
    /// </summary>
    public enum UIAlignment
    {
        /// <summary>
        /// Align to the left edge
        /// </summary>
        Left,

        /// <summary>
        /// Align to the center horizontally
        /// </summary>
        Center,

        /// <summary>
        /// Align to the right edge
        /// </summary>
        Right
    }

    /// <summary>
    /// Vertical alignment options for UI elements
    /// </summary>
    public enum UIVerticalAlignment
    {
        /// <summary>
        /// Align to the top edge
        /// </summary>
        Top,

        /// <summary>
        /// Align to the center vertically
        /// </summary>
        Middle,

        /// <summary>
        /// Align to the bottom edge
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Anchor points for UI elements relative to their parent
    /// </summary>
    public enum UIAnchor
    {
        /// <summary>
        /// Anchor to the top-left corner
        /// </summary>
        TopLeft,

        /// <summary>
        /// Anchor to the top-center
        /// </summary>
        TopCenter,

        /// <summary>
        /// Anchor to the top-right corner
        /// </summary>
        TopRight,

        /// <summary>
        /// Anchor to the middle-left
        /// </summary>
        MiddleLeft,

        /// <summary>
        /// Anchor to the center
        /// </summary>
        MiddleCenter,

        /// <summary>
        /// Anchor to the middle-right
        /// </summary>
        MiddleRight,

        /// <summary>
        /// Anchor to the bottom-left corner
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Anchor to the bottom-center
        /// </summary>
        BottomCenter,

        /// <summary>
        /// Anchor to the bottom-right corner
        /// </summary>
        BottomRight
    }

    /// <summary>
    /// Layout direction for UI elements
    /// </summary>
    public enum UILayoutDirection
    {
        /// <summary>
        /// Layout elements horizontally
        /// </summary>
        Horizontal,

        /// <summary>
        /// Layout elements vertically
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Structure defining padding for UI elements
    /// </summary>
    public struct UIPadding
    {
        /// <summary>
        /// Left padding
        /// </summary>
        public float Left;

        /// <summary>
        /// Top padding
        /// </summary>
        public float Top;

        /// <summary>
        /// Right padding
        /// </summary>
        public float Right;

        /// <summary>
        /// Bottom padding
        /// </summary>
        public float Bottom;

        /// <summary>
        /// Total horizontal padding (left + right)
        /// </summary>
        public float Horizontal => Left + Right;

        /// <summary>
        /// Total vertical padding (top + bottom)
        /// </summary>
        public float Vertical => Top + Bottom;

        /// <summary>
        /// Creates a zero padding
        /// </summary>
        public static UIPadding Zero => new UIPadding { Left = 0, Top = 0, Right = 0, Bottom = 0 };

        /// <summary>
        /// Creates uniform padding
        /// </summary>
        /// <param name="padding">Padding value for all sides</param>
        public static UIPadding Uniform(float padding) => new UIPadding { Left = padding, Top = padding, Right = padding, Bottom = padding };

        /// <summary>
        /// Creates padding with different horizontal and vertical values
        /// </summary>
        /// <param name="horizontal">Horizontal padding (left and right)</param>
        /// <param name="vertical">Vertical padding (top and bottom)</param>
        public static UIPadding Symmetric(float horizontal, float vertical) => new UIPadding { Left = horizontal, Top = vertical, Right = horizontal, Bottom = vertical };
    }

    /// <summary>
    /// Structure defining margins for UI elements
    /// </summary>
    public struct UIMargin
    {
        /// <summary>
        /// Left margin
        /// </summary>
        public float Left;

        /// <summary>
        /// Top margin
        /// </summary>
        public float Top;

        /// <summary>
        /// Right margin
        /// </summary>
        public float Right;

        /// <summary>
        /// Bottom margin
        /// </summary>
        public float Bottom;

        /// <summary>
        /// Total horizontal margin (left + right)
        /// </summary>
        public float Horizontal => Left + Right;

        /// <summary>
        /// Total vertical margin (top + bottom)
        /// </summary>
        public float Vertical => Top + Bottom;

        /// <summary>
        /// Creates a zero margin
        /// </summary>
        public static UIMargin Zero => new UIMargin { Left = 0, Top = 0, Right = 0, Bottom = 0 };

        /// <summary>
        /// Creates uniform margin
        /// </summary>
        /// <param name="margin">Margin value for all sides</param>
        public static UIMargin Uniform(float margin) => new UIMargin { Left = margin, Top = margin, Right = margin, Bottom = margin };

        /// <summary>
        /// Creates margin with different horizontal and vertical values
        /// </summary>
        /// <param name="horizontal">Horizontal margin (left and right)</param>
        /// <param name="vertical">Vertical margin (top and bottom)</param>
        public static UIMargin Symmetric(float horizontal, float vertical) => new UIMargin { Left = horizontal, Top = vertical, Right = horizontal, Bottom = vertical };
    }

    /// <summary>
    /// Structure defining layout constraints for UI elements
    /// </summary>
    public struct UILayoutConstraints
    {
        /// <summary>
        /// Minimum width constraint
        /// </summary>
        public float MinWidth;

        /// <summary>
        /// Minimum height constraint
        /// </summary>
        public float MinHeight;

        /// <summary>
        /// Maximum width constraint
        /// </summary>
        public float MaxWidth;

        /// <summary>
        /// Maximum height constraint
        /// </summary>
        public float MaxHeight;

        /// <summary>
        /// Preferred width constraint
        /// </summary>
        public float PreferredWidth;

        /// <summary>
        /// Preferred height constraint
        /// </summary>
        public float PreferredHeight;

        /// <summary>
        /// Creates unconstrained layout
        /// </summary>
        public static UILayoutConstraints Unconstrained => new UILayoutConstraints
        {
            MinWidth = 0,
            MinHeight = 0,
            MaxWidth = float.MaxValue,
            MaxHeight = float.MaxValue,
            PreferredWidth = 0,
            PreferredHeight = 0
        };

        /// <summary>
        /// Creates fixed size constraints
        /// </summary>
        /// <param name="width">Fixed width</param>
        /// <param name="height">Fixed height</param>
        public static UILayoutConstraints Fixed(float width, float height) => new UILayoutConstraints
        {
            MinWidth = width,
            MinHeight = height,
            MaxWidth = width,
            MaxHeight = height,
            PreferredWidth = width,
            PreferredHeight = height
        };
    }
}





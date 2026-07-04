/*
File:    Size.cs
Purpose: Represents width and height used by UI, rendering, layout, and asset metadata.
         Provides comprehensive size operations for dimensional calculations and scaling.
         
Features: Complete size operations with width and height, aspect ratio calculations,
          scaling operations, and dimensional comparisons.
          Used by UI systems, rendering, layout calculations, and asset management.

Created: Engine Core Implementation
Notes:   This is the canonical size type for the entire engine.
         All size operations should use this unified Size type.
*/

using System;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Core
{
    ///<summary>
    ///Represents a two-dimensional size with width and height components.
    ///Used by UI, rendering, layout, and asset metadata systems throughout the engine.
    ///</summary>
    public readonly struct Size : IEquatable<Size>
    {
        /// Properties
        
        ///<summary>Width component of the size.</summary>
        public readonly float Width;
        
        ///<summary>Height component of the size.</summary>
        public readonly float Height;
        
        ///<summary>Aspect ratio (width/height).</summary>
        public float AspectRatio => Height > 0f ? Width / Height : 0f;
        
        ///<summary>Area of the size (width * height).</summary>
        public float Area => Width * Height;
        
        ///<summary>Perimeter of the size (2 * width + 2 * height).</summary>
        public float Perimeter => 2f * (Width + Height);
        
        ///<summary>Diagonal length of the size.</summary>
        public float Diagonal => (float)System.Math.Sqrt(Width * Width + Height * Height);
        
        ///<summary>Whether this size has zero area.</summary>
        public bool IsEmpty => Width <= 0f || Height <= 0f;
        
        ///<summary>Whether this size has positive area.</summary>
        public bool HasArea => Width > 0f && Height > 0f;
        
        ///<summary>Whether this size is square (width equals height).</summary>
        public bool IsSquare => System.Math.Abs(Width - Height) < 0.001f;
        
        ///<summary>Whether this size is wider than it is tall.</summary>
        public bool IsLandscape => Width > Height;
        
        ///<summary>Whether this size is taller than it is wide.</summary>
        public bool IsPortrait => Height > Width;
        
        ///<summary>Maximum dimension (width or height).</summary>
        public float MaxDimension => System.Math.Max(Width, Height);
        
        ///<summary>Minimum dimension (width or height).</summary>
        public float MinDimension => System.Math.Min(Width, Height);
        
        ///

        /// Constructors
        
        ///<summary>
        ///Creates a new size with specified width and height.
        ///</summary>
        ///<param name="width">Width component.</param>
        ///<param name="height">Height component.</param>
        public Size(float width, float height)
        {
            Width = System.Math.Max(width, 0f);
            Height = System.Math.Max(height, 0f);
        }
        
        ///<summary>
        ///Creates a new square size with the same width and height.
        ///</summary>
        ///<param name="size">Size for both width and height.</param>
        public Size(float size) : this(size, size) { }
        
        ///<summary>
        ///Copy constructor.
        ///</summary>
        ///<param name="other">Size to copy.</param>
        public Size(Size other)
        {
            Width = other.Width;
            Height = other.Height;
        }
        
        ///

        /// Static Factory Methods
        
        ///<summary>
        ///Creates a square size.
        ///</summary>
        ///<param name="dimension">Dimension for both width and height.</param>
        ///<returns>Square size.</returns>
        public static Size Square(float dimension) => new(dimension, dimension);
        
        ///<summary>
        ///Creates a size with the specified aspect ratio, fitting within the maximum dimensions.
        ///</summary>
        ///<param name="maxWidth">Maximum width.</param>
        ///<param name="maxHeight">Maximum height.</param>
        ///<param name="aspectRatio">Target aspect ratio (width/height).</param>
        ///<returns>Size with target aspect ratio that fits within the maximum dimensions.</returns>
        public static Size WithAspectRatio(float maxWidth, float maxHeight, float aspectRatio)
        {
            if (aspectRatio <= 0f)
                return new Size(maxWidth, maxHeight);
                
            var targetWidth = maxHeight * aspectRatio;
            var targetHeight = maxWidth / aspectRatio;
            
            if (targetWidth <= maxWidth)
                return new Size(targetWidth, maxHeight);
            else
                return new Size(maxWidth, targetHeight);
        }
        
        ///<summary>
        ///Creates a size that encompasses two sizes.
        ///</summary>
        ///<param name="a">First size.</param>
        ///<param name="b">Second size.</param>
        ///<returns>Size that encompasses both sizes.</returns>
        public static Size Max(Size a, Size b)
        {
            return new Size(System.Math.Max(a.Width, b.Width), System.Math.Max(a.Height, b.Height));
        }
        
        ///<summary>
        ///Creates a size that fits within two sizes.
        ///</summary>
        ///<param name="a">First size.</param>
        ///<param name="b">Second size.</param>
        ///<returns>Size that fits within both sizes.</returns>
        public static Size Min(Size a, Size b)
        {
            return new Size(System.Math.Min(a.Width, b.Width), System.Math.Min(a.Height, b.Height));
        }
        
        ///<summary>
        ///Creates a size by interpolating between two sizes.
        ///</summary>
        ///<param name="a">Start size.</param>
        ///<param name="b">End size.</param>
        ///<param name="t">Interpolation factor (0.0 = a, 1.0 = b).</param>
        ///<returns>Interpolated size.</returns>
        public static Size Lerp(Size a, Size b, float t)
        {
            t = System.Math.Clamp(t, 0f, 1f);
            return new Size(
                a.Width + (b.Width - a.Width) * t,
                a.Height + (b.Height - a.Height) * t
            );
        }
        
        ///

        /// Size Operations
        
        ///<summary>
        ///Scales this size by the specified factor.
        ///</summary>
        ///<param name="scale">Scale factor.</param>
        ///<returns>Scaled size.</returns>
        public Size Scale(float scale)
        {
            return new Size(Width * scale, Height * scale);
        }
        
        ///<summary>
        ///Scales this size by different factors horizontally and vertically.
        ///</summary>
        ///<param name="scaleX">Horizontal scale factor.</param>
        ///<param name="scaleY">Vertical scale factor.</param>
        ///<returns>Scaled size.</returns>
        public Size Scale(float scaleX, float scaleY)
        {
            return new Size(Width * scaleX, Height * scaleY);
        }
        
        ///<summary>
        ///Expands this size by the specified amount on both dimensions.
        ///</summary>
        ///<param name="amount">Amount to expand on each dimension.</param>
        ///<returns>Expanded size.</returns>
        public Size Expand(float amount)
        {
            return new Size(Width + amount, Height + amount);
        }
        
        ///<summary>
        ///Expands this size by different amounts horizontally and vertically.
        ///</summary>
        ///<param name="widthAmount">Amount to expand width.</param>
        ///<param name="heightAmount">Amount to expand height.</param>
        ///<returns>Expanded size.</returns>
        public Size Expand(float widthAmount, float heightAmount)
        {
            return new Size(Width + widthAmount, Height + heightAmount);
        }
        
        ///<summary>
        ///Contracts this size by the specified amount on both dimensions.
        ///</summary>
        ///<param name="amount">Amount to contract on each dimension.</param>
        ///<returns>Contracted size.</returns>
        public Size Contract(float amount)
        {
            return new Size(System.Math.Max(0f, Width - amount), System.Math.Max(0f, Height - amount));
        }
        
        ///<summary>
        ///Contracts this size by different amounts horizontally and vertically.
        ///</summary>
        ///<param name="widthAmount">Amount to contract width.</param>
        ///<param name="heightAmount">Amount to contract height.</param>
        ///<returns>Contracted size.</returns>
        public Size Contract(float widthAmount, float heightAmount)
        {
            return new Size(System.Math.Max(0f, Width - widthAmount), System.Math.Max(0f, Height - heightAmount));
        }
        
        ///<summary>
        ///Fits this size within the specified bounds while maintaining aspect ratio.
        ///</summary>
        ///<param name="maxWidth">Maximum width.</param>
        ///<param name="maxHeight">Maximum height.</param>
        ///<returns>Size that fits within the bounds while maintaining aspect ratio.</returns>
        public Size FitWithin(float maxWidth, float maxHeight)
        {
            if (Width <= maxWidth && Height <= maxHeight)
                return this; //Already fits
                
            var widthScale = maxWidth / Width;
            var heightScale = maxHeight / Height;
            var scale = System.Math.Min(widthScale, heightScale);
            
            return Scale(scale);
        }
        
        ///<summary>
        ///Fits this size within the specified bounds while maintaining aspect ratio.
        ///</summary>
        ///<param name="bounds">Maximum bounds.</param>
        ///<returns>Size that fits within the bounds while maintaining aspect ratio.</returns>
        public Size FitWithin(Size bounds)
        {
            return FitWithin(bounds.Width, bounds.Height);
        }
        
        ///<summary>
        ///Fills the specified bounds while maintaining aspect ratio (may crop).
        ///</summary>
        ///<param name="minWidth">Minimum width.</param>
        ///<param name="minHeight">Minimum height.</param>
        ///<returns>Size that fills the bounds while maintaining aspect ratio.</returns>
        public Size Fill(float minWidth, float minHeight)
        {
            if (Width >= minWidth && Height >= minHeight)
                return this; //Already fills
                
            var widthScale = minWidth / Width;
            var heightScale = minHeight / Height;
            var scale = System.Math.Max(widthScale, heightScale);
            
            return Scale(scale);
        }
        
        ///<summary>
        ///Fills the specified bounds while maintaining aspect ratio (may crop).
        ///</summary>
        ///<param name="bounds">Minimum bounds.</param>
        ///<returns>Size that fills the bounds while maintaining aspect ratio.</returns>
        public Size Fill(Size bounds)
        {
            return Fill(bounds.Width, bounds.Height);
        }
        
        ///<summary>
        ///Pads this size to achieve the specified aspect ratio.
        ///</summary>
        ///<param name="aspectRatio">Target aspect ratio.</param>
        ///<returns>Padded size with target aspect ratio.</returns>
        public Size PadToAspectRatio(float aspectRatio)
        {
            if (aspectRatio <= 0f)
                return this;
                
            var currentRatio = AspectRatio;
            
            if (System.Math.Abs(currentRatio - aspectRatio) < 0.001f)
                return this; //Already has target aspect ratio
                
            if (currentRatio > aspectRatio)
            {
                //Too wide, pad height
                var newHeight = Width / aspectRatio;
                return new Size(Width, newHeight);
            }
            else
            {
                //Too tall, pad width
                var newWidth = Height * aspectRatio;
                return new Size(newWidth, Height);
            }
        }
        
        ///<summary>
        ///Crops this size to achieve the specified aspect ratio.
        ///</summary>
        ///<param name="aspectRatio">Target aspect ratio.</param>
        ///<returns>Cropped size with target aspect ratio.</returns>
        public Size CropToAspectRatio(float aspectRatio)
        {
            if (aspectRatio <= 0f)
                return this;
                
            var currentRatio = AspectRatio;
            
            if (System.Math.Abs(currentRatio - aspectRatio) < 0.001f)
                return this; //Already has target aspect ratio
                
            if (currentRatio > aspectRatio)
            {
                //Too wide, crop width
                var newWidth = Height * aspectRatio;
                return new Size(newWidth, Height);
            }
            else
            {
                //Too tall, crop height
                var newHeight = Width / aspectRatio;
                return new Size(Width, newHeight);
            }
        }
        
        ///

        /// Utility Methods
        
        ///<summary>
        ///Gets the scale factor needed to fit this size within the specified bounds.
        ///</summary>
        ///<param name="maxWidth">Maximum width.</param>
        ///<param name="maxHeight">Maximum height.</param>
        ///<returns>Scale factor (1.0 = no scaling needed).</returns>
        public float GetFitScale(float maxWidth, float maxHeight)
        {
            if (Width <= maxWidth && Height <= maxHeight)
                return 1f; //Already fits
                
            var widthScale = maxWidth / Width;
            var heightScale = maxHeight / Height;
            return System.Math.Min(widthScale, heightScale);
        }
        
        ///<summary>
        ///Gets the scale factor needed to fill the specified bounds.
        ///</summary>
        ///<param name="minWidth">Minimum width.</param>
        ///<param name="minHeight">Minimum height.</param>
        ///<returns>Scale factor (1.0 = no scaling needed).</returns>
        public float GetFillScale(float minWidth, float minHeight)
        {
            if (Width >= minWidth && Height >= minHeight)
                return 1f; //Already fills
                
            var widthScale = minWidth / Width;
            var heightScale = minHeight / Height;
            return System.Math.Max(widthScale, heightScale);
        }
        
        ///<summary>
        ///Determines if this size can fit within the specified bounds.
        ///</summary>
        ///<param name="maxWidth">Maximum width.</param>
        ///<param name="maxHeight">Maximum height.</param>
        ///<returns>True if this size can fit within the bounds.</returns>
        public bool CanFitWithin(float maxWidth, float maxHeight)
        {
            return Width <= maxWidth && Height <= maxHeight;
        }
        
        ///<summary>
        ///Determines if this size can fill the specified bounds.
        ///</summary>
        ///<param name="minWidth">Minimum width.</param>
        ///<param name="minHeight">Minimum height.</param>
        ///<returns>True if this size can fill the bounds.</returns>
        public bool CanFill(float minWidth, float minHeight)
        {
            return Width >= minWidth && Height >= minHeight;
        }
        
        ///<summary>
        ///Gets the dominant dimension (width or height).
        ///</summary>
        ///<returns>"Width" if width > height, "Height" if height > width, "Equal" if they're the same.</returns>
        public string GetDominantDimension()
        {
            if (System.Math.Abs(Width - Height) < 0.001f)
                return "Equal";
            else if (Width > Height)
                return "Width";
            else
                return "Height";
        }
        
        ///

        /// Equality and Hashing
        
        public bool Equals(Size other)
        {
            return System.Math.Abs(Width - other.Width) < 0.001f &&
                   System.Math.Abs(Height - other.Height) < 0.001f;
        }
        
        public override bool Equals(object obj) => obj is Size other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(Width, Height);
        
        ///

        /// Operators
        
        public static bool operator ==(Size left, Size right) => left.Equals(right);
        public static bool operator !=(Size left, Size right) => !left.Equals(right);
        
        public static Size operator +(Size left, Size right) => new Size(left.Width + right.Width, left.Height + right.Height);
        public static Size operator -(Size left, Size right) => new Size(System.Math.Max(0f, left.Width - right.Width), System.Math.Max(0f, left.Height - right.Height));
        public static Size operator *(Size size, float scale) => size.Scale(scale);
        public static Size operator *(float scale, Size size) => size.Scale(scale);
        public static Size operator /(Size size, float scale) => new Size(size.Width / scale, size.Height / scale);
        
        ///

        /// String Representation
        
        public override string ToString()
        {
            return $"Size(Width: {Width:F1}, Height: {Height:F1})";
        }
        
        ///<summary>
        ///Gets a compact string representation.
        ///</summary>
        ///<returns>Compact string like "1920x1080".</returns>
        public string ToCompactString()
        {
            return $"{Width:F0}x{Height:F0}";
        }
        
        ///
    }
}

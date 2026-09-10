// ====================================================================================================
//  FILE: SizeAspect.cs
//  PATH: ./Engine/Core/CoreSize/
//  MODULE: Core Size
//
//  ROLE:
//      Provide aspect-ratio related helpers for the Core Size subsystem.
//
//  RESPONSIBILITIES:
//      - Compute aspect ratios for Size.
//      - Provide helpers for fitting/padding Size to target aspect ratios.
//      - Provide simple classification helpers (wider/taller/square).
//
//  NON-RESPONSIBILITIES:
//      - Defining the Size struct (handled by Size.cs).
//      - Fit/fill scaling (handled by SizeFit.cs).
//      - General size math operations (handled by SizeOps.cs).
//      - Utility formatting and hashing (handled by SizeUtil.cs).
// ====================================================================================================

namespace SASZombieAssaultTD.Engine.CoreSize
{
    internal static class SizeAspect
    {
        // -------------------------------------------------------------------------------------------------
        // BASIC ASPECT RATIO
        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Computes the aspect ratio (Width / Height) for the given size.
        /// Returns 0 if Height is non-positive.
        /// </summary>
        /// <param name="size">Source size.</param>
        /// <returns>Aspect ratio (Width / Height) or 0 if invalid.</returns>
        public static float GetAspectRatio(Size size)
        {
            return size.Height > 0f ? size.Width / size.Height : 0f;
        }

        /// <summary>
        /// Determines whether the size is approximately square within a tolerance.
        /// </summary>
        /// <param name="size">Source size.</param>
        /// <param name="tolerance">Allowed difference between width and height.</param>
        /// <returns>True if |Width - Height| &lt;= tolerance.</returns>
        public static bool IsSquare(Size size, float tolerance = 0.01f)
        {
            return System.Math.Abs(size.Width - size.Height) <= tolerance;
        }

        /// <summary>
        /// Determines whether the size is wider than it is tall.
        /// </summary>
        public static bool IsWider(Size size)
        {
            return size.Width > size.Height;
        }

        /// <summary>
        /// Determines whether the size is taller than it is wide.
        /// </summary>
        public static bool IsTaller(Size size)
        {
            return size.Height > size.Width;
        }

        // -------------------------------------------------------------------------------------------------
        // ASPECT RATIO ADJUSTMENT
        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Pads the given size to match the target aspect ratio by expanding
        /// either width or height while preserving the other dimension.
        /// </summary>
        /// <param name="size">Original size.</param>
        /// <param name="targetAspect">Target aspect ratio (width / height).</param>
        /// <returns>New size padded to the target aspect ratio.</returns>
        public static Size PadToAspect(Size size, float targetAspect)
        {
            if (size.Height <= 0f || targetAspect <= 0f)
                return size;

            float currentAspect = GetAspectRatio(size);

            // If already close enough, return original
            if (System.Math.Abs(currentAspect - targetAspect) < 0.0001f)
                return size;

            // If too wide, increase height; if too tall, increase width
            if (currentAspect > targetAspect)
            {
                // Too wide: height must grow
                float newHeight = size.Width / targetAspect;
                return new Size(size.Width, newHeight);
            }
            else
            {
                // Too tall: width must grow
                float newWidth = size.Height * targetAspect;
                return new Size(newWidth, size.Height);
            }
        }

        /// <summary>
        /// Scales the given size uniformly so that its aspect ratio matches the target,
        /// preserving area proportions as much as possible.
        /// </summary>
        /// <param name="size">Original size.</param>
        /// <param name="targetAspect">Target aspect ratio (width / height).</param>
        /// <returns>New size scaled to the target aspect ratio.</returns>
        public static Size ScaleToAspect(Size size, float targetAspect)
        {
            if (size.Width <= 0f || size.Height <= 0f || targetAspect <= 0f)
                return size;

            float currentAspect = GetAspectRatio(size);

            // If already close enough, return original
            if (System.Math.Abs(currentAspect - targetAspect) < 0.0001f)
                return size;

            // Choose a dimension to preserve and scale the other
            if (currentAspect > targetAspect)
            {
                // Too wide: preserve height, adjust width
                float newWidth = size.Height * targetAspect;
                return new Size(newWidth, size.Height);
            }
            else
            {
                // Too tall: preserve width, adjust height
                float newHeight = size.Width / targetAspect;
                return new Size(size.Width, newHeight);
            }
        }

        // -------------------------------------------------------------------------------------------------
        // COMPARISON HELPERS
        // -------------------------------------------------------------------------------------------------

        /// <summary>
        /// Compares the aspect ratio of two sizes.
        /// Returns:
        /// &lt; 0 if a's aspect &lt; b's aspect,
        /// &gt; 0 if a's aspect &gt; b's aspect,
        /// 0 if they are approximately equal.
        /// </summary>
        public static int CompareAspect(Size a, Size b, float tolerance = 0.0001f)
        {
            float aspectA = GetAspectRatio(a);
            float aspectB = GetAspectRatio(b);

            float diff = aspectA - aspectB;
            if (System.Math.Abs(diff) <= tolerance)
                return 0;

            return diff < 0f ? -1 : 1;
        }
    }
}

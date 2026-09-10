// ====================================================================================================
//  FILE: SizeFit.cs
//  PATH: ./Engine/Core/Size/
//  MODULE: Core Size
//
//  ROLE:
//      Provide fit/fill scaling operations for the Core Size subsystem.
//
//  RESPONSIBILITIES:
//      - Provide FitWithin() behavior for the Core subsystem.
//      - Provide Fill() behavior for the Core subsystem.
//      - Provide GetFitScale() behavior for the Core subsystem.
//      - Provide GetFillScale() behavior for the Core subsystem.
//      - Provide CanFitWithin() behavior for the Core subsystem.
//      - Provide CanFill() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
namespace SASZombieAssaultTD.Engine.CoreSize
{
    internal static class SizeFit
    {
        public static float GetFitScale(Size source, Size target)
        {
            if (!source.HasArea || !target.HasArea)
                return 0f;

            float scaleW = target.Width / source.Width;
            float scaleH = target.Height / source.Height;

            return System.Math.Min(scaleW, scaleH);
        }

        public static float GetFillScale(Size source, Size target)
        {
            if (!source.HasArea || !target.HasArea)
                return 0f;

            float scaleW = target.Width / source.Width;
            float scaleH = target.Height / source.Height;

            return System.Math.Max(scaleW, scaleH);
        }

        public static bool CanFitWithin(Size source, Size target)
        {
            return GetFitScale(source, target) >= 1f;
        }

        public static bool CanFill(Size source, Size target)
        {
            return GetFillScale(source, target) >= 1f;
        }

        public static Size FitWithin(Size source, Size target)
        {
            float scale = GetFitScale(source, target);
            return new Size(source.Width * scale, source.Height * scale);
        }

        public static Size Fill(Size source, Size target)
        {
            float scale = GetFillScale(source, target);
            return new Size(source.Width * scale, source.Height * scale);
        }
    }
}

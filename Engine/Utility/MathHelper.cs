/*
File:    MathHelper.cs
Purpose: Clamp; Lerp.
*/
namespace SASZombieAssaultTD.Engine.Utility
{
    public static class MathHelper
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}





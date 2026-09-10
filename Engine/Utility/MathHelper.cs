// ====================================================================================================
//  FILE: MathHelper.cs
//  PATH: ./Engine/Utility/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the MathHelper module.
//
//  RESPONSIBILITIES:
//      - Provide Clamp() behavior for the Core subsystem.
//      - Provide Lerp() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    MathHelper.cs
Purpose: Clamp; Lerp.
*/
using SASZombieAssaultTD.Engine.Diagnostics;

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






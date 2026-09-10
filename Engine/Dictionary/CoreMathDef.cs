// ====================================================================================================
//  FILE: CoreMathDef.cs
//  PATH: ./Engine/Dictionary/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the CoreMathDef module.
//
//  RESPONSIBILITIES:
//      - Provide Approximately() behavior for the Core subsystem.
//      - Provide Approximately() behavior for the Core subsystem.
//      - Provide GetTime() behavior for the Core subsystem.
//      - Provide Clamp() behavior for the Core subsystem.
//      - Provide Clamp() behavior for the Core subsystem.
//      - Provide Clamp() behavior for the Core subsystem.
//      - Provide CreateTranslation() behavior for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Dictionary
{
    public static class EngineMath
    {
        //Missing methods
        public static bool Approximately(float a, float b, float epsilon = 0.0001f) => System.Math.Abs(a - b) < epsilon;
        public static bool Approximately(double a, double b, double epsilon = 0.0001) => System.Math.Abs(a - b) < epsilon;
        public static float GetTime() => (float)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        //MathFunctions nested class
        public static class MathFunctions
        {
            public static float Clamp(float value, float min, float max) => System.Math.Clamp(value, min, max);
            public static double Clamp(double value, double min, double max) => System.Math.Clamp(value, min, max);
            public static int Clamp(int value, int min, int max) => System.Math.Clamp(value, min, max);
        }
    }

    public struct Matrix
    {
        //Missing method
        public static Matrix CreateTranslation(float x, float y, float z) => 
            new Matrix
            {
                M11 = 1, M12 = 0, M13 = 0, M14 = 0,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M41 = x, M42 = y, M43 = z, M44 = 1
            };
            
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;
    }
}


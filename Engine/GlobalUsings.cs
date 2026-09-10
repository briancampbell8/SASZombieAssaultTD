// ====================================================================================================
//  FILE: GlobalUsings.cs
//  PATH: ./Engine/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the GlobalUsings module.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Core subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
//Global using directives for core types
global using Color = SASZombieAssaultTD.Engine.CoreSize.Color;
// Global using directives for core types
namespace SASZombieAssaultTD.Engine.CoreSize
{
    public readonly struct Color
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public Color(byte r, byte g, byte b, byte a = 255) { R = r; G = g; B = b; A = a; }

        public static Color White => new Color(255, 255, 255, 255);
    }
}


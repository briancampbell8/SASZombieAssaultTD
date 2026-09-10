// ====================================================================================================
//  FILE: KeyCode.cs
//  PATH: ./Engine/Input/
//  MODULE: Core
//
//  ROLE:
//      Encapsulate core engine behavior for the KeyCode module.
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
using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.Input
{
    ///<summary>
    ///Input key codes to fix compilation errors.
    ///</summary>
    public enum KeyCode
    {
        Unknown = 0,
        Space = 32,
        Enter = 13,
        Escape = 27,
        Tab = 9,
        Backspace = 8,
        Delete = 127,
        
        //Arrow keys
        LeftArrow = 37,
        RightArrow = 39,
        UpArrow = 38,
        DownArrow = 40,
        
        //Letters
        A = 65, B = 66, C = 67, D = 68, E = 69, F = 70, G = 71, H = 72, I = 73, J = 74,
        K = 75, L = 76, M = 77, N = 78, O = 79, P = 80, Q = 81, R = 82, S = 83, T = 84,
        U = 85, V = 86, W = 87, X = 88, Y = 89, Z = 90,
        
        //Numbers
        Num0 = 48, Num1 = 49, Num2 = 50, Num3 = 51, Num4 = 52, Num5 = 53, Num6 = 54, Num7 = 55, Num8 = 56, Num9 = 57,
        
        //Function keys
        F1 = 112, F2 = 113, F3 = 114, F4 = 115, F5 = 116, F6 = 117, F7 = 118, F8 = 119, F9 = 120, F10 = 121, F11 = 122, F12 = 123
    }
}


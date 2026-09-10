// ====================================================================================================
//  FILE: TextAlignment.cs
//  PATH: ./Engine/Rendering/
//  MODULE: Rendering
//
//  ROLE:
//      Provide rendering logic, draw calls, batching, or GPU resource management.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the Rendering subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    ///<summary>
    ///Text alignment options for rendering.
    ///P11-04-09-F: Supports left, center, and right text alignment.
    ///</summary>
    public enum TextAlignment
    {
        ///<summary>Align text to the left</summary>
        Left,
        ///<summary>Align text to the center</summary>
        Center,
        ///<summary>Align text to the right</summary>
        Right
    }
}


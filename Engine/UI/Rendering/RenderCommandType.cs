// ====================================================================================================
//  FILE: RenderCommandType.cs
//  PATH: ./Engine/UI/Rendering/
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
//============================================================================
//File: RenderCommandType.cs
//Path: E:\BDC\Projects\SASZombieAssaultTD\Engine\UI\Rendering\RenderCommandType.cs
//Program: RenderCommandType
//Subsystem: UI Rendering / Command Queue
//
//Purpose:
//    Defines the types of UI render commands supported by the modern UI
//    rendering pipeline. Replaces the legacy constant-class version.
//
//Doctrine:
//    - Pure enum (no logic)
//    - No System.Diagnostics
//    - No DLogger.Log() needed
//    - Deterministic, grep‑friendly identifiers
//============================================================================

namespace SASZombieAssaultTD.Engine.UI.Rendering
//
{
    /// <summary>
    /// Types of render commands supported by the UI rendering system.
    /// </summary>
    public enum RenderCommandType
    {
        /// <summary>
        /// Draws a UI element (rectangles, panels, bars, etc.).
        /// </summary>
        DrawElement = 0,

        /// <summary>
        /// Draws a texture or sprite.
        /// </summary>
        DrawTexture = 1,

        /// <summary>
        /// Draws a rectangle primitive.
        /// </summary>
        DrawRectangle = 2,

        /// <summary>
        /// Draws a line primitive.
        /// </summary>
        DrawLine = 3,

        /// <summary>
        /// Draws a circle primitive.
        /// </summary>
        DrawCircle = 4,

        /// <summary>
        /// Draws text using the active font/material.
        /// </summary>
        DrawText = 5,

        Rectangle = 6,
        Sprite = 7,
        Line = 8,
        Text = 9
    }
}
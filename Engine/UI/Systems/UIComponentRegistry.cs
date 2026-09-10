// ====================================================================================================
//  FILE: UIComponentRegistry.cs
//  PATH: ./Engine/UI/Systems/
//  MODULE: UI
//
//  ROLE:
//      Provide UI layout, interaction logic, or HUD rendering.
//
//  RESPONSIBILITIES:
//      - Provide core functionality for the UI subsystem.
//
//  NON-RESPONSIBILITIES:
//      - Low-level data persistence or file serialization.
//
//  NOTES:
//      Auto-generated structure verified locally via file state scripts.
// ====================================================================================================
/*
File:    UIComponentRegistry.cs
Purpose: Registry for UI components.
*/

using System;   using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.UI.Systems
//
{
    ///<summary>
    ///Registry for UI components.
    ///</summary>
    public static class UIComponentRegistry
    {
        ///<summary>
        ///Registers a UI component.
        ///</summary>
        public static void RegisterComponent<T>() where T : class
        {
            DLogger.Log(LogSubsystems.UI, LogEnums.LogLevel.Info, $"UIComponentRegistry: Registered component {typeof(T).Name}");
        }
    }
}


/*
File:    UIComponentRegistry.cs
Purpose: Registry for UI components.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Systems
{
    /// <summary>
    /// Registry for UI components.
    /// </summary>
    public static class UIComponentRegistry
    {
        /// <summary>
        /// Registers a UI component.
        /// </summary>
        public static void RegisterComponent<T>() where T : class
        {
            Engine.Diagnostics.DebugLogger.LogDebug("INFO", $"UIComponentRegistry: Registered component {typeof(T).Name}");
        }
    }
}

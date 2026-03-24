/*
File:    UIEventSystem.cs
Purpose: UI event system for handling UI events.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Systems
{
    /// <summary>
    /// UI event system for handling UI events.
    /// </summary>
    public static class UIEventSystem
    {
        /// <summary>
        /// Initializes the UI event system.
        /// </summary>
        public static void Initialize()
        {
            ModernLoggingSystem.Log("INFO", "UIEventSystem: Initialized");
        }
    }
}

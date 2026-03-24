/*
File:    UIElementFactory.cs
Purpose: Factory for creating UI elements.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Systems
{
    /// <summary>
    /// Factory for creating UI elements.
    /// </summary>
    public static class UIElementFactory
    {
        /// <summary>
        /// Initializes the UI element factory.
        /// </summary>
        public static void Initialize()
        {
            ModernLoggingSystem.Log("INFO", "UIElementFactory: Initialized");
        }
    }
}

/*
File:    UIElementFactory.cs
Purpose: Factory for creating UI elements.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.UI.Systems
//
{
    ///<summary>
    ///Factory for creating UI elements.
    ///</summary>
    public static class UIElementFactory
    {
        ///<summary>
        ///Initializes the UI element factory.
        ///</summary>
        public static void Initialize()
        {
            DLogger.Log(LogSubsystems.UI, LogLevel.Info, "UIElementFactory: Initialized");
        }
    }
}

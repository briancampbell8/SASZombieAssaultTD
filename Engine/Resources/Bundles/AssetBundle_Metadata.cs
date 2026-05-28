/*
Program Name: SASZombieAssaultTD
File Path: Engine\Resources\Bundles\AssetBundle_Metadata.cs
Purpose: Resource management, save system, scene management, and state machine systems.
Features: Asset bundles, save data persistence, scene transitions, and enhanced state management.
*/

using System;
using SASZombieAssaultTD.Engine.Diagnostics;




namespace SASZombieAssaultTD.Engine.Resources
{
    public partial class AssetBundle
    {
        private const string PassThruMessage =
            "[DIAG][PASS-THRU] {0}: execution forwarded with no processing or state changes.";

        public static void ForwardExecutionWithDebugging(object context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            string message = string.Format(PassThruMessage, context);

            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

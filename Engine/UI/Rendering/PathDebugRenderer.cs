using System.Diagnostics;
using System.Reflection;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.UI.Rendering
{
    public class UIPathDebugRenderer
    {
        public UIPathDebugRenderer()
        {
            ModernLoggingSystem.Log("BREAKPOINT", "Execution reached here");
            ModernLoggingSystem.Log("BREAKPOINT", "Reached execution checkpoint");
            ModernLoggingSystem.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }

        public void Update()
        {
            ModernLoggingSystem.Log("BREAKPOINT", "Execution reached here");
            ModernLoggingSystem.Log("BREAKPOINT", "Reached execution checkpoint");
            ModernLoggingSystem.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }
    }
}






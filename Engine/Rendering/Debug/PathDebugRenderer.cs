using System.Diagnostics;
using System.Reflection;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Rendering.Debug
{
    public class PathDebugRenderer
    {
        public PathDebugRenderer()
        {
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", "Execution reached here");
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", "Reached execution checkpoint");
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }

        public void Update()
        {
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", "Execution reached here");
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", "Reached execution checkpoint");
            Engine.Diagnostics.DebugLogger.LogDebug("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }
    }
}






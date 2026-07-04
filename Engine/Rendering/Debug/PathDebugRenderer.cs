using System.Diagnostics;
using System.Reflection;
using SASZombieAssaultTD.Engine.Core;

using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Rendering.Debug
//
{
    public class PathDebugRenderer
    {
        public PathDebugRenderer()
        {
            DLogger.Log("BREAKPOINT", "Execution reached here");
            DLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DLogger.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }

        public void Update()
        {
            DLogger.Log("BREAKPOINT", "Execution reached here");
            DLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DLogger.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }
    }
}






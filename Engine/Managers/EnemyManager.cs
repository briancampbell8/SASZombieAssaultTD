/*
    File:    EnemyManager.cs
    Author:  BDC
    Created: 2026-02-10

    Purpose:
        Stub manager for coordinating enemy instances with debug logging.

    Notes:
        <Any architectural notes, constraints, or special behaviors.>

*/
using SASZombieAssaultTD.Engine.Systems.Diagnostics;
using System.Diagnostics;
using System.Reflection;

namespace SASZombieAssaultTD.Engine.Managers
{
    public class EnemyManager
    {
        public EnemyManager()
        {
            DebugLogger.Log("BREAKPOINT", "Execution reached here");
            DebugLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DebugLogger.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }

        public void Update()
        {
            DebugLogger.Log("BREAKPOINT", "Execution reached here");
            DebugLogger.Log("BREAKPOINT", "Reached execution checkpoint");
            DebugLogger.Log("BREAKPOINT", $"Method={nameof(MethodBase.GetCurrentMethod)}, Line={new StackTrace(true).GetFrame(0)?.GetFileLineNumber()}");
        }
    }
}


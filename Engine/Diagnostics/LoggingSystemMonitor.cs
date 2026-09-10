// ====================================================================================================
//  FILE: LoggingSystemMonitor.cs
//  PATH: Engine/Core/LoggingSystemMonitor.cs
//  SUBSYSTEM: Diagnostics
//  ROLE: Public façade for logging subsystem health evaluation and status reporting.
//
//  RESPONSIBILITIES:
//      - Expose public API for checking logging system health.
//      - Retrieve monitoring statistics from DiagnosticsMonitor.
//      - Provide formatted status strings for runtime display or debugging.
//      - Attempt automatic recovery when emergency-stop is active.
//      - Supply detailed monitoring data for diagnostic tools.
//
//  NON-RESPONSIBILITIES:
//      - Log routing, formatting, or sink management (handled by DebugLogger).
//      - Monitoring state storage or threshold evaluation (handled by DiagnosticsMonitor).
//      - External reporting, serialization, or UI rendering.
//
//  ARCHITECTURAL NOTES:
//      - All monitoring data originates from DiagnosticsMonitor.
//      - DebugLogger must not expose monitoring APIs.
//      - Emergency-stop recovery delegates to DiagnosticsMonitor.ResetEmergencyStop().
//      - This façade must remain stateless and deterministic.
// ==================================================================================================== 

//
using static SASZombieAssaultTD.Engine.Diagnostics.LogEnums;
namespace SASZombieAssaultTD.Engine.Diagnostics
{
    ///<summary>
    ///Provides runtime health monitoring accessors for the logging subsystem.
    ///Wraps DiagnosticsMonitor to expose simplified health evaluation and status reporting.
    ///</summary>
    public static class LoggingSystemMonitor
    {
        ///<summary>
        ///Check if logging system is healthy.
        ///</summary>
        ///<returns>True if system is healthy, false if emergency stop is active.</returns>
        public static bool IsHealthy()
        {
            var (isEmergencyStop, _, _) = DiagnosticsMonitor.GetMonitoringStats();
            return !isEmergencyStop;
        }

        ///<summary>
        ///Get current logging system status.
        ///</summary>
        ///<returns>Status information about the logging system.</returns>
        public static string GetStatus()
        {
            var (isEmergencyStop, logsPerSecond, duplicateErrors) = DiagnosticsMonitor.GetMonitoringStats();

            if (isEmergencyStop)
                return "🚨 EMERGENCY STOP ACTIVE - Logging system has been halted due to abnormal conditions";

            if (logsPerSecond > 500)
                return $"⚠️ WARNING - High log volume: {logsPerSecond} logs/sec, {duplicateErrors} duplicate errors (approaching emergency stop)";

            if (duplicateErrors > 5)
                return $"⚠️ WARNING - Multiple duplicate errors: {logsPerSecond} logs/sec, {duplicateErrors} duplicate errors (approaching emergency stop)";

            return $"✅ Healthy - {logsPerSecond} logs/sec, {duplicateErrors} duplicate errors";
        }

        ///<summary>
        ///Perform health check with automatic recovery if needed.
        ///</summary>
        ///<returns>True if system is healthy or was successfully recovered.</returns>
        public static bool PerformHealthCheck()
        {
            var (isEmergencyStop, logsPerSecond, duplicateErrors) = DiagnosticsMonitor.GetMonitoringStats();

            if (!isEmergencyStop)
                return true;

            DLogger.Log(LogSubsystems.General, LogLevel.Info, LogCategory.General, "LoggingSystemMonitor: Recovery failed - manual intervention required");
            return false;

            DiagnosticsMonitor.ResetEmergencyStop();

            var (stillEmergency, _, _) = DiagnosticsMonitor.GetMonitoringStats();

            if (!stillEmergency)
            {
                DLogger.Log(LogSubsystems.General, LogLevel.Info, LogCategory.General, "LoggingSystemMonitor: Emergency stop cleared - system recovered");
                return true;
            }

            DLogger.Log(LogSubsystems.General, LogLevel.Info, LogCategory.General, "LoggingSystemMonitor: Recovery failed - manual intervention required");
            return false;
        }

        ///<summary>
        ///Get detailed statistics for debugging.
        ///</summary>
        public static (bool IsHealthy, int LogsPerSecond, int DuplicateErrors, string Status) GetDetailedStats()
        {
            var (isEmergencyStop, logsPerSecond, duplicateErrors) = DiagnosticsMonitor.GetMonitoringStats();
            var status = GetStatus();

            return (!isEmergencyStop, (int)logsPerSecond, (int)duplicateErrors, status);
        }
    }
}

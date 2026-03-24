using System;

namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Logging system health monitor for runtime monitoring.
    /// Provides easy access to logging system status and health checks.
    /// </summary>
    public static class LoggingSystemMonitor
    {
        /// <summary>
        /// Check if logging system is healthy.
        /// </summary>
        /// <returns>True if system is healthy, false if emergency stop is active.</returns>
        public static bool IsHealthy()
        {
            var (isEmergencyStop, _, _) = ModernLoggingSystem.GetMonitoringStats();
            return !isEmergencyStop;
        }

        /// <summary>
        /// Get current logging system status.
        /// </summary>
        /// <returns>Status information about the logging system.</returns>
        public static string GetStatus()
        {
            var (isEmergencyStop, logsPerSecond, duplicateErrors) = ModernLoggingSystem.GetMonitoringStats();
            
            if (isEmergencyStop)
            {
                return "🚨 EMERGENCY STOP ACTIVE - Logging system has been halted due to abnormal conditions";
            }
            
            if (logsPerSecond > 500)
            {
                return $"⚠️ WARNING - High log volume: {logsPerSecond} logs/sec, {duplicateErrors} duplicate errors (approaching emergency stop)";
            }
            
            if (duplicateErrors > 5)
            {
                return $"⚠️ WARNING - Multiple duplicate errors: {logsPerSecond} logs/sec, {duplicateErrors} duplicate errors (approaching emergency stop)";
            }
            
            return $"✅ Healthy - {logsPerSecond} logs/sec, {duplicateErrors} duplicate errors";
        }

        /// <summary>
        /// Perform health check with automatic recovery if needed.
        /// </summary>
        /// <returns>True if system is healthy or was successfully recovered.</returns>
        public static bool PerformHealthCheck()
        {
            var (isEmergencyStop, logsPerSecond, duplicateErrors) = ModernLoggingSystem.GetMonitoringStats();
            
            if (isEmergencyStop)
            {
                // Log the health check attempt (this will work since it's critical)
                ModernLoggingSystem.Log(ModernLoggingSystem.LogLevel.Critical, "HEALTH_CHECK", 
                    $"Emergency stop detected - attempting recovery: {logsPerSecond} logs/sec, {duplicateErrors} duplicates");
                
                // Attempt recovery
                ModernLoggingSystem.ResetEmergencyStop();
                
                // Check if recovery worked
                var (stillEmergency, _, _) = ModernLoggingSystem.GetMonitoringStats();
                if (!stillEmergency)
                {
                    ModernLoggingSystem.Log(ModernLoggingSystem.LogLevel.Info, "HEALTH_CHECK", 
                        "Emergency stop successfully cleared - logging system recovered");
                    return true;
                }
                else
                {
                    Console.WriteLine("❌ Failed to recover logging system - manual intervention required");
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Get detailed statistics for debugging.
        /// </summary>
        public static (bool IsHealthy, int LogsPerSecond, int DuplicateErrors, string Status) GetDetailedStats()
        {
            var (isEmergencyStop, logsPerSecond, duplicateErrors) = ModernLoggingSystem.GetMonitoringStats();
            var status = GetStatus();
            
            return (!isEmergencyStop, (int)logsPerSecond, (int)duplicateErrors, status);
        }
    }
}

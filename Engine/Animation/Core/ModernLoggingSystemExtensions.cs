using System;

namespace SASZombieAssaultTD.Engine.Core
{
    /// <summary>
    /// Helper methods for ModernLoggingSystem to maintain backward compatibility.
    /// Provides legacy 2-parameter Log method support while migrating to 3-parameter signature.
    /// </summary>
    public static class ModernLoggingSystemExtensions
    {
        /// <summary>
        /// Legacy Log method for backward compatibility.
        /// Maps category strings to appropriate LogLevel.
        /// </summary>
        /// <param name="category">Log category (DEBUG, INFO, WARNING, ERROR, CRITICAL)</param>
        /// <param name="message">Log message</param>
        public static void Log(string category, string message)
        {
            var logLevel = category.ToUpper() switch
            {
                "DEBUG" => ModernLoggingSystem.LogLevel.Debug,
                "INFO" => ModernLoggingSystem.LogLevel.Info,
                "WARNING" => ModernLoggingSystem.LogLevel.Warning,
                "WARN" => ModernLoggingSystem.LogLevel.Warning,
                "ERROR" => ModernLoggingSystem.LogLevel.Error,
                "CRITICAL" => ModernLoggingSystem.LogLevel.Critical,
                _ => ModernLoggingSystem.LogLevel.Debug
            };
            
            ModernLoggingSystem.Log(logLevel, category, message);
        }
    }
}

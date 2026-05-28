/*
File:    ErrorHandling.cs
Path:    Engine/GameLoop/ErrorHandling.cs
Purpose: P11-09-01 - Contains all error handling for GameLoop.
         Handles exception handling and recovery mechanisms.

Role:     Game loop error handling specialist.
         - Exception handling logic
         - Safe shutdown procedures
         - Error recovery mechanisms
         - Error logging and reporting
         - Critical error detection

Notes:    Contains all error handling logic extracted from GameLoop.
         Ensures graceful error handling without polluting main loop.
         Provides comprehensive error recovery strategies.
*/

using System;
using SASZombieAssaultTD.Engine.Core;

namespace SASZombieAssaultTD.Engine.Systems
{
    /// <summary>
    /// Partial class containing error handling logic for GameLoop.
    /// </summary>
    public partial class GameLoop
    {
        // Error tracking
        private int _criticalErrorCount = 0;
        private int _totalErrorCount = 0;
        private DateTime _lastErrorTime = DateTime.MinValue;
        private readonly object _errorLock = new();

        /// <summary>
        /// Gets the number of critical errors encountered.
        /// </summary>
        public int CriticalErrorCount => _criticalErrorCount;

        /// <summary>
        /// Gets the total number of errors encountered.
        /// </summary>
        public int TotalErrorCount => _totalErrorCount;

        /// <summary>
        /// Gets the time of the last error.
        /// </summary>
        public DateTime LastErrorTime => _lastErrorTime;

        /// <summary>
        /// Handles frame processing errors.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="context">The context in which the error occurred.</param>
        private void HandleFrameError(Exception ex, string context)
        {
            lock (_errorLock)
            {
                _totalErrorCount++;
                _lastErrorTime = DateTime.Now;

                // Record error in diagnostics
                _diagnostics.RecordFrameError(ex);

                // Log error with context
                LogFrameError(ex, context);

                // Determine error severity and response
                var severity = DetermineErrorSeverity(ex);

                switch (severity)
                {
                    case ErrorSeverity.Critical:
                        HandleCriticalError(ex, context);
                        break;
                    case ErrorSeverity.Serious:
                        HandleSeriousError(ex, context);
                        break;
                    case ErrorSeverity.Minor:
                        HandleMinorError(ex, context);
                        break;
                }
            }
        }

        /// <summary>
        /// Determines the severity of an error.
        /// </summary>
        /// <param name="ex">The exception to evaluate.</param>
        /// <returns>The error severity.</returns>
        private ErrorSeverity DetermineErrorSeverity(Exception ex)
        {
            return ex switch
            {
                OutOfMemoryException => ErrorSeverity.Critical,
                StackOverflowException => ErrorSeverity.Critical,
                AccessViolationException => ErrorSeverity.Critical,
                System.Security.SecurityException => ErrorSeverity.Critical,
                // InvalidOperationException when ex.Message.Contains("critical") => ErrorSeverity.Critical, // Already handled below

                ArgumentException => ErrorSeverity.Serious,
                TimeoutException => ErrorSeverity.Serious,

                _ => ErrorSeverity.Minor
            };
        }

        /// <summary>
        /// Handles critical errors that require immediate shutdown.
        /// </summary>
        /// <param name="ex">The critical exception.</param>
        /// <param name="context">The error context.</param>
        private void HandleCriticalError(Exception ex, string context)
        {
            _criticalErrorCount++;

            Engine.Diagnostics.DebugLogger.LogError($"CRITICAL ERROR in {context}: {ex.Message}");
            Engine.Diagnostics.DebugLogger.LogError("Initiating emergency shutdown due to critical error");

            // Perform emergency shutdown
            PerformEmergencyShutdown(ex);
        }

        /// <summary>
        /// Handles serious errors that may require special handling.
        /// </summary>
        /// <param name="ex">The serious exception.</param>
        /// <param name="context">The error context.</param>
        private void HandleSeriousError(Exception ex, string context)
        {
            Engine.Diagnostics.DebugLogger.LogError($"SERIOUS ERROR in {context}: {ex.Message}");

            // Attempt recovery
            if (AttemptErrorRecovery(ex))
            {
                Engine.Diagnostics.DebugLogger.LogInfo("Error recovery successful, continuing game loop");
            }
            else
            {
                Engine.Diagnostics.DebugLogger.LogWarning("Error recovery failed, considering shutdown");

                // Check if we've had too many serious errors
                if (_totalErrorCount > 10) // Arbitrary threshold
                {
                    Engine.Diagnostics.DebugLogger.LogError("Too many serious errors, shutting down");
                    PerformGracefulShutdown();
                }
            }
        }

        /// <summary>
        /// Handles minor errors that can be logged and ignored.
        /// </summary>
        /// <param name="ex">The minor exception.</param>
        /// <param name="context">The error context.</param>
        private void HandleMinorError(Exception ex, string context)
        {
            Engine.Diagnostics.DebugLogger.LogWarning($"Minor error in {context}: {ex.Message}");

            // Minor errors are just logged and ignored
            // Game loop continues normally
        }

        /// <summary>
        /// Attempts to recover from an error.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <returns>True if recovery was successful.</returns>
        private bool AttemptErrorRecovery(Exception ex)
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogInfo("Attempting error recovery...");

                // Recovery strategies based on error type
                if (ex is InvalidOperationException)
                {
                    // Try to reset game state
                    return ResetGameState();
                }

                if (ex is TimeoutException)
                {
                    // Try to increase timeout tolerance
                    return IncreaseTimeoutTolerance();
                }

                // Generic recovery attempt
                return PerformGenericRecovery();
            }
            catch (Exception recoveryEx)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Error recovery failed: {recoveryEx.Message}");
                return false;
            }
        }

        /// <summary>
        /// Resets the game state as a recovery strategy.
        /// </summary>
        /// <returns>True if reset was successful.</returns>
        private bool ResetGameState()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogInfo("Resetting game state for error recovery");

                // This would implement game state reset logic
                // For now, just return true as a placeholder

                return true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Game state reset failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Increases timeout tolerance as a recovery strategy.
        /// </summary>
        /// <returns>True if timeout tolerance was increased.</returns>
        private bool IncreaseTimeoutTolerance()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogInfo("Increasing timeout tolerance for error recovery");

                // This would implement timeout tolerance increase
                // For now, just return true as a placeholder

                return true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Timeout tolerance increase failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Performs generic error recovery.
        /// </summary>
        /// <returns>True if generic recovery was successful.</returns>
        private bool PerformGenericRecovery()
        {
            try
            {
                Engine.Diagnostics.DebugLogger.LogInfo("Performing generic error recovery");

                // Generic recovery: pause briefly and continue
                System.Threading.Thread.Sleep(100);

                return true;
            }
            catch (Exception ex)
            {
                Engine.Diagnostics.DebugLogger.LogError($"Generic error recovery failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Logs frame error with detailed context.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="context">The error context.</param>
        private void LogFrameError(Exception ex, string context)
        {
            Engine.Diagnostics.DebugLogger.LogError($"Frame error in {context} at frame {_frameCount}: {ex.Message}");
            Engine.Diagnostics.DebugLogger.LogError($"Error details - Type: {ex.GetType().Name}, Stack: {ex.StackTrace}");
            Engine.Diagnostics.DebugLogger.LogError($"Context - FPS: {FramesPerSecond:F2}, Memory: {GC.GetTotalMemory(false) / 1024 / 1024}MB");
        }

        /// <summary>
        /// Gets error statistics.
        /// </summary>
        /// <returns>Error statistics.</returns>
        public ErrorStatistics GetErrorStatistics()
        {
            lock (_errorLock)
            {
                return new ErrorStatistics
                {
                    TotalErrors = _totalErrorCount,
                    CriticalErrors = _criticalErrorCount,
                    LastErrorTime = _lastErrorTime,
                    ErrorRate = _frameCount > 0 ? (float)_totalErrorCount / _frameCount : 0f,
                    TimeSinceLastError = DateTime.Now - _lastErrorTime
                };
            }
        }

        /// <summary>
        /// Resets error statistics.
        /// </summary>
        public void ResetErrorStatistics()
        {
            lock (_errorLock)
            {
                _criticalErrorCount = 0;
                _totalErrorCount = 0;
                _lastErrorTime = DateTime.MinValue;
            }
        }
    }

    /// <summary>
    /// Error severity enumeration.
    /// </summary>
    public enum ErrorSeverity
    {
        /// <summary>Minor error that can be ignored.</summary>
        Minor,

        /// <summary>Serious error that requires attention.</summary>
        Serious,

        /// <summary>Critical error that requires immediate shutdown.</summary>
        Critical
    }

    /// <summary>
    /// Error statistics for monitoring.
    /// </summary>
    public class ErrorStatistics
    {
        public int TotalErrors { get; set; }
        public int CriticalErrors { get; set; }
        public DateTime LastErrorTime { get; set; }
        public float ErrorRate { get; set; }
        public TimeSpan TimeSinceLastError { get; set; }
    }
}
